using Microsoft.Extensions.DependencyInjection;

namespace LizardCode.Framework.Application.Helpers
{
    //
    //https://thomaslevesque.com/2020/03/18/lazily-resolving-services-to-fix-circular-dependencies-in-net-core
    //
    public class LazyResolver<T> : Lazy<T>
    {
        public LazyResolver(IServiceProvider serviceProvider) : base(serviceProvider.GetRequiredService<T>) { }
    }
}
