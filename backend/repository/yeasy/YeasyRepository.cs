using System.Text.Json;
using System.Text;
using Bot.Api.Dto.Yeasy;
using Bot.Api.Model.Yeasy;
using Microsoft.Extensions.Options;

namespace Bot.Api.Repository.Yeasy;

public class YeasyRepository : IYeasyRepository
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _httpClient;
    private readonly YeasyOptions _options;

    public YeasyRepository(HttpClient httpClient, IOptions<YeasyOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;

        if (_httpClient.BaseAddress is null)
        {
            var baseUrl = string.IsNullOrWhiteSpace(_options.BaseUrl) ? "https://apitest.yeasyapp.com/" : _options.BaseUrl;
            _httpClient.BaseAddress = new Uri(baseUrl);
        }
    }

    public async Task<IReadOnlyList<BarbershopServiceDto>> GetServicesAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.CommerceUuid))
        {
            throw new InvalidOperationException("Yeasy:CommerceUuid no está configurado.");
        }

        var endpoint = $"services/?commerce={Uri.EscapeDataString(_options.CommerceUuid)}";
        using var response = await _httpClient.GetAsync(endpoint, cancellationToken);
        var payload = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Error consultando servicios de Yeasy ({(int)response.StatusCode}): {payload}");
        }

        var rawServices = JsonSerializer.Deserialize<List<YeasyServiceItemDto>>(payload, JsonOptions) ?? [];

        return rawServices
            .Where(x => !string.IsNullOrWhiteSpace(x.Uuid) && !string.IsNullOrWhiteSpace(x.Name))
            .Select(x => new BarbershopServiceDto(
                x.Uuid,
                x.Name,
                x.Price,
                x.Decimal,
                x.DefaultDuration))
            .ToList();
    }

    public async Task<BarbershopAvailabilityDto> GetAvailabilityAsync(BarbershopAvailabilityQueryDto query, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.CommerceUuid))
        {
            throw new InvalidOperationException("Yeasy:CommerceUuid no está configurado.");
        }

        if (string.IsNullOrWhiteSpace(query.ServiceUuid))
        {
            throw new InvalidOperationException("El identificador del servicio es obligatorio.");
        }

        var requestBody = new YeasyAvailabilityRequestDto(
            _options.CommerceUuid,
            query.Date,
            query.ServicesDuration,
            [new YeasyServiceReferenceDto(query.ServiceUuid)],
            query.UserTimezone);

        var requestJson = JsonSerializer.Serialize(requestBody, JsonOptions);
        using var content = new StringContent(requestJson, Encoding.UTF8, "application/json");
        using var response = await _httpClient.PostAsync("availability/", content, cancellationToken);
        var payload = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Error consultando disponibilidad de Yeasy ({(int)response.StatusCode}): {payload}");
        }

        using var document = JsonDocument.Parse(payload);
        return ParseAvailability(document.RootElement);
    }

    private static BarbershopAvailabilityDto ParseAvailability(JsonElement root)
    {
        if (root.ValueKind != JsonValueKind.Object)
        {
            return new BarbershopAvailabilityDto([], []);
        }

        if (!root.TryGetProperty("availability", out var availabilityElement) || availabilityElement.ValueKind != JsonValueKind.Object)
        {
            return new BarbershopAvailabilityDto([], []);
        }

        var morning = ParseAvailabilityPeriod(availabilityElement, "morning");
        var afternoon = ParseAvailabilityPeriod(availabilityElement, "afternoon");

        return new BarbershopAvailabilityDto(morning, afternoon);
    }

    private static IReadOnlyList<BarbershopAvailabilitySlotDto> ParseAvailabilityPeriod(JsonElement availabilityElement, string periodName)
    {
        if (!availabilityElement.TryGetProperty(periodName, out var periodElement) || periodElement.ValueKind != JsonValueKind.Array)
        {
            return [];
        }

        return periodElement
            .EnumerateArray()
            .Select(ParseSlot)
            .Where(x => !string.IsNullOrWhiteSpace(x.Label))
            .ToList();
    }

    private static BarbershopAvailabilitySlotDto ParseSlot(JsonElement slotElement)
    {
        var date = GetString(slotElement, "date");
        var label = GetString(slotElement, "label");
        var employees = new List<BarbershopEmployeeAvailabilityDto>();

        if (slotElement.TryGetProperty("employee", out var employeeElement) && employeeElement.ValueKind == JsonValueKind.Array)
        {
            employees.AddRange(employeeElement
                .EnumerateArray()
                .Select(ParseEmployee)
                .Where(x => !string.IsNullOrWhiteSpace(x.Uuid) || !string.IsNullOrWhiteSpace(x.Name)));
        }

        return new BarbershopAvailabilitySlotDto(date, label, employees);
    }

    private static BarbershopEmployeeAvailabilityDto ParseEmployee(JsonElement employeeElement)
    {
        return new BarbershopEmployeeAvailabilityDto(
            GetString(employeeElement, "uuid"),
            GetString(employeeElement, "name"),
            GetString(employeeElement, "surname"));
    }

    private static string GetString(JsonElement element, string propertyName)
    {
        if (element.TryGetProperty(propertyName, out var property) && property.ValueKind == JsonValueKind.String)
        {
            return property.GetString() ?? string.Empty;
        }

        return string.Empty;
    }
}