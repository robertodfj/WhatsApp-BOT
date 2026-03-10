using Bot.Api.Database;
using Bot.Api.Dto.Auth;
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

    public async Task<UserVerificationStateDto> GetVerificationStateByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber, cancellationToken);
        if (user is null)
        {
            return new UserVerificationStateDto(null, phoneNumber, null, UserStatus.PendingName, false, false);
        }

        return ToStateDto(user, true);
    }

    public async Task<UserVerificationStateDto> CreatePendingUserAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        var user = new User
        {
            PhoneNumber = phoneNumber,
            Status = UserStatus.PendingName,
            Name = null
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return ToStateDto(user, true);
    }

    public async Task<UserVerificationStateDto> SetPendingNameByPhoneNumberAsync(string phoneNumber, string fullName, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber, cancellationToken);
        if (user is null)
        {
            return new UserVerificationStateDto(null, phoneNumber, null, UserStatus.PendingName, false, false);
        }

        user.Name = fullName;
        user.Status = UserStatus.PendingNameConfirmation;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return ToStateDto(user, true);
    }

    public async Task<UserVerificationStateDto> ConfirmPendingNameByPhoneNumberAsync(string phoneNumber, bool isConfirmed, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber, cancellationToken);
        if (user is null)
        {
            return new UserVerificationStateDto(null, phoneNumber, null, UserStatus.PendingName, false, false);
        }

        if (isConfirmed)
        {
            user.Status = UserStatus.Active;
        }
        else
        {
            user.Name = null;
            user.Status = UserStatus.PendingName;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return ToStateDto(user, true);
    }

    private static UserVerificationStateDto ToStateDto(User user, bool exists)
    {
        var isVerified = user.Status == UserStatus.Active && !string.IsNullOrWhiteSpace(user.Name);
        return new UserVerificationStateDto(user.Id, user.PhoneNumber, user.Name, user.Status, exists, isVerified);
    }
}
