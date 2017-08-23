using System;
using System.Linq;
using System.Windows;
using WoWAddonsManager.Config;
using WoWAddonsManager.Sources;

namespace WoWAddonsManager
{
    public partial class AddNewAddon : Window
    {
        public AddNewAddon()
        {
            InitializeComponent();
            btnAdd.IsEnabled = false;
        }

        internal AddonConfigItem newItem = null;

        private async void btnDeets_Click(object sender, RoutedEventArgs e)
        {
            var strUrl = txtUrl.Text;

            if (string.IsNullOrWhiteSpace(strUrl))
            {
                MessageBox.Show("Add url to first box");
                return;
            }

            var config = ConfigManager.GetConfig();
            if (config.Items.Any(i => i.Url == strUrl))
            {
                MessageBox.Show("Item already added with the same url");
                return;
            }

            try
            {
                var addon = await new CurseAddonSource().GetAddonDetails(strUrl);
                txtName.Text = addon.Name;
                txtVersion.Text = addon.SiteVersion;
                txtSupports.Text = addon.Supports;
                btnAdd.IsEnabled = true;
                newItem = addon;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to retrived addon details from '{txtUrl.Text}'. Check Url and try again. {Environment.NewLine} {ex.Message}", 
                    "You are not prepared", MessageBoxButton.OK, MessageBoxImage.Error);
                btnAdd.IsEnabled = false;
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var config = ConfigManager.GetConfig();
            if (config == null)
            {
                config = new AddonConfig();
            }

            config.Items.Add(newItem);
            ConfigManager.SetConfig(config);
            Close();
        }
    }
}
