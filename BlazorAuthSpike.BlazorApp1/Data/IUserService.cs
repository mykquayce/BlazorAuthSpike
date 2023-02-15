using System.Security.Claims;

namespace BlazorAuthSpike.BlazorApp1.Data
{
	public interface IUserService
	{
		Task<ClaimsIdentity?> GetUserAsync(CancellationToken cancellationToken = default);
		Task LoginAsync(string name, CancellationToken cancellationToken = default);
		Task LogoutAsync(CancellationToken cancellationToken);
	}
}