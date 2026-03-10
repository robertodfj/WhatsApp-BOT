using Bot.Api.Database;
using Bot.Api.Model.Auth;
using Microsoft.EntityFrameworkCore;

namespace Bot.Api.Repository.Auth;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _dbContext;

    public UserRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<User?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        return _dbContext.Users.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber, cancellationToken);
    }

    public async Task<User> CreatePendingUserAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        var user = new User
        {
            PhoneNumber = phoneNumber,
            Status = UserStatus.PendingName,
            Name = null
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return user;
    }

    public async Task<UserStatus?> GetStatusByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .Where(x => x.PhoneNumber == phoneNumber)
            .Select(x => (UserStatus?)x.Status)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> SetStatusByPhoneNumberAsync(string phoneNumber, UserStatus status, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber, cancellationToken);
        if (user is null)
        {
            return false;
        }

        user.Status = status;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> SetNameByPhoneNumberAsync(string phoneNumber, string fullName, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber, cancellationToken);
        if (user is null)
        {
            return false;
        }

        user.Name = fullName;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
