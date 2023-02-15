using System.Security.Claims;
using System.Text.Json;

namespace BlazorAuthSpike.BlazorApp1.Tests;

public class ClaimsSerializationTests
{
	[Theory]
	[InlineData("name", "bob")]
	[InlineData("name", "bob", "email", "mail@bob.codes")]
	public void Test1(params string[] kvps)
	{
		var before = new Claims();

		foreach (var kvp in kvps.Chunk(size: 2))
		{
			before.Add(kvp[0], kvp[1]);
		}

		var after = JsonSerializer.Deserialize<Claims>(JsonSerializer.Serialize(before));
		Assert.NotNull(after);
		Assert.Equal(before, after);
	}
}
