using System.Security.Claims;

namespace BlazorAuthSpike.BlazorApp1.Data;

public class UserService : IUserService
{
	private readonly ISessionService _sessionService;

	public UserService(ISessionService sessionService)
	{
		_sessionService = sessionService;
	}

	public async Task<ClaimsIdentity?> GetUserAsync(CancellationToken cancellationToken = default)
	{
		var claims = await _sessionService.GetStoredValueAsync<Claim[]>(cancellationToken);
		if (claims?.Any() ?? false)
		{
			var identity = new ClaimsIdentity(claims, authenticationType: "auth");
			return identity;
		}
		return default;
	}

	public Task LoginAsync(string name, CancellationToken cancellationToken = default)
	{
		var claims = new[]
		{
			new Claim(ClaimTypes.Name, name),
			//{ ClaimTypes.Name, name },
		};
		return _sessionService.SetStoredValueAsync(claims, cancellationToken);
	}

	public Task LogoutAsync(CancellationToken cancellationToken)
		=> _sessionService.SetStoredValueAsync(default, cancellationToken);
}
