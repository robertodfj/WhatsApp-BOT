using System.Text.Json.Serialization;

namespace Bot.Api.Dto.Yeasy;

public sealed record YeasyServiceItemDto(
    [property: JsonPropertyName("uuid")] string Uuid,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("price")] decimal Price,
    [property: JsonPropertyName("decimal")] int Decimal,
    [property: JsonPropertyName("defaultDuration")] int DefaultDuration);
