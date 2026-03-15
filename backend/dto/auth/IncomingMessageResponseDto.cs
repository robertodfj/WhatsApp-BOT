namespace Bot.Api.Dto.Auth;

public enum IncomingMessageActionDto
{
    AskForEmail = 0,
    AskForName = 1,
    AskNameConfirmation = 2,
    Welcome = 3,
    ExecuteCommand = 4,
    Blocked = 5,
    InvalidEmail = 6,
    InvalidName = 7,
    InvalidConfirmation = 8
}

public sealed record IncomingMessageResponseDto(IncomingMessageActionDto Action, string Reply);
