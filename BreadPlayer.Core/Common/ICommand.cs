namespace BreadPlayer.Core.Common
{
    public interface ICommand : System.Windows.Input.ICommand
    {
        bool IsEnabled { get; set; }
    }
}