namespace Bot.Api.Dto.Auth;

public sealed record IncomingMessageRequestDto(string PhoneNumber, string Text);
