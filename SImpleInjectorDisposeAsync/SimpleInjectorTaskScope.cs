using SimpleInjector;
using SimpleInjector.Lifestyles;

public class SimpleInjectorTaskScope : ITaskScope
{
    private readonly Scope _scope;
    private bool disposedValue;

    public SimpleInjectorTaskScope(Scope scope)
    {
        this._scope = scope;
    }

    public async ValueTask DisposeAsync()
    {
        await this._scope.DisposeAsync();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~SimlpeInjectorTaskScope()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public TService GetService<TService>() where TService : class
    {
        return _scope.GetInstance<TService>();
    }
}

public class SimpleInjectorScopeProvider : IScopeProvider
{
    private readonly Container _container;

    public SimpleInjectorScopeProvider(Container container)
    {
        _container = container;
    }

    public ITaskScope CreateScope()
    {
        var scope = AsyncScopedLifestyle.BeginScope(_container);
        return new SimpleInjectorTaskScope(scope);
    }
}


