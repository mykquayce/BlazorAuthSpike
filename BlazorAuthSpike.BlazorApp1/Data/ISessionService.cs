namespace BlazorAuthSpike.BlazorApp1.Data
{
	public interface ISessionService
	{
		Task<Guid> GetSessionIdAsync(CancellationToken cancellationToken = default);
		Task<T?> GetStoredValueAsync<T>(CancellationToken cancellationToken = default);
		Task<T?> GetStoredValueAsync<T>(Guid sessionId, CancellationToken cancellationToken = default);
		Task SetStoredValueAsync<T>(Guid sessionId, T value, CancellationToken cancellationToken = default);
		Task SetStoredValueAsync<T>(T value, CancellationToken cancellationToken = default);
	}
}