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
        }

        public SuccessDialog(MainWindow MainParent)
        {
            this.InitializeComponent();
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
