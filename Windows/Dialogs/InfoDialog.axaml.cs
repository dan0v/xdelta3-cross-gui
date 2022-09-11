/*Copyright 2020-2022 dan0v

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
    public class InfoDialog : Window
    {
        Button btn_Dismiss;
        Button btn_OpenLicense;
        Button btn_OpenNotice;

        public InfoDialog()
        {
            this.InitializeComponent();
            this.Configure();
        }
        private void Configure()
        {
            this.btn_Dismiss = this.FindControl<Button>("btn_Dismiss");
            this.btn_Dismiss.Click += DismissClicked;

            this.btn_OpenLicense = this.FindControl<Button>("btn_OpenLicense");
            this.btn_OpenLicense.Click += OpenLicense;

            this.btn_OpenNotice = this.FindControl<Button>("btn_OpenNotice");
            this.btn_OpenNotice.Click += OpenNotice;
        }

        private void DismissClicked(object sender, RoutedEventArgs args)
        {
            this.Close();
        }

        private void OpenLicense(object sender, RoutedEventArgs args)
        {
            OpenTextFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, MainWindow.LICENSE_FILE));
        }

        private void OpenNotice(object sender, RoutedEventArgs args)
        {
            OpenTextFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, MainWindow.NOTICE_FILE));
        }

        private void OpenTextFile(string path)
        {
            var p = new ProcessStartInfo()
            {
                FileName = path,
                UseShellExecute = true
            };
            Process.Start(p);
        }
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
