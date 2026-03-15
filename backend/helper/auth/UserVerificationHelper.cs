using Bot.Api.Dto.Auth;
using Bot.Api.Model.Auth;
using Bot.Api.Service.Auth;

namespace Bot.Api.Helper.Auth;

public class UserVerificationHelper : IUserVerificationHelper
{
    private readonly IUserOnboardingService _userOnboardingService;

    public UserVerificationHelper(IUserOnboardingService userOnboardingService)
    {
        _userOnboardingService = userOnboardingService;
    }

    public async Task<IncomingMessageResponseDto> ProcessAsync(IncomingMessageRequestDto request, CancellationToken cancellationToken = default)
    {
        var normalizedPhone = NormalizePhone(request.PhoneNumber);
        var messageText = (request.Text ?? string.Empty).Trim();

        var verificationState = await _userOnboardingService.GetVerificationStateAsync(normalizedPhone, cancellationToken);

        if (!verificationState.Exists)
        {
            await _userOnboardingService.CreatePendingUserAsync(normalizedPhone, cancellationToken);
            return new IncomingMessageResponseDto(IncomingMessageActionDto.AskForEmail, "Hola, para registrarte dime tu correo electrónico.");
        }

        if (verificationState.Status == UserStatus.Blocked)
        {
            return new IncomingMessageResponseDto(IncomingMessageActionDto.Blocked, "Tu usuario está bloqueado. Contacta al administrador.");
        }

        if (verificationState.Status == UserStatus.PendingEmail)
        {
            if (!IsValidEmail(messageText))
            {
                return new IncomingMessageResponseDto(IncomingMessageActionDto.InvalidEmail, "Correo no válido. Escribe un email correcto, por ejemplo nombre@dominio.com.");
            }

            var pendingNameState = await _userOnboardingService.SetPendingEmailAsync(normalizedPhone, messageText, cancellationToken);
            if (!pendingNameState.Exists || string.IsNullOrWhiteSpace(pendingNameState.Email))
            {
                return new IncomingMessageResponseDto(IncomingMessageActionDto.InvalidEmail, "No se pudo guardar el correo. Inténtalo de nuevo.");
            }

            return new IncomingMessageResponseDto(IncomingMessageActionDto.AskForName, "Perfecto. Ahora escribe tu nombre completo.");
        }

        if (verificationState.Status == UserStatus.PendingName)
        {
            if (!IsValidFullName(messageText))
            {
                return new IncomingMessageResponseDto(IncomingMessageActionDto.InvalidName, "Nombre no válido. Escribe tu nombre completo.");
            }

            var pendingConfirmationState = await _userOnboardingService.SetPendingNameAsync(normalizedPhone, messageText, cancellationToken);
            if (!pendingConfirmationState.Exists || string.IsNullOrWhiteSpace(pendingConfirmationState.Name))
            {
                return new IncomingMessageResponseDto(IncomingMessageActionDto.InvalidName, "No se pudo completar el registro. Inténtalo de nuevo.");
            }

            return new IncomingMessageResponseDto(
                IncomingMessageActionDto.AskNameConfirmation,
                $"¿Tu nombre es {pendingConfirmationState.Name}? Responde SI o NO.");
        }

        if (verificationState.Status == UserStatus.PendingNameConfirmation)
        {
            if (IsAffirmative(messageText))
            {
                var activatedState = await _userOnboardingService.ConfirmPendingNameAsync(normalizedPhone, true, cancellationToken);
                if (!activatedState.IsVerified)
                {
                    return new IncomingMessageResponseDto(IncomingMessageActionDto.InvalidConfirmation, "No se pudo confirmar el nombre. Inténtalo de nuevo.");
                }

                return new IncomingMessageResponseDto(IncomingMessageActionDto.Welcome, "Perfecto, ya estás verificado. ¿Qué consulta quieres hacer?");
            }

            if (IsNegative(messageText))
            {
                await _userOnboardingService.ConfirmPendingNameAsync(normalizedPhone, false, cancellationToken);
                return new IncomingMessageResponseDto(IncomingMessageActionDto.AskForName, "De acuerdo, vuelve a escribir tu nombre completo.");
            }

            return new IncomingMessageResponseDto(IncomingMessageActionDto.InvalidConfirmation, "Responde únicamente SI o NO para confirmar tu nombre.");
        }

        return await _userOnboardingService.ExecuteCommandAsync(normalizedPhone, messageText, cancellationToken);
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

    private static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        var trimmed = email.Trim();
        if (trimmed.Length < 5 || trimmed.Length > 200)
        {
            return false;
        }

        try
        {
            var parsed = new System.Net.Mail.MailAddress(trimmed);
            return parsed.Address.Equals(trimmed, StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }

    private static bool IsAffirmative(string text)
    {
        var normalized = RemoveDiacritics((text ?? string.Empty).Trim().ToUpperInvariant());
        return normalized is "SI" or "S" or "YES" or "Y";
    }

    private static bool IsNegative(string text)
    {
        var normalized = RemoveDiacritics((text ?? string.Empty).Trim().ToUpperInvariant());
        return normalized is "NO" or "N";
    }

    private static string RemoveDiacritics(string input)
    {
        return input.Replace("Í", "I").Replace("É", "E").Replace("Á", "A").Replace("Ó", "O").Replace("Ú", "U");
    }
}
