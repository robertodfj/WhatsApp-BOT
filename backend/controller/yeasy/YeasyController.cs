using Bot.Api.Service.Yeasy;
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
}