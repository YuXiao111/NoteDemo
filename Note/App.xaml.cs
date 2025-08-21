using Note.Views;
using System.Configuration;
using System.Data;
using System.Windows;
using MessageBox = HandyControl.Controls.MessageBox;

namespace Note
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            if (Licence.Helper.IsLicenseExpired())
            {
                MessageBox.Show("许可已到期!", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                Application.Current.Shutdown();
            }
            else
            {
                new MainView().Show();
            }
        }
    }

}
