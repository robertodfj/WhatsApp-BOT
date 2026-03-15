namespace Bot.Api.Model.Yeasy;

public class YeasyOptions
{
    public const string SectionName = "Yeasy";

    public string BaseUrl { get; set; } = "https://apitest.yeasyapp.com/";

    public string CommerceUuid { get; set; } = string.Empty;
}
