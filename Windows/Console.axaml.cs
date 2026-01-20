/*Copyright 2020-2026 dan0v

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
using Avalonia.Threading;
using System;
using System.Diagnostics;

namespace xdelta3_cross_gui
{
    public partial class Console : Window
    {
        private static MainWindow? _parent;

        public bool CanClose = false;

        public Console()
        {
            InitializeComponent();
        }

        public void SetParent(MainWindow parent)
        {
            _parent = parent;
        }

        protected override void OnClosing(WindowClosingEventArgs e)
        {
            e.Cancel = !CanClose;

            // Hide window instead of closing
            if (!CanClose && _parent != null)
            {
                try
                {
                    _parent.ShowTerminal = false;
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
                BeginMoveDrag(e);
            }
            base.OnPointerPressed(e);
        }

        public void AddLine(string input)
        {
            Dispatcher.UIThread.InvokeAsync(new Action(() =>
            {
                if (txt_blk_Output == null)
                {
                    return;
                }
                if (txt_blk_Output.Text == null)
                {
                    txt_blk_Output.Text = "";
                }
                if (txt_blk_Output.Text?.Length > 20000)
                {
                    txt_blk_Output.Text = txt_blk_Output.Text[15000..];
                }
                txt_blk_Output.Text += input + "\n\n";
                sv_ScrollConsole.ScrollToEnd();
            }));
        }
    }
}
