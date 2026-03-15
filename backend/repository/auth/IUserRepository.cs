using Bot.Api.Dto.Auth;
using Bot.Api.Model.Auth;

namespace Bot.Api.Repository.Auth;

public interface IUserRepository
{
    Task<UserVerificationStateDto> GetVerificationStateByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default);
    Task<UserVerificationStateDto> CreatePendingUserAsync(string phoneNumber, CancellationToken cancellationToken = default);
    Task<UserVerificationStateDto> SetPendingEmailByPhoneNumberAsync(string phoneNumber, string email, CancellationToken cancellationToken = default);
    Task<UserVerificationStateDto> SetPendingNameByPhoneNumberAsync(string phoneNumber, string fullName, CancellationToken cancellationToken = default);
    Task<UserVerificationStateDto> ConfirmPendingNameByPhoneNumberAsync(string phoneNumber, bool isConfirmed, CancellationToken cancellationToken = default);
}
