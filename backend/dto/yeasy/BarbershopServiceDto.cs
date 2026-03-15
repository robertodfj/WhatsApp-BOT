namespace Bot.Api.Dto.Yeasy;

public sealed record BarbershopServiceDto(
    string Uuid,
    string Name,
    decimal Price,
    int Decimal,
    int DefaultDuration);
