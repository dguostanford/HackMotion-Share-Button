using CefSharp;
using CefSharp.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace HackMotion_Share
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static ChromiumWebBrowser WebBrowser { get; set; }
        private bool IsBrowserInitialized { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            string cefCache = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "HackMotion_ShareBtn", "CefSharp", "Cache");
            Debug.WriteLine("CefSharp cache path: {0}", cefCache);
            var settings = new CefSettings()
            {
                CachePath = cefCache
            };
            Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);
            WebBrowser = new ChromiumWebBrowser()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,

                // Custom menu handler to disable right click
                MenuHandler = new CustomMenuHandler()
            };
            Debug.WriteLine("Loading: " + System.IO.Path.GetFullPath(@".\fb_share.html"));
            WebBrowser.Address = "file:///" + System.IO.Path.GetFullPath(@".\fb_share.html");
            WebBrowser.LoadingStateChanged += BrowserLoadingStateChanged;

            WebGrid.Children.Add(WebBrowser);
        }

        private void BrowserLoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if (!e.IsLoading)
            {
                WebBrowser.LoadingStateChanged -= BrowserLoadingStateChanged;
                Debug.WriteLine("Browser loaded");
                IsBrowserInitialized = true;
            }
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Just to make sure WebBrowser is loaded.
            if (IsBrowserInitialized)
            {
                Debug.WriteLine("shareFB executed");

                // Mainly the most important line - calling the 'shareFB' javascript method.
                WebBrowser.ExecuteScriptAsync("shareFB();");
            }
            else
            {
                Debug.WriteLine("WebView not loaded");
            }
        }
    }

    #region Custom Menu Handler - just to disable right click
    public class CustomMenuHandler : IContextMenuHandler
    {
        public void OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model)
        {
            model.Clear();
        }

        public bool OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags)
        {

            return false;
        }

        public void OnContextMenuDismissed(IWebBrowser browserControl, IBrowser browser, IFrame frame)
        {

        }

        public bool RunContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback)
        {
            return false;
        }
    }
    #endregion

}
