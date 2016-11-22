namespace ViewModels
{
    public interface IView
    {
        void ViewModelClosingHandler(bool? dialogResult);
        void ViewModelActivatingHandler();
        object DataContext{get;set;}
    }
}