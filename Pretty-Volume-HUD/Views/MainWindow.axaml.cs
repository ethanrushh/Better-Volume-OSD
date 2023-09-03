using Avalonia.Controls;
using Pretty_Volume_HUD.ViewModels;

namespace Pretty_Volume_HUD.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        DataContext = new MainWindowViewModel(this);
        
        InitializeComponent();
    }
}
