using GrooveOn.Services.Database;
using GrooveOn.Services.Interfaces;
using GrooveOn.Model.RequestObjects;
using GrooveOn.Model.ResponseObjects;
using GrooveOn.Model.SearchObjects;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using GrooveOn.Model.ResponseObject;
using GrooveOn.Services.Helpers;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Text;
using GrooveOn.MailingService.Messages;
using RabbitMQ.Client;
using GrooveOn.Services.Exceptions;
using GrooveOn.MailingService.Configuration;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace GrooveOn.Services.Services
{
    public class UserService : BaseCRUDService<UserResponse, UserSearchObject, User, UserInsertRequest, UserUpdateRequest>, IUserService
    {
        private readonly IConfiguration _configuration;
        private readonly IConnection _rabbitConnection;
        private readonly AppConfig _appConfig;

        public UserService(
            GrooveOnDbContext context,
            IMapper mapper,
            IConfiguration configuration,
            IConnection rabbitConnection,
            IOptions<AppConfig> appConfig
        ) : base(context, mapper)
        {
            _configuration = configuration;
            _rabbitConnection = rabbitConnection;
            _appConfig = appConfig.Value;
        }

        protected override IQueryable<User> ApplyFilter(IQueryable<User> query, UserSearchObject? search = null)
        {
            query = base.ApplyFilter(query, search);

            if (!string.IsNullOrWhiteSpace(search?.FTS))
            {
                var fts = search.FTS.ToLower();

                query = query.Where(x =>
                    x.Username.ToLower().Contains(fts) ||
                    x.Email.ToLower().Contains(fts) ||
                    (x.FirstName != null && x.FirstName.ToLower().Contains(fts)) ||
                    (x.LastName != null && x.LastName.ToLower().Contains(fts))
                );
            }

            return query;
        }

        protected override User MapInsertToEntity(User entity, UserInsertRequest request)
        {
            _mapper.Map(request, entity);

            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                entity.PasswordHash = UserHelper.CreatePasswordHash(request.Password);
            }

            entity.JoinDate = DateTime.UtcNow;
            return entity;
        }

        protected override void MapUpdateToEntity(User entity, UserUpdateRequest request)
        {
            var joinDate = entity.JoinDate;

            _mapper.Map(request, entity);

            entity.JoinDate = joinDate;

            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                entity.PasswordHash = UserHelper.CreatePasswordHash(request.Password);
            }
        }

        protected override async Task BeforeInsert(User entity, UserInsertRequest request)
        {
            var exists = await _context.Users.AnyAsync(x =>
                x.Username == entity.Username || x.Email == entity.Email);

            if (exists)
                throw new InvalidOperationException("User with the same username/email already exists");

            await UserHelper.AssignRoleByFlagsAsync(entity, request, _context);
        }

        protected override async Task BeforeUpdate(User entity, UserUpdateRequest request)
        {
            var exists = await _context.Users.AnyAsync(x =>
                x.Id != entity.Id && (x.Username == request.Username || x.Email == request.Email));

            if (exists)
                throw new InvalidOperationException("User with the same username/email already exists");
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(x => x.Username == request.Username);

            if (user == null || !user.IsActive)
                throw new UnauthorizedAccessException("Invalid username or password");

            if (!UserHelper.VerifyPassword(request.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid username or password");

            var token = UserHelper.CreateJwt(user, _configuration);

            var response = new LoginResponse
            {
                UserId = user.Id,
                UserName = request.Username,
                Token = token,
                Roles = user.UserRoles
                    .Select(ur => ur.Role.Name)
                    .ToList()
            };

            user.LastLogin = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return response;
        }

        public async Task ForgotPasswordAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new UserException("Email je obavezan.");

            var normalizedEmail = email.Trim();

            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Email == normalizedEmail);

            if (user == null)
                throw new UserException("Email nije povezan ni sa jednim nalogom.");

            var newPassword = GenerateRandomPassword();

            user.PasswordHash = UserHelper.CreatePasswordHash(newPassword);

            await _context.SaveChangesAsync();

            await using var channel = await _rabbitConnection.CreateChannelAsync();

            string queueName = _appConfig.ResetPasswordQueue;

            await channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            var message = new ResetPasswordEmailMessage
            {
                To = user.Email!,
                Name = user.FirstName ?? "Korisnik",
                UserName = user.Username,
                NewPassword = newPassword
            };

            var body = Encoding.UTF8.GetBytes(
                JsonSerializer.Serialize(message)
            );

            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: queueName,
                body: body
            );
        }

        private string GenerateRandomPassword(int length = 10)
        {
            const string chars =
                "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789!@$?";

            var bytes = new byte[length];
            RandomNumberGenerator.Fill(bytes);

            var result = new char[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = chars[bytes[i] % chars.Length];
            }

            return new string(result);
        }
    }
}