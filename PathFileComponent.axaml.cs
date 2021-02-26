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
using Avalonia.Markup.Xaml;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace xdelta3_cross_gui
{
    public class PathFileComponent : UserControl
    {
        private MainWindow MainParent { get; set; }
        public string FullPath { get; set; }
        public string ShortName { get; set; }
        public int Index { get; set; }

        public bool _Shifted = false;

        private MainWindow.FileCategory _FileCategory;

        private bool _IsSelected = false;
        public bool IsSelected {
            get => _IsSelected;
            set
            {
                if (value != _IsSelected)
                {
                    _IsSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        TextBlock txt_blk_Index;
        TextBox txt_bx_Path;
        CheckBox chk_IsChecked;

        public PathFileComponent()
        {
            this.InitializeComponent();
        }
        public PathFileComponent(MainWindow parent,  string url, int index, MainWindow.FileCategory fileCategory)
        {
            this.InitializeComponent();


            this.txt_blk_Index = this.FindControl<TextBlock>("txt_blk_Index");
            this.txt_bx_Path = this.FindControl<TextBox>("txt_bx_Path");
            this.chk_IsChecked = this.FindControl<CheckBox>("chk_IsChecked");

            this.MainParent = parent;
            this.FullPath = "";
            this.ShortName = "";
            this.Index = index;
            this._FileCategory = fileCategory;


            try
            {
                this.FullPath = Path.GetFullPath(url);
                this.ShortName = Path.GetFileName(url);
            }
            catch (Exception e) { Debug.WriteLine(e); }

            this.UpdateValues();
        }

        public void UpdateValues()
        {
            txt_blk_Index.Text = this.Index + "";

            txt_bx_Path.Text = this.MainParent.Options.ShowFullPaths ? this.FullPath : this.ShortName;

            chk_IsChecked.IsChecked = this.IsSelected;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
