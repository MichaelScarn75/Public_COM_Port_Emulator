using System.Configuration;
using System.Configuration.Internal;
using System.Data;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace WeighingScaleEmulator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            IConfigurationRoot config = new ConfigurationBuilder().AddUserSecrets<App>().Build();
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(config["LicenseKey"]);
        }
    }

}
