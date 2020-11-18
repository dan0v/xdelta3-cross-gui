using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using System;
using System.ComponentModel;
using System.Diagnostics;

namespace xdelta3_cross_gui
{
    public class Console : Window
    {
        private static MainWindow _Parent;

        public bool CanClose = false;

        TextBlock txt_blk_Output;
        ScrollViewer sv_ScrollConsole;
        public Console()
        {
            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            this.txt_blk_Output = this.FindControl<TextBlock>("txt_blk_Output");
            this.sv_ScrollConsole = this.FindControl<ScrollViewer>("sv_ScrollConsole");
        }

        public void SetParent(MainWindow parent)
        {
            _Parent = parent;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        protected override void OnClosing(CancelEventArgs e)
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
                if (this.txt_blk_Output.Text.Length > 20000)
                {
                    this.txt_blk_Output.Text = this.txt_blk_Output.Text.Substring(15000, this.txt_blk_Output.Text.Length - 15000);
                }
                this.txt_blk_Output.Text += input + "\n\n";
            }));
        }
    }
}
