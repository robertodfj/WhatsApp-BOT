using System.Text.Json.Serialization;

namespace Bot.Api.Dto.Yeasy;

public sealed record YeasyAvailabilityRequestDto(
    [property: JsonPropertyName("commerce")] string Commerce,
    [property: JsonPropertyName("date")] string Date,
    [property: JsonPropertyName("servicesDuration")] int ServicesDuration,
    [property: JsonPropertyName("serviceCollection")] IReadOnlyList<YeasyServiceReferenceDto> ServiceCollection,
    [property: JsonPropertyName("userTimezone")] string UserTimezone);

public sealed record YeasyServiceReferenceDto(
    [property: JsonPropertyName("uuid")] string Uuid);
