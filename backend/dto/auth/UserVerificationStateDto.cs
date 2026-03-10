using Bot.Api.Model.Auth;

namespace Bot.Api.Dto.Auth;

public sealed record UserVerificationStateDto(
    int? UserId,
    string PhoneNumber,
    string? Name,
    UserStatus Status,
    bool Exists,
    bool IsVerified);
