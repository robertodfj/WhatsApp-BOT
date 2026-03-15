using Bot.Api.Dto.Yeasy;
using Bot.Api.Repository.Yeasy;

namespace Bot.Api.Service.Yeasy;

public class YeasyService : IYeasyService
{
    private readonly IYeasyRepository _yeasyRepository;

    public YeasyService(IYeasyRepository yeasyRepository)
    {
        _yeasyRepository = yeasyRepository;
    }

    public async Task<IReadOnlyList<BarbershopServiceDto>> GetServicesAsync(CancellationToken cancellationToken = default)
    {
        return await _yeasyRepository.GetServicesAsync(cancellationToken);
    }
}