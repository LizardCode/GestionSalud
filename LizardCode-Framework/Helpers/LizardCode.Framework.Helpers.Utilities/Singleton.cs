using System;

namespace LizardCode.Framework.Helpers.Utilities
{
    public abstract class Singleton<T> where T : class, new()
    {
        protected static readonly Lazy<T> lazy = new Lazy<T>(() => new T());

        public static T Instance => lazy.Value;
    }
}
