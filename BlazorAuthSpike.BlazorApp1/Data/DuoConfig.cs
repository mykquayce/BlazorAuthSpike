using Microsoft.Extensions.Options;

namespace BlazorAuthSpike.BlazorApp1.Data;

public record class DuoConfig(string ClientId, string ClientSecret, string ApiHost, string RedirectUri)
	: IOptions<DuoConfig>
{
	public DuoConfig():this(default!, default!, default!, default!) { }

	public DuoConfig Value => this;
}
