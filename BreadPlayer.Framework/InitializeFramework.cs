using BreadPlayer.Core.Interfaces;

public class InitializeFramework
{
    private static IDispatcher dispatcher;
    public static IDispatcher Dispatcher
    {
        get => dispatcher;
        set => dispatcher = value;
    }
    public InitializeFramework(IDispatcher dispatcher)
    {
        Dispatcher = dispatcher;
    }
}
