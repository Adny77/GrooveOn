using GrooveOn.Services.Database;
using GrooveOn.Services.Interfaces;
using GrooveOn.Model.RequestObjects;
using GrooveOn.Model.ResponseObjects;
using GrooveOn.Model.SearchObjects;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using GrooveOn.Model.ResponseObject;
using GrooveOn.Services.Helpers;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Text;
// using RabbitMQ.Client;
// using GrooveOn.EmailConsumer.Messages;

namespace GrooveOn.Services.Services
{
    public class UserService : BaseCRUDService<UserResponse, UserSearchObject, User, UserInsertRequest, UserUpdateRequest>, IUserService
    {
        IConfiguration _configuration;
        //private readonly IConnection _rabbitConnection;
        public UserService(GrooveOnDbContext context, IMapper mapper, IConfiguration configuration) : base(context, mapper)
        {
            _configuration = configuration;
           // _rabbitConnection = rabbitConnection;
        }



        protected override User MapInsertToEntity(User entity, UserInsertRequest request)
        {
            _mapper.Map(request, entity);

            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                UserHelper.CreatePasswordHash(request.Password, out var hash, out var salt);
                entity.PasswordHash = hash;
                entity.PasswordSalt = salt;
            }

            entity.JoinDate = DateTime.UtcNow;
            return entity;
        }

        protected override void MapUpdateToEntity(User entity, UserUpdateRequest request)
        {
            var JoinDate = entity.JoinDate;

            _mapper.Map(request, entity);

            entity.JoinDate = JoinDate;

            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                UserHelper.CreatePasswordHash(request.Password, out var hash, out var salt);
                entity.PasswordHash = hash;
                entity.PasswordSalt = salt;
            }
        }

        protected override async Task BeforeInsert(User entity, UserInsertRequest request)
        {
            var exists = await _context.Users.AnyAsync(x =>
                x.Username == entity.Username || x.Email == entity.Email);

            if (exists)
                throw new InvalidOperationException("Korisnik sa istim username/email već postoji.");

            await UserHelper.AssignRoleByFlagsAsync(entity, request, _context);
        }

        protected override async Task BeforeUpdate(User entity, UserUpdateRequest request)
        {
            var exists = await _context.Users.AnyAsync(x =>
                x.Id != entity.Id && (x.Username == request.Username || x.Email == request.Email));

            if (exists)
                throw new InvalidOperationException("Korisnik sa istim username/email već postoji.");
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(x => x.Username == request.Username);

            if (user == null || !user.IsActive)
                throw new UnauthorizedAccessException("Pogrešan username ili password.");

            if (!UserHelper.VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt))
                throw new UnauthorizedAccessException("Pogrešan username ili password.");

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

        // public async Task ForgotPasswordAsync(string email)
        // {
        //     var user = await _context.Users
        //         .FirstOrDefaultAsync(x => x.Email == email);

        //     if (user == null)
        //         throw new Exception("Email nije povezan ni sa jednim nalogom.");

        //     var newPassword = GenerateRandomPassword();

        //     UserHelper.CreatePasswordHash(newPassword, out string hash, out string salt);

        //     user.PasswordHash = hash;
        //     user.PasswordSalt = salt;

        //     await _context.SaveChangesAsync();

        //     var channel = await _rabbitConnection.CreateChannelAsync();

        //     await channel.QueueDeclareAsync(
        //         queue: "email.reset-password",
        //         durable: true,
        //         exclusive: false,
        //         autoDelete: false,
        //         arguments: null
        //     );

        //     var message = new ResetPasswordEmailMessage
        //     {
        //         To = user.Email!,
        //         UserName = user.FirstName ?? user.Username ?? "Korisnik",
        //         NewPassword = newPassword
        //     };

        //     var body = Encoding.UTF8.GetBytes(
        //         JsonSerializer.Serialize(message)
        //     );

        //     await channel.BasicPublishAsync(
        //         exchange: "",
        //         routingKey: "email.reset-password",
        //         body: body
        //     );
        // }

        private string GenerateRandomPassword(int length = 10)
        {
            const string chars =
                "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789!@$?";

            var random = new Random();
            return new string(
                Enumerable.Repeat(chars, length)
                    .Select(s => s[random.Next(s.Length)])
                    .ToArray()
            );
        }
    }
}
