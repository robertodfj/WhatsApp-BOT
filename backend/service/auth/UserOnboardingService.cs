using Bot.Api.Dto.Auth;
using Bot.Api.Repository.Auth;

namespace Bot.Api.Service.Auth;

public class UserOnboardingService : IUserOnboardingService
{
    private readonly IUserRepository _userRepository;

    public UserOnboardingService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserVerificationStateDto> GetVerificationStateAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        return await _userRepository.GetVerificationStateByPhoneNumberAsync(phoneNumber, cancellationToken);
    }

    public async Task<UserVerificationStateDto> CreatePendingUserAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        return await _userRepository.CreatePendingUserAsync(phoneNumber, cancellationToken);
    }

    public async Task<UserVerificationStateDto> SetPendingEmailAsync(string phoneNumber, string email, CancellationToken cancellationToken = default)
    {
        return await _userRepository.SetPendingEmailByPhoneNumberAsync(phoneNumber, email, cancellationToken);
    }

    public async Task<UserVerificationStateDto> SetPendingNameAsync(string phoneNumber, string fullName, CancellationToken cancellationToken = default)
    {
        return await _userRepository.SetPendingNameByPhoneNumberAsync(phoneNumber, fullName, cancellationToken);
    }

    public async Task<UserVerificationStateDto> ConfirmPendingNameAsync(string phoneNumber, bool isConfirmed, CancellationToken cancellationToken = default)
    {
        return await _userRepository.ConfirmPendingNameByPhoneNumberAsync(phoneNumber, isConfirmed, cancellationToken);
    }

    public Task<IncomingMessageResponseDto> ExecuteCommandAsync(string phoneNumber, string text, CancellationToken cancellationToken = default)
    {
        var normalizedText = (text ?? string.Empty).Trim().ToUpperInvariant();

        if (normalizedText == "MENU")
        {
            return Task.FromResult(new IncomingMessageResponseDto(IncomingMessageActionDto.ExecuteCommand, "Menú: AYUDA, ESTADO, SALDO."));
        }

        if (normalizedText == "AYUDA")
        {
            return Task.FromResult(new IncomingMessageResponseDto(IncomingMessageActionDto.ExecuteCommand, "Comandos disponibles: MENU, AYUDA, ESTADO."));
        }

        return Task.FromResult(new IncomingMessageResponseDto(IncomingMessageActionDto.ExecuteCommand, "Comando recibido. Procesando..."));
    }
}
