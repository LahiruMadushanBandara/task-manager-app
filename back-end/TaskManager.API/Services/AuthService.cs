using TaskManager.API.Helpers;
using TaskManager.API.Repositories.Contracts;
using TaskManager.API.Services.Contracts;

namespace TaskManager.API.Services;

public class AuthService(IUserRepository userRepository) : IAuthService
{
    public async Task<int?> ValidateCredentialsAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByUsernameAsync(username, cancellationToken);
        if (user is null) return null;

        return PasswordHasher.Verify(password, user.Salt, user.PasswordHash) ? user.Id : null;
    }
}
