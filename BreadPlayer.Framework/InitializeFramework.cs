using BreadPlayer.Core.Interfaces;

public class InitializeFramework
{
    static IDispatcher dispatcher;
    public static IDispatcher Dispatcher
    {
        get { return dispatcher; }
        set { dispatcher = value; }
    }
    public InitializeFramework(IDispatcher dispatcher)
    {
        Dispatcher = dispatcher;
    }
}
