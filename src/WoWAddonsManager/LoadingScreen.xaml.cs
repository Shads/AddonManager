using System.Windows;

namespace WoWAddonsManager
{
    /// <summary>
    /// Interaction logic for LoadingScreen.xaml
    /// </summary>
    public partial class LoadingScreen : Window
    {
        public LoadingScreen()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        public LoadingScreen(int updateLength)
            : this()
        {
            pbUpdate.Maximum = updateLength;
        }

        public void Increment()
        {
            Dispatcher.Invoke(() =>
            {
                pbUpdate.Value += 1;
            });
        }
    }
}
