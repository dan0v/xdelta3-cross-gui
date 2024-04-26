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

namespace xdelta3_cross_gui
{
    public partial class InfoDialog : Window
    {

        public InfoDialog()
        {
            InitializeComponent();
            Configure();
        }
        private void Configure()
        {
            btn_Dismiss.Click += DismissClicked;
            btn_OpenLicense.Click += OpenLicense;
            btn_OpenNotice.Click += OpenNotice;
        }

        private void DismissClicked(object? sender, RoutedEventArgs args)
        {
            Close();
        }

        private void OpenLicense(object? sender, RoutedEventArgs args)
        {
            OpenInfoTextDialog(InfoTextDialog.InfoTextType.LICENSE);
        }

        private void OpenNotice(object? sender, RoutedEventArgs args)
        {
            OpenInfoTextDialog(InfoTextDialog.InfoTextType.NOTICE);
        }

        private void OpenInfoTextDialog(InfoTextDialog.InfoTextType type)
        {
            Window infoTextDialog = new InfoTextDialog(type);
            infoTextDialog.Show();
        }
    }
}
