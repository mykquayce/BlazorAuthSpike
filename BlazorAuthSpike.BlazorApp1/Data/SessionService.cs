using Blazored.LocalStorage;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text.Json;

namespace BlazorAuthSpike.BlazorApp1.Data;

public class SessionService : ISessionService
{
	private readonly IDistributedCache _serverStorage;
	private readonly ILocalStorageService _browserStorage;

	public SessionService(
		IDistributedCache serverStorage,
		ILocalStorageService browserStorage)
	{
		_serverStorage = serverStorage;
		_browserStorage = browserStorage;
	}

	public async Task<Guid> GetSessionIdAsync(CancellationToken cancellationToken = default)
	{
		var sessionId = await _browserStorage.GetItemAsync<Guid?>("sessionid", cancellationToken);
		if (sessionId == null)
		{
			sessionId = new Guid(RandomNumberGenerator.GetBytes(count: 16));
			await _browserStorage.SetItemAsync("sessionid", sessionId, cancellationToken);
		}
		return sessionId!.Value;
	}

	public async Task<T?> GetStoredValueAsync<T>(CancellationToken cancellationToken = default)
	{
		var sessionId = await GetSessionIdAsync(cancellationToken);
		var o = await GetStoredValueAsync<T>(sessionId, cancellationToken);
		return o;
	}

	public async Task<T?> GetStoredValueAsync<T>(Guid sessionId, CancellationToken cancellationToken = default)
	{
		var bytes = await _serverStorage.GetAsync(sessionId.ToString("n"), cancellationToken);
		if (bytes == null) return default;

		await using var stream = new MemoryStream(bytes!);
		var o = await JsonSerializer.DeserializeAsync<T>(stream, cancellationToken: cancellationToken);
		return o!;
	}

	public async Task SetStoredValueAsync<T>(T value, CancellationToken cancellationToken = default)
	{
		var sessionId = await GetSessionIdAsync(cancellationToken);
		await SetStoredValueAsync(sessionId, value, cancellationToken);
	}

	public async Task SetStoredValueAsync<T>(Guid sessionId, T value, CancellationToken cancellationToken = default)
	{
		await using var stream = new MemoryStream();
		await JsonSerializer.SerializeAsync(stream, value, cancellationToken: cancellationToken);
		stream.Position = 0;
		var bytes = stream.ToArray();
		var options = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1), };
		await _serverStorage.SetAsync(sessionId.ToString("n"), bytes, options, cancellationToken);
	}
}
