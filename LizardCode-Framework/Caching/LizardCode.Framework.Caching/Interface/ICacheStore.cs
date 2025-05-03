using System;

namespace LizardCode.Framework.Caching.Interface
{
	public interface ICacheStore
	{
		void Add<TItem>(TItem item, ICacheKey<TItem> key, String cachedObjectName);
		void Add<TItem>(TItem item, ICacheKey<TItem> key, TimeSpan? expirationTime = null);

		void Add<TItem>(TItem item, ICacheKey<TItem> key, DateTime? absoluteExpiration = null);

		TItem Get<TItem>(ICacheKey<TItem> key) where TItem : class;

		void Remove<TItem>(ICacheKey<TItem> key);
	}
}