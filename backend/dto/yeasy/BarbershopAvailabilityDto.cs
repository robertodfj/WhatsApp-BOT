namespace Bot.Api.Dto.Yeasy;

public sealed record BarbershopAvailabilityDto(
    IReadOnlyList<BarbershopAvailabilitySlotDto> Morning,
    IReadOnlyList<BarbershopAvailabilitySlotDto> Afternoon);
