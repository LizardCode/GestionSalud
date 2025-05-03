namespace LizardCode.Framework.Caching.Interface
{
	public interface ICacheKey<TItem>
	{
		string CacheKey { get; }
	}
}