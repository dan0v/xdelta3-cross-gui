/*Copyright 2020-2023 dan0v

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
using System.IO;

namespace xdelta3_cross_gui
{
    public partial class InfoTextDialog : Window
    {
        public enum InfoTextType
        {
            LICENSE,
            NOTICE
        }

        public InfoTextDialog()
        {
            this.InitializeComponent();
            this.Configure();
        }

        public InfoTextDialog(InfoTextType infoType)
        {
            this.InitializeComponent();
            this.Configure();

            try
            {
                string path = "";
                switch (infoType)
                {
                    case InfoTextType.LICENSE:
                        path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, MainWindow.LICENSE_FILE);
                        this.txt_blk_Info.TextWrapping = Avalonia.Media.TextWrapping.NoWrap;
                        this.txt_blk_Info.TextTrimming = Avalonia.Media.TextTrimming.WordEllipsis;
                        this.Title = "License";
                        break;
                    case InfoTextType.NOTICE:
                        path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, MainWindow.NOTICE_FILE);
                        this.txt_blk_Info.TextWrapping = Avalonia.Media.TextWrapping.Wrap;
                        this.Title = "Notice";
                        break;
                }
                string content = File.ReadAllText(path);
                this.txt_blk_Info.Text = content;
            }
            catch { }
        }

        private void DismissClicked(object? sender, RoutedEventArgs args)
        {
            this.Close();
        }

        private void Configure()
        {
            this.btn_Dismiss.Click += DismissClicked;
        }
    }
}
