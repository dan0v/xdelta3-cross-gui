using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System.Collections.Generic;

namespace xdelta3_cross_gui
{
    public class ErrorDialog : Window
    {
        List<string> missingOldFiles;
        List<string> missingNewFiles;
        Button btn_Dismiss;
        TextBlock txt_blk_MissingOld;
        TextBlock txt_blk_MissingNew;
        Grid grd_MissingFiles;

        public ErrorDialog()
        {
            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        public ErrorDialog(List<string> missingOldFiles, List<string> missingNewFiles)
        {
            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            this.missingNewFiles = missingNewFiles;
            this.missingOldFiles = missingOldFiles;
            this.Configure();
        }

        private void Configure()
        {
            this.btn_Dismiss = this.FindControl<Button>("btn_Dismiss");

            this.txt_blk_MissingOld = this.FindControl<TextBlock>("txt_blk_MissingOld");
            this.txt_blk_MissingNew = this.FindControl<TextBlock>("txt_blk_MissingNew");
            this.grd_MissingFiles = this.FindControl<Grid>("grd_MissingFiles");

            this.btn_Dismiss.Click += DismissClicked;

            if (this.missingOldFiles.Count > 0)
            {
                this.txt_blk_MissingOld.Text = string.Join("\n", missingOldFiles);
                this.grd_MissingFiles.IsVisible = true;
            }
            if (this.missingNewFiles.Count > 0)
            {
                this.txt_blk_MissingNew.Text = string.Join("\n", missingNewFiles);
                this.grd_MissingFiles.IsVisible = true;
            }
        }

        private void DismissClicked(object sender, RoutedEventArgs args)
        {
            this.Close();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
