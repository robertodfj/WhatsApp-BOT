namespace Bot.Api.Dto.Auth;

public enum IncomingMessageActionDto
{
    AskForName = 0,
    AskNameConfirmation = 1,
    Welcome = 2,
    ExecuteCommand = 3,
    Blocked = 4,
    InvalidName = 5,
    InvalidConfirmation = 6
}

public sealed record IncomingMessageResponseDto(IncomingMessageActionDto Action, string Reply);
