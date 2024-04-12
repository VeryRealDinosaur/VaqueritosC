using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Vaqueritos;

public partial class MainMenu : Window
{
    public MainMenu()
    {
        InitializeComponent();
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        var secondWindow = new Game();
        secondWindow.Show();
    }
}