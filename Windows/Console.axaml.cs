﻿/*Copyright 2020-2023 dan0v

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
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using System;
using System.ComponentModel;
using System.Diagnostics;

namespace xdelta3_cross_gui
{
    public partial class Console : Window
    {
        private static MainWindow _Parent;

        public bool CanClose = false;

        public Console()
        {
            this.InitializeComponent();
        }

        public void SetParent(MainWindow parent)
        {
            _Parent = parent;
        }

        protected override void OnClosing(WindowClosingEventArgs e)
        {
            e.Cancel = !this.CanClose;

            // Hide window instead of closing
            if (!this.CanClose)
            {
                try
                {
                    _Parent.ShowTerminal = false;
                }
                catch (Exception e1)
                {
                    Debug.WriteLine(e1);
                }
            }
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            if (e.Pointer.IsPrimary)
            {
                this.BeginMoveDrag(e);
            }
            base.OnPointerPressed(e);
        }

        public void AddLine(string input)
        {
            Dispatcher.UIThread.InvokeAsync(new Action(() =>
            {
                if (this.txt_blk_Output?.Text == null)
                {
                    this.txt_blk_Output.Text = "";
                }
                if (this.txt_blk_Output.Text.Length > 20000)
                {
                    this.txt_blk_Output.Text = this.txt_blk_Output.Text.Substring(15000, this.txt_blk_Output.Text.Length - 15000);
                }
                this.txt_blk_Output.Text += input + "\n\n";
            }));
        }
    }
}
