using BlazorAuthSpike.BlazorApp1.Data;
using Blazored.LocalStorage;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using System.Security.Claims;
using System.Text.Json;

namespace BlazorAuthSpike.BlazorApp1.Tests;

public class SessionServiceTests
{
	private readonly ISessionService _sut;
	private readonly IDictionary<string, Guid> _browser = new Dictionary<string, Guid>();
	private readonly IDictionary<string, byte[]> _server = new Dictionary<string, byte[]>();

	public SessionServiceTests()
	{
		var serverStorageMock = new Mock<IDistributedCache>();
		serverStorageMock
			.Setup(c => c.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()))
			.Callback<string, byte[], DistributedCacheEntryOptions, CancellationToken>((key, bytes, _, _) => _server.Add(key, bytes));
		serverStorageMock
			.Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync((string key, CancellationToken _) => _server[key]);

		var browserStorageMock = new Mock<ILocalStorageService>();
		browserStorageMock
			.Setup(s => s.GetItemAsync<Guid?>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync((string key, CancellationToken _) => _browser.TryGetValue(key, out var value) ? value : null);
		browserStorageMock
			.Setup(s => s.SetItemAsync(It.IsAny<string>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
			.Callback<string, Guid?, CancellationToken>((key, sessionId, _) => _browser.Add(key, sessionId!.Value));

		_sut = new SessionService(serverStorageMock.Object, browserStorageMock.Object);
	}

	[Fact]
	public async Task GetSessionIdTests() => Assert.NotEqual(default, await _sut.GetSessionIdAsync());

	[Theory]
	[InlineData("key", "value")]
	[InlineData("key1", "value1", "key2", "value2")]
	public async Task SetStoredValueTests(params string[] kvps)
	{
		// Arrange
		var before = new Claims();
		foreach (var kvp in kvps.Chunk(size: 2)) { before.Add(kvp[0], kvp[1]); }

		// Act
		await _sut.SetStoredValueAsync(before);
		var after = await _sut.GetStoredValueAsync<Claims>();

		// Assert
		Assert.NotNull(after);
		Assert.NotEmpty(after);
		Assert.All(before.Keys, s => Assert.Contains(s, after.Keys));
		Assert.All(before.Keys, s => Assert.Equal(before[s], after[s]));
	}
}

public class DistributedCacheEntryOptionsTests
{
	[Theory]
	[InlineData(1, 0, 0)]
	public void SerializationTests(int hours, int minutes, int seconds)
	{
		var before = new DistributedCacheEntryOptions
		{
			AbsoluteExpirationRelativeToNow = new TimeSpan(hours: hours, minutes: minutes, seconds: seconds),
		};
		var json = JsonSerializer.Serialize(before);
		Assert.NotNull(json);
		Assert.NotEmpty(json);
		Assert.NotEqual("{}", json);
	}

	[Theory]
	[InlineData("""
		{"AbsoluteExpiration":null,"AbsoluteExpirationRelativeToNow":"01:00:00","SlidingExpiration":null}
		""")]
	public void DeserializationTests(string json)
	{
		var o = JsonSerializer.Deserialize<DistributedCacheEntryOptions>(json);
		Assert.NotNull(o);
		Assert.NotEqual(default, o.AbsoluteExpirationRelativeToNow);
	}
}