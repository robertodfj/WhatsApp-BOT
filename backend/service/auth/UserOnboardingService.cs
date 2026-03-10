using Bot.Api.Model.Auth;
using Bot.Api.Repository.Auth;

namespace Bot.Api.Service.Auth;

public class UserOnboardingService : IUserOnboardingService
{
    private readonly IUserRepository _userRepository;

    public UserOnboardingService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IncomingMessageResult> HandleIncomingMessageAsync(string phoneNumber, string text, CancellationToken cancellationToken = default)
    {
        var normalizedPhone = NormalizePhone(phoneNumber);
        var messageText = (text ?? string.Empty).Trim();

        var user = await _userRepository.GetByPhoneNumberAsync(normalizedPhone, cancellationToken);
        if (user is null)
        {
            await _userRepository.CreatePendingUserAsync(normalizedPhone, cancellationToken);
            return new IncomingMessageResult(IncomingMessageAction.AskForName, "Hola, dime tu nombre completo.");
        }

        if (user.Status == UserStatus.PendingName)
        {
            if (!IsValidFullName(messageText))
            {
                return new IncomingMessageResult(IncomingMessageAction.InvalidName, "Nombre no válido. Escribe tu nombre completo.");
            }

            await _userRepository.SetNameByPhoneNumberAsync(normalizedPhone, messageText, cancellationToken);
            await _userRepository.SetStatusByPhoneNumberAsync(normalizedPhone, UserStatus.Active, cancellationToken);

            return new IncomingMessageResult(IncomingMessageAction.Welcome, "¡Bienvenido! Ya estás registrado. Comandos: MENU, AYUDA.");
        }

        if (user.Status == UserStatus.Blocked)
        {
            return new IncomingMessageResult(IncomingMessageAction.Blocked, "Tu usuario está bloqueado. Contacta al administrador.");
        }

        return new IncomingMessageResult(IncomingMessageAction.ExecuteCommand, "Comando recibido. Procesando...");
    }

    private static string NormalizePhone(string phoneNumber)
    {
        return new string((phoneNumber ?? string.Empty).Where(char.IsDigit).ToArray());
    }

    private static bool IsValidFullName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            return false;
        }

        var compact = fullName.Trim();
        if (compact.Length < 3 || compact.Length > 150)
        {
            return false;
        }

        return compact.Contains(' ');
    }
}
