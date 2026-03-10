namespace Bot.Api.Service.Auth;

public enum IncomingMessageAction
{
    AskForName = 0,
    Welcome = 1,
    ExecuteCommand = 2,
    Blocked = 3,
    InvalidName = 4
}

public sealed record IncomingMessageResult(IncomingMessageAction Action, string Reply);

public interface IUserOnboardingService
{
    Task<IncomingMessageResult> HandleIncomingMessageAsync(string phoneNumber, string text, CancellationToken cancellationToken = default);
}
