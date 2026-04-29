using GrooveOn.Model.RequestObjects;
using GrooveOn.Services.Database;
using GrooveOn.Services.Exceptions;
using Konscious.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace GrooveOn.Services.Helpers
{
    public static class UserHelper
    {
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Argon2Iterations = 4;
        private const int Argon2MemorySize = 65536;
        private const int Argon2DegreeOfParallelism = 4;
        private const string TemporaryPasswordChars =
            "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789!@$?";

        public static string CreatePasswordHash(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password is required.", nameof(password));

            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            using var argon2 = new Argon2id(passwordBytes);
            argon2.Salt = salt;
            argon2.Iterations = Argon2Iterations;
            argon2.MemorySize = Argon2MemorySize;
            argon2.DegreeOfParallelism = Argon2DegreeOfParallelism;

            byte[] hash = argon2.GetBytes(HashSize);

            return string.Join('.',
                Convert.ToBase64String(salt),
                argon2.Iterations,
                argon2.MemorySize,
                argon2.DegreeOfParallelism,
                Convert.ToBase64String(hash)
            );
        }

        public static async Task AssignRoleByFlagsAsync(User entity, UserInsertRequest request, GrooveOnDbContext context)
        {
            int? roleId = null;

            if (request.IsAdmin == true)
                roleId = await GetRoleIdAsync("Admin", context);
            else
                roleId = await GetRoleIdAsync("User", context);

            if (roleId == null)
                throw new NotFoundException("Valid role not found.");

            entity.UserRoles.Add(new UserRole
            {
                UserId = entity.Id,
                RoleId = roleId.Value
            });
        }

        public static bool VerifyPassword(string password, string storedHash)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(storedHash))
                return false;

            try
            {
                var parts = storedHash.Split('.');
                if (parts.Length != 5)
                    return false;

                byte[] salt = Convert.FromBase64String(parts[0]);
                int iterations = int.Parse(parts[1]);
                int memorySize = int.Parse(parts[2]);
                int degreeOfParallelism = int.Parse(parts[3]);
                byte[] expectedHash = Convert.FromBase64String(parts[4]);

                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

                using var argon2 = new Argon2id(passwordBytes);
                argon2.Salt = salt;
                argon2.Iterations = iterations;
                argon2.MemorySize = memorySize;
                argon2.DegreeOfParallelism = degreeOfParallelism;

                byte[] computedHash = argon2.GetBytes(expectedHash.Length);

                return CryptographicOperations.FixedTimeEquals(computedHash, expectedHash);
            }
            catch (FormatException)
            {
                return false;
            }
            catch (OverflowException)
            {
                return false;
            }
        }

        public static string GenerateTemporaryPassword(int length = 12)
        {
            if (length <= 0)
                throw new ArgumentOutOfRangeException(nameof(length), "Password length must be greater than zero.");

            var result = new char[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = TemporaryPasswordChars[
                    RandomNumberGenerator.GetInt32(TemporaryPasswordChars.Length)];
            }

            return new string(result);
        }

        public static string CreateJwt(User user, IConfiguration configuration)
        {
            var jwtKey = configuration["JWT_SECRET"];
            var jwtIssuer = configuration["JWT_ISSUER"];
            var jwtAudience = configuration["JWT_AUDIENCE"];

            if (string.IsNullOrWhiteSpace(jwtKey))
                throw new InvalidOperationException("JWT_SECRET is not configured.");

            if (string.IsNullOrWhiteSpace(jwtIssuer))
                throw new InvalidOperationException("JWT_ISSUER is not configured.");

            if (string.IsNullOrWhiteSpace(jwtAudience))
                throw new InvalidOperationException("JWT_AUDIENCE is not configured.");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            foreach (var userRole in user.UserRoles)
            {
                if (!string.IsNullOrWhiteSpace(userRole.Role?.Name))
                {
                    claims.Add(new Claim(ClaimTypes.Role, userRole.Role!.Name));
                }
            }

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static async Task<int?> GetRoleIdAsync(string roleName, GrooveOnDbContext context)
        {
            var role = await context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
            return role?.Id;
        }
    }
}
