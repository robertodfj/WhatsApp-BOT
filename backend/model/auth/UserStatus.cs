namespace Bot.Api.Model.Auth
{

    public enum UserStatus
    {
        PendingEmail = 0,
        PendingName = 1,
        PendingNameConfirmation = 2,
        Active = 3,
        Blocked = 4
    }
}