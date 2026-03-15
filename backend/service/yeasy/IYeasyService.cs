using Bot.Api.Dto.Yeasy;

namespace Bot.Api.Service.Yeasy;

public interface IYeasyService
{
    Task<IReadOnlyList<BarbershopServiceDto>> GetServicesAsync(CancellationToken cancellationToken = default);
}