using Bot.Api.Dto.Auth;

namespace Bot.Api.Service.Auth;

public interface IUserOnboardingService
{
    Task<UserVerificationStateDto> GetVerificationStateAsync(string phoneNumber, CancellationToken cancellationToken = default);
    Task<UserVerificationStateDto> CreatePendingUserAsync(string phoneNumber, CancellationToken cancellationToken = default);
    Task<UserVerificationStateDto> SetPendingEmailAsync(string phoneNumber, string email, CancellationToken cancellationToken = default);
    Task<UserVerificationStateDto> SetPendingNameAsync(string phoneNumber, string fullName, CancellationToken cancellationToken = default);
    Task<UserVerificationStateDto> ConfirmPendingNameAsync(string phoneNumber, bool isConfirmed, CancellationToken cancellationToken = default);
    Task<IncomingMessageResponseDto> ExecuteCommandAsync(string phoneNumber, string text, CancellationToken cancellationToken = default);
}
