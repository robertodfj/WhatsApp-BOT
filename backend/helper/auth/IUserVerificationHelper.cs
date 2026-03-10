using Bot.Api.Dto.Auth;

namespace Bot.Api.Helper.Auth;

public interface IUserVerificationHelper
{
    Task<IncomingMessageResponseDto> ProcessAsync(IncomingMessageRequestDto request, CancellationToken cancellationToken = default);
}
