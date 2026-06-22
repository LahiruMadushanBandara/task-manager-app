using Microsoft.EntityFrameworkCore;
using TaskManager.API.Data;
using TaskManager.API.Models.Entities;
using TaskManager.API.Repositories.Contracts;

namespace TaskManager.API.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default) =>
        context.Users.AsNoTracking()
               .FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
}
