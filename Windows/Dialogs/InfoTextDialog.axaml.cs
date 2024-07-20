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
            InitializeComponent();
            Configure();
        }

        public InfoTextDialog(InfoTextType infoType)
        {
            InitializeComponent();
            Configure();

            try
            {
                string path = "";
                switch (infoType)
                {
                    case InfoTextType.LICENSE:
                        path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, MainWindow.LICENSE_FILE);
                        txt_blk_Info.TextWrapping = Avalonia.Media.TextWrapping.NoWrap;
                        txt_blk_Info.TextTrimming = Avalonia.Media.TextTrimming.WordEllipsis;
                        Title = "License";
                        break;
                    case InfoTextType.NOTICE:
                        path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, MainWindow.NOTICE_FILE);
                        txt_blk_Info.TextWrapping = Avalonia.Media.TextWrapping.Wrap;
                        Title = "Notice";
                        break;
                }
                string content = File.ReadAllText(path);
                txt_blk_Info.Text = content;
            }
            catch { }
        }

        private void DismissClicked(object? sender, RoutedEventArgs args)
        {
            Close();
        }

        private void Configure()
        {
            btn_Dismiss.Click += DismissClicked;
        }
    }
}
