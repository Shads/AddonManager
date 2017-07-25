using Microsoft.WindowsAPICodePack.Dialogs;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WoWAddonsManager.Models;
using WoWAddonsManager.Sources;

namespace WoWAddonsManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IEnumerable<Addon> addons;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog { IsFolderPicker = true };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                txtWowLocation.Text = dialog.FileName;
            }
        }

        private void txtWowLocation_TextChanged(object sender, TextChangedEventArgs e)
        {
            var addonsPath = Path.Combine(txtWowLocation.Text, "Interface\\AddOns");
            if (Directory.Exists(addonsPath))
            {
                addons = new TocLoader(addonsPath).GetAddons().ToList();
                dgAddons.ItemsSource = addons;
            }
        }

        private async void btnCheckCurse_Click(object sender, RoutedEventArgs e)
        {
            if (addons == null)
            {
                return;
            }

            var source = new CurseAddonSource();
            var requests = addons.Skip(7).Take(1).Select(a => new Task(async () => await source.GetAvailableVersion(a)));
            await Task.WhenAll(requests);
            dgAddons.ItemsSource = addons;
        }
    }
}
