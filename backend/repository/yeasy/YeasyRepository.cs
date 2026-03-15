using System.Text.Json;
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
}