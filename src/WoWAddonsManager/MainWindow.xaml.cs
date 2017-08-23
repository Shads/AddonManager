using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WoWAddonsManager.Config;
using WoWAddonsManager.Sources;

namespace WoWAddonsManager
{
public partial class MainWindow : Window
    {
        private string path = string.Empty;
        private string archivePath = string.Empty;
        private string extractPath = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
            ReadConfig();
            btnSetPath.IsEnabled = false;
            btnUpdateSelected.IsEnabled = false;
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
            if (!Directory.Exists(txtWowLocation.Text))
            {
                MessageBox.Show("Addons Folder not found.");
                txtWowLocation.Text = "";
            }
            var addons = ConfigManager.GetConfig();
            path = txtWowLocation.Text;
            addons.FolderPath = path;
            ConfigManager.SetConfig(addons);
        }

        private async void btnCheckCurse_Click(object sender, RoutedEventArgs e)
        {
            var addons = ConfigManager.GetConfig();
            if (addons == null || addons.Items.Count == 0)
            {
                return;
            }

            var loadingScreen = new LoadingScreen(addons.Items.Count) { Owner = this };
            loadingScreen.Show();
            var source = new CurseAddonSource();

            try
            {
                await source.RefreshConfig(addons, () => loadingScreen.Increment());
            }
            catch (Exception ex)
            {
                loadingScreen.Close();
                MessageBox.Show(ex.Message, "Broken", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            dgAddons.ItemsSource = addons.Items;
            dgAddons.Items.Refresh();

            ConfigManager.SetConfig(addons);

            loadingScreen.Close();
            MessageBox.Show("Update complete", "Finished", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnAddItem_Click(object sender, RoutedEventArgs e)
        {
            var frmAddNew = new AddNewAddon();
            frmAddNew.Closing += FrmAddNew_Closing;
            frmAddNew.Show();
        }

        private void FrmAddNew_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var frm = (AddNewAddon)sender;
            if (frm.newItem != null)
            {
                ReadConfig();
            }
        }

        private void btnSetPath_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog { IsFolderPicker = true };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok && Directory.Exists(dialog.FileName))
            {
                var item = (AddonConfigItem)dgAddons.SelectedItem;
                item.Path = dialog.FileName;
                ReadConfig();
            }
        }

        private void dgAddons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnSetPath.IsEnabled = dgAddons.SelectedItem != null;
            btnUpdateSelected.IsEnabled = dgAddons.SelectedItem != null;
        }

        private async void btnUpdateSelected_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            EnsureArchive();

            var item = dgAddons.SelectedItem as AddonConfigItem;
            if (item != null)
            {
                await UpdateItem(item, null);
                MessageBox.Show($"{item.Name} Updated!", "Update Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            MessageBox.Show($"Nothing selected...", "You suck", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private async void btnUpdateAll_Click(object sender, RoutedEventArgs e)
        {
            EnsureArchive();
            var loadingScreen = new LoadingScreen(dgAddons.Items.Count) { Owner = this };
            loadingScreen.Show();

            var addons = ConfigManager.GetConfig();
            var updates = new List<Task>();
            foreach (var item in addons.Items)
            {
                if (item != null)
                {
                    updates.Add(UpdateItem(item, () => loadingScreen.Increment()));
                    item.Version = item.SiteVersion;
                }
            }

            dgAddons.ItemsSource = addons.Items;
            dgAddons.Items.Refresh();

            await Task.WhenAll(updates);

            loadingScreen.Close();
        }

        private void CopyFiles(string source, string dest)
        {
            foreach (string dirPath in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(source, dest));

            foreach (string newPath in Directory.GetFiles(source, "*.*", SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(source, dest), true);

            Directory.Delete(extractPath + "\\", true);
        }

        private void EnsureArchive()
        {
            archivePath = Directory.Exists("archive")
                ? Path.Combine(Directory.GetCurrentDirectory(), "archive")
                : Directory.CreateDirectory("archive").FullName;

            extractPath = Directory.Exists("archive\\extract")
                ? Path.Combine(Directory.GetCurrentDirectory(), "archive\\extract")
                : Directory.CreateDirectory("archive\\extract").FullName;
        }

        private void ReadConfig()
        {
            var addons = ConfigManager.GetConfig();
            if (addons != null && addons.Items.Count > 0)
            {
                txtWowLocation.Text = addons.FolderPath;
                dgAddons.ItemsSource = addons.Items;
                dgAddons.Items.Refresh();
            }

            dgAddons.SelectedItem = null;
        }

        private async Task UpdateItem(AddonConfigItem item, Action done)
        {
            var source = new CurseAddonSource();
            var zipPath = await source.GetZipFile(item);

            if (!string.IsNullOrWhiteSpace(zipPath))
            {
                ZipFile.ExtractToDirectory(zipPath, extractPath);
                CopyFiles(extractPath, path);
            }

            done?.Invoke();
        }
    }
}
