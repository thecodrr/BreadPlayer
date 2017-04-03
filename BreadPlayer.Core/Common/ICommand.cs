namespace BreadPlayer
{
    public interface ICommand : System.Windows.Input.ICommand
    {
        bool IsEnabled { get; set; }
    }
}
