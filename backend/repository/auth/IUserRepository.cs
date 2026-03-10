using Bot.Api.Model.Auth;

namespace Bot.Api.Repository.Auth;

public interface IUserRepository
{
    Task<User?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default);
    Task<User> CreatePendingUserAsync(string phoneNumber, CancellationToken cancellationToken = default);
    Task<UserStatus?> GetStatusByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default);
    Task<bool> SetStatusByPhoneNumberAsync(string phoneNumber, UserStatus status, CancellationToken cancellationToken = default);
    Task<bool> SetNameByPhoneNumberAsync(string phoneNumber, string fullName, CancellationToken cancellationToken = default);
}
