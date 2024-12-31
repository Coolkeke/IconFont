using IconFont.Shell.ViewModels;
using LayUI.Wpf.Controls;
using System.Windows;

namespace IconFont.Shell.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : LayWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            if (DataContext is MainWindowViewModel main) {
                main.Loaded();
            }
        }
    }
}
