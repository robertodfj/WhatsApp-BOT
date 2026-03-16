using Bot.Api.Service.Yeasy;
using Bot.Api.Dto.Yeasy;
using Microsoft.AspNetCore.Mvc;

namespace Bot.Api.Controller.Yeasy;

[ApiController]
[Route("api/[controller]")]
public class YeasyController : ControllerBase
{
    private readonly IYeasyService _yeasyService;

    public YeasyController(IYeasyService yeasyService)
    {
        _yeasyService = yeasyService;
    }

    [HttpGet("services")]
    public async Task<IActionResult> GetServices(CancellationToken cancellationToken)
    {
        var services = await _yeasyService.GetServicesAsync(cancellationToken);
        return Ok(services);
    }

    [HttpPost("availability")]
    public async Task<IActionResult> GetAvailability(
        [FromBody] AvailabilityRequestBodyDto request,
        CancellationToken cancellationToken = default)
    {
        var availability = await _yeasyService.GetAvailabilityResponseAsync(request, cancellationToken);
        return Ok(availability);
    }
}