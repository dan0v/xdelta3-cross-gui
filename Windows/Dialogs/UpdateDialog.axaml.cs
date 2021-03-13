/*Copyright 2020-2021 dan0v

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.*/

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.Diagnostics;
using xdelta3_cross_gui.Localization;

namespace xdelta3_cross_gui
{
    public class UpdateDialog : Window
    {
        private MainWindow MainParent;
        private string newVersion = "";

        TextBlock txt_blk_Prompt;
        Button btn_Dismiss;
        Button btn_GoToReleases;

        public UpdateDialog()
        {
            this.InitializeComponent();
        }

        public UpdateDialog(MainWindow MainParent, string newVersion)
        {
            this.InitializeComponent();
            this.MainParent = MainParent;
            this.newVersion = newVersion;
            this.Configure();
        }
        private void Configure()
        {
            this.btn_Dismiss = this.FindControl<Button>("btn_Dismiss");
            this.btn_GoToReleases = this.FindControl<Button>("btn_GoToReleases");
            this.txt_blk_Prompt = this.FindControl<TextBlock>("txt_blk_Prompt");

            this.btn_Dismiss.Click += DismissClicked;
            this.btn_GoToReleases.Click += GoToReleasesClicked;
            this.txt_blk_Prompt.Text = string.Format(Localizer.Instance["NewVersionText"], newVersion);
        }

        private void DismissClicked(object sender, RoutedEventArgs args)
        {
            this.Close();
        }
        private void GoToReleasesClicked(object sender, RoutedEventArgs args)
        {
            try
            {
                ProcessStartInfo url = new ProcessStartInfo
                {
                    FileName = MainWindow.RELEASES_PAGE,
                    UseShellExecute = true
                };
                Process.Start(url);
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
