namespace TaskManager.API.Services.Contracts;

public interface IAuthService
{
    Task<int?> ValidateCredentialsAsync(string username, string password);
}
