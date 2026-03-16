using Bot.Api.Dto.Yeasy;

namespace Bot.Api.Service.Yeasy;

public interface IYeasyService
{
    Task<IReadOnlyList<BarbershopServiceDto>> GetServicesAsync(CancellationToken cancellationToken = default);
    Task<AvailabilityResponseDto> GetAvailabilityResponseAsync(AvailabilityRequestBodyDto request, CancellationToken cancellationToken = default);
    Task<BarbershopAvailabilityDto> GetAvailabilityAsync(BarbershopAvailabilityQueryDto query, CancellationToken cancellationToken = default);
    Task<bool> IsTimeAvailableAsync(
        BarbershopAvailabilityQueryDto query,
        string requestedTime,
        string? barberName = null,
        CancellationToken cancellationToken = default);
    BarbershopAvailabilityDto FilterByDate(BarbershopAvailabilityDto availability, string targetDate);
}