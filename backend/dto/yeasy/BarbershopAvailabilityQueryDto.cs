namespace Bot.Api.Dto.Yeasy;

public sealed record BarbershopAvailabilityQueryDto(
    string ServiceUuid,
    string Date,
    int ServicesDuration,
    string UserTimezone);
