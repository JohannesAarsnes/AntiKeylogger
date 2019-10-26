using AntiKeyloggerUI.Auxiliary;
using AntiKeyloggerUI.Properties;

using System.ComponentModel;
using System.Windows;

namespace AntiKeyloggerUI.View
{
    public partial class MainWindow : Window
    {

       
        public MainWindow()
        {
            InitializeComponent();
            SourceInitialized +=(s,e) => WindowCromeResizeHelper.SourceInitialize(s,e );
            UiDispatcherHelper.Initialize();
            MinimazeButton.Click += (s, e) => WindowState = WindowState.Minimized;
           // MaximixeButton.Click += (s, e) => WindowState=(WindowState == WindowState.Maximized) ? WindowState.Minimized : WindowState.Maximized;
            CloseButton.Click += (s, e) => Close();
            
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Settings.Default.Save();
            base.OnClosing(e);
        }
    }
}
