namespace Bot.Api.Model.Auth
{

    public enum UserStatus
    {
        PendingName = 0,
        PendingNameConfirmation = 1,
        Active = 2,
        Blocked = 3
    }
}