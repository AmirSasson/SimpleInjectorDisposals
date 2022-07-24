public interface IScopeProvider
{
    ITaskScope CreateScope();
}

public interface ITaskScope : IAsyncDisposable, IDisposable
{
    TService GetService<TService>() where TService : class;
}




