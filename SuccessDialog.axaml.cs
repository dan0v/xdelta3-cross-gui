using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.Diagnostics;
using System.IO;

namespace xdelta3_cross_gui
{
    public class SuccessDialog : Window
    {
        private MainWindow MainParent;

        Button btn_Dismiss;
        Button btn_OpenDestination;

        private string _Destination = "";
        public SuccessDialog()
        {
            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        public SuccessDialog(MainWindow MainParent)
        {
            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            this.MainParent = MainParent;
            this.Configure();
        }
        private void Configure()
        {
            if (this.MainParent.Options.ZipFilesWhenDone)
            {
                this._Destination = Path.Combine(this.MainParent.Options.PatchFileDestination, "..");
            }
            else
            {
                this._Destination = this.MainParent.Options.PatchFileDestination;
            }
            this.btn_Dismiss = this.FindControl<Button>("btn_Dismiss");
            this.btn_OpenDestination = this.FindControl<Button>("btn_OpenDestination");

            this.btn_Dismiss.Click += DismissClicked;
            this.btn_OpenDestination.Click += OpenDestinationClicked;
        }

        private void DismissClicked(object sender, RoutedEventArgs args)
        {
            this.Close();
        }
        private void OpenDestinationClicked(object sender, RoutedEventArgs args)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = this._Destination + Path.DirectorySeparatorChar,
                    UseShellExecute = true
                });
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
