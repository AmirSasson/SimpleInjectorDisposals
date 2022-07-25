using SimpleInjector;
using SimpleInjector.Lifestyles;

await using var container = new Container();
container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
container.Register<ScopedServiceNotThreadSafe>(Lifestyle.Scoped);
container.RegisterSingleton<SingletoneProcessingEngine>();
container.RegisterSingleton<IScopeProvider, SimpleInjectorScopeProvider>();

await container.GetInstance<SingletoneProcessingEngine>().RunEngine();

class ScopedServiceNotThreadSafe : IAsyncDisposable
{
    private int? cache = null;
    public async Task<int> RunTask(int i)
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
    private const int NUMBER_OF_TASKS = 5;

    public SingletoneProcessingEngine(IScopeProvider sp)
    {
        this.sp = sp;
    }

    async public Task RunEngine()
    {
        for (int i = 0; i < NUMBER_OF_TASKS; i++)
        {
            await HandleTask(i);
        }
    }

    async private Task HandleTask(int task)
    {
        await using var scope = sp.CreateScope();
        var scopedSerice = scope.GetService<ScopedServiceNotThreadSafe>();
        var res = await scopedSerice.RunTask(DateTime.Now.Second);
        Console.WriteLine(res);
    }

    public async ValueTask DisposeAsync()
    {
        Console.WriteLine("Disposing SigletoneDependency..");
        await Task.Delay(500);
        Console.WriteLine("Disposed SigletoneDependency");
    }
}


