using System.ComponentModel.DataAnnotations;

namespace Bot.Api.Dto.Yeasy;

public sealed class AvailabilityRequestBodyDto
{
    [Required(AllowEmptyStrings = false, ErrorMessage = "El parámetro 'serviceUuid' es obligatorio.")]
    public string ServiceUuid { get; init; } = string.Empty;

    [Required(AllowEmptyStrings = false, ErrorMessage = "El parámetro 'date' es obligatorio. Formato: YYYY-MM-DD")]
    public string Date { get; init; } = string.Empty;

    public string? UserTimezone { get; init; } = "Europe/Madrid";
}
