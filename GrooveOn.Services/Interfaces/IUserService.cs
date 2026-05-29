using GrooveOn.Model.RequestObjects;
using GrooveOn.Model.ResponseObject;
using GrooveOn.Model.ResponseObjects;
using GrooveOn.Model.SearchObjects;

namespace GrooveOn.Services.Interfaces
{
    public interface IUserService : ICRUDService<UserResponse, UserSearchObject, UserInsertRequest, UserUpdateRequest>
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task ForgotPasswordAsync(string email);
        Task ResetPasswordWithTokenAsync(string token, string newPassword);
        Task ChangePasswordAsync(int userId, ChangePasswordRequest request);
        Task<bool> CurrentUserHasPremiumAsync();
        Task AssignRolesAsync(int userId, List<int> roleIds);
    }
}
