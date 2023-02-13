using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using DuoClient = DuoUniversal.Client;

namespace BlazorAuthSpike.BlazorApp1.Data;

public class DuoAuthenticationStateProvider : AuthenticationStateProvider
{
	private ClaimsPrincipal _currentUser = new(new ClaimsIdentity());
	private readonly DuoClient _duoClient;

	public DuoAuthenticationStateProvider(DuoClient duoClient)
	{
		_duoClient = duoClient;
	}

	public override Task<AuthenticationState> GetAuthenticationStateAsync()
	{
		return Task.FromResult(new AuthenticationState(_currentUser));
	}

	public Task<string> CreateUserAsync(string username)
	{
		var state = DuoClient.GenerateState();
		var claims = new Claims
		{
			{ ClaimTypes.Sid, state },
			{ ClaimTypes.Name, username },
		};
		var identity = new ClaimsIdentity(claims, authenticationType: "Duo");
		var user = new ClaimsPrincipal(identity);
		var authenticationState = new AuthenticationState(user);
		base.NotifyAuthenticationStateChanged(Task.FromResult(authenticationState));
		return Task.FromResult(state);
	}

	public Task LoginAsync(string duoCode)
	{
		var task = f();
		base.NotifyAuthenticationStateChanged(task);
		return task;

		async Task<AuthenticationState> f()
		{
			var username = _currentUser.Claims.FirstOrDefault(c => string.Equals(c.Type, ClaimTypes.Name, StringComparison.OrdinalIgnoreCase))?.Value
				?? throw new KeyNotFoundException(string.Join(',', _currentUser.Claims.Select(c => c.Type)));
			_currentUser = await LoginWithExternalProviderAsync(duoCode, username);
			var newAuthenticationState = new AuthenticationState(_currentUser);
			return newAuthenticationState;
		}
	}

	private async Task<ClaimsPrincipal> LoginWithExternalProviderAsync(string code, string username)
	{
		var idToken = await _duoClient.ExchangeAuthorizationCodeFor2faResult(code, username);
		var claims = new Claims
		{
			{ ClaimTypes.Name, idToken.Sub },
			{ ClaimTypes.GroupSid, idToken.Aud },
			{ ClaimTypes.Expiration, idToken.Exp.ToString("O") },
		};
		var identity = new ClaimsIdentity(claims, authenticationType: "Duo");
		_currentUser = new ClaimsPrincipal(identity);
		return _currentUser;
	}
}
