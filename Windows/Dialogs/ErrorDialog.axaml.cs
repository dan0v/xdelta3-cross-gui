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
using System.Collections.Generic;

namespace xdelta3_cross_gui
{
    public partial class ErrorDialog : Window
    {
        List<string>? missingOldFiles;
        List<string>? missingNewFiles;
        string? errorString;

        public ErrorDialog()
        {
            InitializeComponent();
        }

        public ErrorDialog(List<string> missingOldFiles, List<string> missingNewFiles)
        {
            InitializeComponent();
            this.missingNewFiles = missingNewFiles;
            this.missingOldFiles = missingOldFiles;
            Configure();
        }

        public ErrorDialog(string xDeltaFailed)
        {
            InitializeComponent();
            errorString = xDeltaFailed;
            Configure();
        }

        private void Configure()
        {
            btn_Dismiss.Click += DismissClicked;

            if ((missingOldFiles?.Count ?? 0) > 0)
            {
                txt_blk_MissingOld.Text = string.Join("\n", missingOldFiles ?? []);
                grd_MissingFiles.IsVisible = true;
            }
            if ((missingNewFiles?.Count ?? 0) > 0)
            {
                txt_blk_MissingNew.Text = string.Join("\n", missingNewFiles ?? []);
                grd_MissingFiles.IsVisible = true;
            }
            if (grd_MissingFiles.IsVisible)
            {
                Height = 400;
            }
            else
            {
                Height = 200;
            }

            if (!string.IsNullOrEmpty(errorString))
            {
                txt_blk_ErrorString.Text = errorString;
            }
        }

        private void DismissClicked(object? sender, RoutedEventArgs args)
        {
            Close();
        }
    }
}
