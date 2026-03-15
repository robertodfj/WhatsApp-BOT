namespace Bot.Api.Dto.Yeasy;

public sealed record BarbershopAvailabilitySlotDto(
    string Date,
    string Label,
    IReadOnlyList<BarbershopEmployeeAvailabilityDto> Employees);
