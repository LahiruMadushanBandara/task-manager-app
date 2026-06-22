using TaskManager.API.Models.Entities;

namespace TaskManager.API.Repositories.Contracts;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
}
