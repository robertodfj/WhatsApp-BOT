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

    public async Task<AvailabilityResponseDto> GetAvailabilityResponseAsync(AvailabilityRequestBodyDto request, CancellationToken cancellationToken = default)
    {
        var query = new BarbershopAvailabilityQueryDto(
            request.ServiceUuid,
            request.Date,
            30,
            "Europe/Madrid");

        var availability = await _yeasyRepository.GetAvailabilityAsync(query, cancellationToken);
        var filtered = FilterByDate(availability, request.Date);

        var data = filtered.Morning
            .Concat(filtered.Afternoon)
            .Where(slot => slot.Employees.Count > 0)
            .OrderBy(slot => slot.Date)
            .Select(slot => new AvailabilitySlotResponseDto(
                slot.Date,
                slot.Label,
                slot.Employees
                    .Select(employee => new AvailabilityEmployeeResponseDto(
                        employee.Uuid,
                        employee.Name,
                        employee.Surname))
                    .ToList()))
            .ToList();

        return new AvailabilityResponseDto(request.ServiceUuid, data);
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

    public BarbershopAvailabilityDto FilterByDate(BarbershopAvailabilityDto availability, string targetDate)
    {
        var morning = availability.Morning
            .Where(slot => ExtractDateFromIso(slot.Date) == targetDate)
            .ToList();

        var afternoon = availability.Afternoon
            .Where(slot => ExtractDateFromIso(slot.Date) == targetDate)
            .ToList();

        return new BarbershopAvailabilityDto(morning, afternoon);
    }

    private static string ExtractDateFromIso(string isoDateString)
    {
        if (string.IsNullOrWhiteSpace(isoDateString))
        {
            return string.Empty;
        }

        // Extract YYYY-MM-DD from ISO format like "2026-03-20T10:00:00.000Z"
        var parts = isoDateString.Split('T');
        return parts.Length > 0 ? parts[0] : string.Empty;
    }
}