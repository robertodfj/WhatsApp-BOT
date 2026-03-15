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

    public async Task<BarbershopAvailabilityDto> GetAvailabilityAsync(BarbershopAvailabilityQueryDto query, CancellationToken cancellationToken = default)
    {
        return await _yeasyRepository.GetAvailabilityAsync(query, cancellationToken);
    }

    public async Task<bool> IsTimeAvailableAsync(
        BarbershopAvailabilityQueryDto query,
        string requestedTime,
        string? barberName = null,
        CancellationToken cancellationToken = default)
    {
        var availability = await _yeasyRepository.GetAvailabilityAsync(query, cancellationToken);
        var requestedLabel = NormalizeTime(requestedTime);
        if (string.IsNullOrWhiteSpace(requestedLabel))
        {
            return false;
        }

        var allSlots = availability.Morning.Concat(availability.Afternoon);
        var slot = allSlots.FirstOrDefault(x => string.Equals(NormalizeTime(x.Label), requestedLabel, StringComparison.OrdinalIgnoreCase));
        if (slot is null)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(barberName))
        {
            return true;
        }

        return slot.Employees.Any(employee => MatchesBarber(employee, barberName));
    }

    private static bool MatchesBarber(BarbershopEmployeeAvailabilityDto employee, string barberName)
    {
        var normalizedRequested = NormalizeText(barberName);
        var fullName = NormalizeText($"{employee.Name} {employee.Surname}");
        var name = NormalizeText(employee.Name);
        var surname = NormalizeText(employee.Surname);

        return fullName.Contains(normalizedRequested, StringComparison.Ordinal)
            || name.Contains(normalizedRequested, StringComparison.Ordinal)
            || surname.Contains(normalizedRequested, StringComparison.Ordinal);
    }

    private static string NormalizeTime(string value)
    {
        return (value ?? string.Empty).Trim();
    }

    private static string NormalizeText(string value)
    {
        return (value ?? string.Empty).Trim().ToUpperInvariant();
    }
}