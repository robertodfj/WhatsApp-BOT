using Bot.Api.Database;
using Bot.Api.Dto.Auth;
using Bot.Api.Helper.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bot.Api.Controller.Auth;

[ApiController]
[Route("api/[controller]")]
public class MessagesController : ControllerBase
{
    private readonly IUserVerificationHelper _userVerificationHelper;
    private readonly AppDbContext _dbContext;

    public MessagesController(IUserVerificationHelper userVerificationHelper, AppDbContext dbContext)
    {
        _userVerificationHelper = userVerificationHelper;
        _dbContext = dbContext;
    }

    [HttpPost]
    public async Task<IActionResult> ProcessMessage([FromBody] IncomingMessageRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _userVerificationHelper.ProcessAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers(CancellationToken cancellationToken)
    {
        var users = await _dbContext.Users
            .AsNoTracking()
            .Select(user => new
            {
                user.Id,
                user.PhoneNumber,
                user.Name,
                status = user.Status.ToString()
            })
            .ToListAsync(cancellationToken);

        return Ok(users);
    }
}
