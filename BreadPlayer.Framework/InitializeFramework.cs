using BreadPlayer.Core.Interfaces;

public class InitializeFramework
{
    private static IDispatcher _dispatcher;
    public static IDispatcher Dispatcher
    {
        get => _dispatcher;
        set => _dispatcher = value;
    }
    public InitializeFramework(IDispatcher dispatcher)
    {
        Dispatcher = dispatcher;
    }
}
