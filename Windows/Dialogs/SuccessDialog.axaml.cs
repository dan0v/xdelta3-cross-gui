/*Copyright 2020-2024 dan0v

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
    public partial class SuccessDialog : Window
    {
        private MainWindow MainParent;

        private string _Destination = "";

        public SuccessDialog(MainWindow MainParent)
        {
            InitializeComponent();
            this.MainParent = MainParent;
            Configure();
        }
        private void Configure()
        {
            if (MainParent.Config.ZipFilesWhenDone)
            {
                _Destination = Path.Combine(MainParent.Config.PatchFileDestination, "..");
            }
            else
            {
                _Destination = MainParent.Config.PatchFileDestination;
            }

            btn_Dismiss.Click += DismissClicked;
            btn_OpenDestination.Click += OpenDestinationClicked;
        }

        private void DismissClicked(object? sender, RoutedEventArgs args)
        {
            Close();
        }
        private void OpenDestinationClicked(object? sender, RoutedEventArgs args)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = _Destination + Path.DirectorySeparatorChar,
                    UseShellExecute = true
                });
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }
    }
}
