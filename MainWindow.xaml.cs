using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Portal;
using Esri.ArcGISRuntime.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QuartzPortalExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        /// <summary>
        /// The URL for your Portal's REST services.
        /// </summary>
        string portalUrl = "https://myagsportal.com/arcgis/sharing/rest";
        /// <summary>
        /// The Portal user.
        /// </summary>
        string portalUser = "user";
        /// <summary>
        /// The password for the Portal user.  (Needless to say, this is for
        /// demonstration purposes only.  Don't store your passwords like this.)
        /// </summary>
        string portalPass = "password";
        /// <summary>
        /// The ID of the web map we're going to load.
        /// </summary>
        string webMapItemId = "web_map_guid";

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Loaded event of the MapView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void MapView_Loaded(object sender, RoutedEventArgs e)
        {
            // Create the set of credentials we require to access the Portal
            // site.
            var creds = await AuthenticationManager.Current.GenerateCredentialAsync(
                    new Uri(this.portalUrl),
                    this.portalUser,
                    this.portalPass);
            // These credentials need to be added to the authentication manager
            // that created them.
            AuthenticationManager.Current.AddCredential(creds);
            // Create the portal instance.
            var agsPortal = await ArcGISPortal.CreateAsync(
                new Uri(this.portalUrl));
            // Let Portal know who's asking.
            var agsPortalUser = agsPortal.CurrentUser;

            // Get the web map content item by its ID.
            var webmapItem =
                await ArcGISPortalItem.CreateAsync(
                    agsPortal,
                    this.webMapItemId);
            // Create an ESRI Map based on the web map item.
            var myMap = new Map(webmapItem);
            // Load it up.
            await myMap.LoadAsync();
            // If the map loaded successfully...
            if (myMap.LoadStatus != Esri.ArcGISRuntime.LoadStatus.FailedToLoad)
            {
                // ...add it to the map view.
                this.MapView.Map = myMap;
            }
            else
            {
                MessageBox.Show("The Portal map failed to load.");
            }

            // Now, let's inspect the operational layers.  In our data, these
            // have popups.  We're trying to figure out why we don't see them
            // when we click on features.
            for (int i = 0; i < myMap.OperationalLayers.Count; i++)
            {
                // We want to inspect the feature layers.
                FeatureLayer operationalLayer = myMap.OperationalLayers[i] as FeatureLayer;

                FeatureTable ft = operationalLayer.FeatureTable;
                

                if (operationalLayer == null) continue;
                // In our map, the layers the popups are enabled, and popups
                // are defined.
                Console.WriteLine("[{0}]: {1}", i, operationalLayer.Name);
                Console.WriteLine("   IsPopupEnabled={0}", operationalLayer.IsPopupEnabled); // true
                Console.WriteLine("   PopupDefinition != null: {0}", operationalLayer.PopupDefinition != null);
                // ...but we don't get see popups when we click the features. ???
            }
        }
    }
}
