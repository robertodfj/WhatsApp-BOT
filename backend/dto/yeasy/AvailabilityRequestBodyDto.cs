using System.ComponentModel.DataAnnotations;

namespace Bot.Api.Dto.Yeasy;

public sealed record AvailabilityRequestBodyDto(
    [property: Required(AllowEmptyStrings = false, ErrorMessage = "El parámetro 'serviceUuid' es obligatorio.")]
    string ServiceUuid,
    [property: Required(AllowEmptyStrings = false, ErrorMessage = "El parámetro 'date' es obligatorio. Formato: YYYY-MM-DD")]
    string Date,
    string? UserTimezone = "Europe/Madrid");
