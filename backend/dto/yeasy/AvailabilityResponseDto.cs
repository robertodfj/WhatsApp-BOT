namespace Bot.Api.Dto.Yeasy;

public sealed record AvailabilityResponseDto(
    string ServiceUuid,
    IReadOnlyList<AvailabilitySlotResponseDto> Data);

public sealed record AvailabilitySlotResponseDto(
    string Date,
    string Label,
    IReadOnlyList<AvailabilityEmployeeResponseDto> Employee);

public sealed record AvailabilityEmployeeResponseDto(
    string Uuid,
    string Name,
    string Surname);