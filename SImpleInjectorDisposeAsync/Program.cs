using SimpleInjector;
using SimpleInjector.Lifestyles;

await using var container = new Container();
container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
container.Register<ScopedNotThreadSafeService>(Lifestyle.Scoped);
container.RegisterSingleton<SingletoneProcessingEngine>();
container.RegisterSingleton<IScopeProvider, SimpleInjectorScopeProvider>();

for (int i = 0; i < 2; i++)
{
    await container.GetInstance<SingletoneProcessingEngine>().LongTask();
    await Task.Delay(50);
}

class ScopedNotThreadSafeService : IAsyncDisposable
{
    private int? cache = null;
    public async Task<int> Echo(int i)
    {
        if (cache.HasValue)
        {
            return cache.Value;
        }
        await Task.Delay(1500);
        cache = i;
        return i;
    }

    public async ValueTask DisposeAsync()
    {
        Console.WriteLine("Disposing SomeService..");
        await Task.Delay(1000);
        Console.WriteLine("Disposed SomeService");
    }
}

class SingletoneProcessingEngine : IAsyncDisposable
{
    private readonly IScopeProvider sp;

    public SingletoneProcessingEngine(IScopeProvider sp)
    {
        this.sp = sp;
    }

    async public Task LongTask()
    {
        await using var scope = sp.CreateScope();
        var s = scope.GetService<ScopedNotThreadSafeService>();
        Console.WriteLine(await s.Echo(DateTime.Now.Second));

        scope.GetService<ScopedNotThreadSafeService>();
        Console.WriteLine(await s.Echo(DateTime.Now.Second));
    }

    public async ValueTask DisposeAsync()
    {
        Console.WriteLine("Disposing SigletoneDependency..");
        await Task.Delay(500);
        Console.WriteLine("Disposed SigletoneDependency");
    }
}


