using Bot.Api.Dto.Yeasy;

namespace Bot.Api.Repository.Yeasy;

public interface IYeasyRepository
{
    Task<IReadOnlyList<BarbershopServiceDto>> GetServicesAsync(CancellationToken cancellationToken = default);
}