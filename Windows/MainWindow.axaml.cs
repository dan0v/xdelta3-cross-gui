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
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using xdelta3_cross_gui.Localization;
using static System.Environment;

namespace xdelta3_cross_gui
{
    public class MainWindow : Window, INotifyPropertyChanged
    {
        public static readonly string VERSION = GetVersion();
        public static readonly string TITLE = "xDelta3 Cross GUI " + VERSION;
        public static string XDELTA3_PATH = "";

        public static readonly string XDELTA3_BINARY_WINDOWS = "xdelta3_x86_64_win.exe";
        public static readonly string XDELTA3_BINARY_LINUX = "xdelta3_x64_linux";
        public static readonly string XDELTA3_BINARY_MACOS = "xdelta3_mac";
        public static string XDELTA3_APP_STORAGE
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(SpecialFolder.LocalApplicationData, SpecialFolderOption.DoNotVerify), "xdelta3-cross-gui");
            }
        }
        public static readonly string VERSION_CHECK_URL = "https://github.com/dan0v/xdelta3-cross-gui/releases/latest/download/version.txt";
        public static readonly string RELEASES_PAGE = "https://github.com/dan0v/xdelta3-cross-gui/releases/latest/";

        private bool _XDeltaOnSystemPath { get; set; }
        public bool XDeltaOnSystemPath
        {
            get => _XDeltaOnSystemPath;
            set
            {
                if (value != _XDeltaOnSystemPath)
                {
                    _XDeltaOnSystemPath = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _EqualFileCount { get; set; }
        public bool EqualFileCount
        {
            get => _EqualFileCount;
            set
            {
                if (value != _EqualFileCount)
                {
                    _EqualFileCount = value;
                    OnPropertyChanged();
                }
            }
        }
        private double _PatchProgress { get; set; }
        public double PatchProgress
        {
            get => _PatchProgress;
            set
            {
                if (value != _PatchProgress)
                {
                    _PatchProgress = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _PatchProgressIsIndeterminate { get; set; }
        public bool PatchProgressIsIndeterminate
        {
            get => _PatchProgressIsIndeterminate;
            set
            {
                if (value != _PatchProgressIsIndeterminate)
                {
                    _PatchProgressIsIndeterminate = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _AlreadyBusy { get; set; }
        public bool AlreadyBusy
        {
            get => _AlreadyBusy;
            set
            {
                if (value != _AlreadyBusy)
                {
                    _AlreadyBusy = value;
                    OnPropertyChanged();
                }
            }
        }
        public List<PathFileComponent> OldFilesList { get; set; }
        public List<PathFileComponent> NewFilesList { get; set; }
        private int _OldFilesListCount { get; set; }
        public int OldFilesListCount
        {
            get => _OldFilesListCount;
            set
            {
                if (value != _OldFilesListCount)
                {
                    _OldFilesListCount = value;
                    OnPropertyChanged();
                }
            }
        }
        private int _NewFilesListCount { get; set; }
        public int NewFilesListCount
        {
            get => _NewFilesListCount;
            set
            {
                if (value != _NewFilesListCount)
                {
                    _NewFilesListCount = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool ShowTerminal
        {
            get => Options.ShowTerminal;
            set
            {
                Options.ShowTerminal = value;
                try
                {
                    if (value)
                    {
                        this.Console.Show();
                        this.Console.Activate();
                        this.Console.Focus();
                    }
                    else
                    {
                        this.Console.Hide();
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                    this._Console = new Console();
                }
                OnPropertyChanged();
            }
        }

        private bool _AllOldFilesSelected = false;
        private bool _AllNewFilesSelected = false;

        private Console _Console = new Console();
        public Console Console { get => _Console; }

        private Options _Options = new Options();
        public Options Options { get { return this._Options; } }

        Button btn_ToggleAllOldFilesSelection;
        Button btn_ToggleAllNewFilesSelection;
        Button btn_AddOld;
        Button btn_UpOld;
        Button btn_DownOld;
        Button btn_DeleteOld;
        Button btn_AddNew;
        Button btn_UpNew;
        Button btn_DownNew;
        Button btn_DeleteNew;
        Button btn_BrowsePathDestination;
        Button btn_ResetDefaults;
        Button btn_SaveSettings;
        Button btn_Go;
        StackPanel sp_OldFilesDisplay;
        StackPanel sp_NewFilesDisplay;
        ScrollViewer sv_OldFilesDisplay;
        ScrollViewer sv_NewFilesDisplay;
        CheckBox chk_UseShortNames;
        public ProgressBar pb_Progress;
        ComboBox cb_LanguageOptions;
        TextBox txt_bx_PatchDestination;

        public enum FileCategory { New, Old };

        public MainWindow()
        {
            InitializeComponent();
            this.Configure();
        }

        private void InitializeComponent()
        {
            Localizer.Instance.LoadLanguage();
            AvaloniaXamlLoader.Load(this);
        }

        #region public
        public void OldFilesDropped(object sender, DragEventArgs args)
        {
            if (args.Data.Contains(DataFormats.FileNames))
            {
                List<string> url = new List<string>(args.Data.GetFileNames());
                this.AddFiles(url.ToArray(), FileCategory.Old);
            }
        }
        public async void AddOldFileClicked(object sender, RoutedEventArgs args)
        {
            try
            {
                string[] url = await this.OpenFileBrowser();
                this.AddFiles(url, FileCategory.Old);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }
        public void MoveOldFileUpClicked(object sender, RoutedEventArgs args)
        {
            this.MoveFilesUp(FileCategory.Old);
        }
        public void MoveOldFileDownClicked(object sender, RoutedEventArgs args)
        {
            this.MoveFilesDown(FileCategory.Old);
        }
        public void DeleteOldFilesClicked(object sender, RoutedEventArgs args)
        {
            this.DeleteFiles(FileCategory.Old);
        }
        public void ToggleAllOldFilesSelectionClicked(object sender, RoutedEventArgs args)
        {
            this.ToggleAllFilesSelection(FileCategory.Old);
        }

        public void NewFilesDropped(object sender, DragEventArgs args)
        {
            if (args.Data.Contains(DataFormats.FileNames))
            {
                List<string> url = new List<string>(args.Data.GetFileNames());
                this.AddFiles(url.ToArray(), FileCategory.New);
            }
        }
        public async void AddNewFileClicked(object sender, RoutedEventArgs args)
        {
            try
            {
                string[] url = await this.OpenFileBrowser();
                this.AddFiles(url, FileCategory.New);

            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }
        public void MoveNewFileUpClicked(object sender, RoutedEventArgs args)
        {
            this.MoveFilesUp(FileCategory.New);
        }
        public void MoveNewFileDownClicked(object sender, RoutedEventArgs args)
        {
            this.MoveFilesDown(FileCategory.New);
        }
        public void DeleteNewFilesClicked(object sender, RoutedEventArgs args)
        {
            this.DeleteFiles(FileCategory.New);
        }
        public void ToggleAllNewFilesSelectionClicked(object sender, RoutedEventArgs args)
        {
            this.ToggleAllFilesSelection(FileCategory.New);
        }

        public void ReloadFiles(FileCategory category, bool forceReloadContents = false)
        {
            List<PathFileComponent> components = this.NewFilesList;
            StackPanel sp = sp_NewFilesDisplay;

            if (category == FileCategory.New)
            {
                components = this.NewFilesList;
                sp = sp_NewFilesDisplay;
            }
            else if (category == FileCategory.Old)
            {
                components = this.OldFilesList;
                sp = sp_OldFilesDisplay;
            }

            sp.Children.Clear();

            if (forceReloadContents)
            {
                for (int i = 0; i < components.Count; i++)
                {
                    components[i].Index = i;
                    components[i]._Shifted = false;
                    components[i].UpdateValues();
                }
            }

            sp.Children.AddRange(components);
        }

        public void SaveSettingsClicked(object sender, RoutedEventArgs args)
        {
            this.Options.SaveCurrent();
        }

        public void ResetDefaultsClicked(object sender, RoutedEventArgs args)
        {
            this.Options.ResetToDefault();
            //this.Options.SaveCurrent();
        }

        public void GoClicked(object sender, RoutedEventArgs args)
        {
            bool failed = false;
            List<string> missingOldFiles = new List<string>();
            List<string> missingNewFiles = new List<string>();

            foreach (PathFileComponent component in this.OldFilesList)
            {
                if (!File.Exists(component.FullPath))
                {
                    missingOldFiles.Add(component.FullPath);
                    failed = true;
                }
            }

            foreach (PathFileComponent component in this.NewFilesList)
            {
                if (!File.Exists(component.FullPath))
                {
                    missingNewFiles.Add(component.FullPath);
                    failed = true;
                }
            }

            if (!this.Options.Validate())
            {
                failed = true;
            }

            if (failed)
            {
                ErrorDialog dialog = new ErrorDialog(missingOldFiles, missingNewFiles);
                dialog.Show();
                dialog.Topmost = true;
                this.AlreadyBusy = false;
            }
            else
            {
                PatchCreator patcher = new PatchCreator(this);
                this.AlreadyBusy = true;
                patcher.CreateReadme();
                patcher.CopyNotice();
                if (this.Options.CopyExecutables)
                {
                    patcher.CopyExecutables();
                }
                patcher.CreatePatchingBatchFiles();
            }
        }

        public async void BrowseOutputDirectory(object sender, RoutedEventArgs args)
        {
            try
            {
                string url = await this.OpenFolderBrowser();
                this.Options.PatchFileDestination = url;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        public void UseShortNamesChecked(object sender, RoutedEventArgs args)
        {
            this.ReloadFiles(FileCategory.New, true);
            this.ReloadFiles(FileCategory.Old, true);
        }

        public void SortListInPlaceByIndex(List<PathFileComponent> list)
        {
            list.Sort((x, y) => x.Index.CompareTo(y.Index));
        }

        public void ChangeLanguage(string language)
        {
            if (language == null || !Localizer.Languages.ContainsKey(language))
            {
                language = "English";
            }

            CultureInfo.CurrentUICulture = new CultureInfo(Localizer.Languages[language]);
            Options.Language = language;
            Localizer.Instance.LoadLanguage();
        }
        #endregion

        #region private
        private void Configure()
        {
            this.Title = TITLE;

            this.EqualFileCount = false;
            this.AlreadyBusy = false;
            this.PatchProgressIsIndeterminate = false;
            this.OldFilesListCount = 0;
            this.NewFilesListCount = 0;

            this.Options.LoadSaved();
            this.SetXDeltaLocations();

            this.OldFilesList = new List<PathFileComponent>();
            this.NewFilesList = new List<PathFileComponent>();

            // Button Click event does not compile in XAML, so has to be manually added https://github.com/AvaloniaUI/Avalonia/issues/3898
            this.btn_ToggleAllOldFilesSelection = this.FindControl<Button>("btn_ToggleAllOldFilesSelection");
            this.btn_ToggleAllNewFilesSelection = this.FindControl<Button>("btn_ToggleAllNewFilesSelection");
            this.btn_AddOld = this.FindControl<Button>("btn_AddOld");
            this.btn_UpOld = this.FindControl<Button>("btn_UpOld");
            this.btn_DownOld = this.FindControl<Button>("btn_DownOld");
            this.btn_DeleteOld = this.FindControl<Button>("btn_DeleteOld");
            this.btn_AddNew = this.FindControl<Button>("btn_AddNew");
            this.btn_UpNew = this.FindControl<Button>("btn_UpNew");
            this.btn_DownNew = this.FindControl<Button>("btn_DownNew");
            this.btn_DeleteNew = this.FindControl<Button>("btn_DeleteNew");
            this.btn_BrowsePathDestination = this.FindControl<Button>("btn_BrowsePathDestination");
            this.sp_OldFilesDisplay = this.FindControl<StackPanel>("sp_OldFilesDisplay");
            this.sp_NewFilesDisplay = this.FindControl<StackPanel>("sp_NewFilesDisplay");
            this.sv_OldFilesDisplay = this.FindControl<ScrollViewer>("sv_OldFilesDisplay");
            this.sv_NewFilesDisplay = this.FindControl<ScrollViewer>("sv_NewFilesDisplay");
            this.chk_UseShortNames = this.FindControl<CheckBox>("chk_UseShortNames");
            this.txt_bx_PatchDestination = this.FindControl<TextBox>("txt_bx_PatchDestination");
            this.btn_SaveSettings = this.FindControl<Button>("btn_SaveSettings");
            this.btn_ResetDefaults = this.FindControl<Button>("btn_ResetDefaults");
            this.btn_Go = this.FindControl<Button>("btn_Go");
            this.pb_Progress = this.FindControl<ProgressBar>("pb_Progress");
            this.cb_LanguageOptions = this.FindControl<ComboBox>("cb_LanguageOptions");

            // Bindings
            this.btn_ToggleAllOldFilesSelection.Click += ToggleAllOldFilesSelectionClicked;
            this.btn_ToggleAllNewFilesSelection.Click += ToggleAllNewFilesSelectionClicked;
            this.btn_AddOld.Click += AddOldFileClicked;
            this.btn_UpOld.Click += MoveOldFileUpClicked;
            this.btn_DownOld.Click += MoveOldFileDownClicked;
            this.btn_DeleteOld.Click += DeleteOldFilesClicked;
            this.btn_AddNew.Click += AddNewFileClicked;
            this.btn_UpNew.Click += MoveNewFileUpClicked;
            this.btn_DownNew.Click += MoveNewFileDownClicked;
            this.btn_DeleteNew.Click += DeleteNewFilesClicked;
            this.btn_SaveSettings.Click += SaveSettingsClicked;
            this.btn_ResetDefaults.Click += ResetDefaultsClicked;
            this.btn_Go.Click += GoClicked;
            this.btn_BrowsePathDestination.Click += BrowseOutputDirectory;
            this.chk_UseShortNames.Click += UseShortNamesChecked;
            this.cb_LanguageOptions.SelectionChanged += ChangeLanguageSelection;

            this.sv_OldFilesDisplay.AddHandler(DragDrop.DropEvent, OldFilesDropped);
            this.sv_NewFilesDisplay.AddHandler(DragDrop.DropEvent, NewFilesDropped);

            this.LoadLanguageOptions();

            this.ChangeLanguage(Options.Language);
            this.MatchSelectedLanguage();

            this.Console.SetParent(this);
            this.CheckForUpdates();
        }

        private void CheckFileCounts()
        {
            if (this.OldFilesList.Count != this.NewFilesList.Count || this.OldFilesList.Count == 0)
            {
                this.EqualFileCount = false;
            }
            else
            {
                this.EqualFileCount = true;
            }
            this.OldFilesListCount = this.OldFilesList.Count;
            this.NewFilesListCount = this.NewFilesList.Count;
        }
        private void AddFiles(string[] urls, FileCategory version)
        {
            List<PathFileComponent> filesList = this.NewFilesList;

            if (version == FileCategory.New)
            {
                filesList = this.NewFilesList;
            }
            else if (version == FileCategory.Old)
            {
                filesList = this.OldFilesList;
            }

            if (urls.Length > 0)
            {
                foreach (string path in urls)
                {
                    filesList.Add(new PathFileComponent(this, path, filesList.Count, version));
                }
                this.ReloadFiles(version);
                this.CheckFileCounts();

                if (version == FileCategory.New)
                {
                    if (this.Options.PatchFileDestination == "")
                    {
                        this.Options.PatchFileDestination = Path.Combine(Path.GetDirectoryName(this.NewFilesList[0].FullPath), "xDelta3_Output");
                    }
                }
            }
        }
        private void MoveFilesUp(FileCategory category)
        {
            List<PathFileComponent> list = this.NewFilesList;
            if (category == FileCategory.New)
            {
                list = this.NewFilesList;
            }
            else if (category == FileCategory.Old)
            {
                list = this.OldFilesList;
            }

            List<PathFileComponent> selectedList = list.FindAll(c => c.IsSelected == true);
            this.SortListInPlaceByIndex(selectedList);
            for (int i = 0; i < selectedList.Count; i++)
            {
                PathFileComponent component = selectedList[i];
                // Do not shift up if element before it was not shifted
                if (component.Index != 0 && (i == 0 || (i > 0 && (list.IndexOf(component) - list.IndexOf(selectedList[i - 1]) > 1) || selectedList[i - 1]._Shifted == true)))
                {
                    list[list.IndexOf(component) - 1].Index++;
                    component.Index--;
                    component._Shifted = true;
                    this.SortListInPlaceByIndex(list);
                }
            }
            this.ReloadFiles(category, true);
        }
        private void MoveFilesDown(FileCategory category)
        {
            List<PathFileComponent> list = this.NewFilesList;
            if (category == FileCategory.New)
            {
                list = this.NewFilesList;
            }
            else if (category == FileCategory.Old)
            {
                list = this.OldFilesList;
            }

            List<PathFileComponent> selectedList = list.FindAll(c => c.IsSelected == true);
            this.SortListInPlaceByIndex(selectedList);
            for (int i = selectedList.Count - 1; i >= 0; i--)
            {
                PathFileComponent component = selectedList[i];
                // Do not shift down if element after it was not shifted
                if (component.Index != list.Count - 1 && (i == selectedList.Count - 1 || (i < selectedList.Count - 1 && (list.IndexOf(selectedList[i + 1]) - list.IndexOf(component)) > 1) || selectedList[i + 1]._Shifted == true))
                {
                    list[list.IndexOf(component) + 1].Index--;
                    component.Index++;
                    component._Shifted = true;
                    this.SortListInPlaceByIndex(list);
                }
            }
            this.ReloadFiles(category, true);
        }
        private void DeleteFiles(FileCategory category)
        {
            List<PathFileComponent> list = this.NewFilesList;
            if (category == FileCategory.New)
            {
                list = this.NewFilesList;
            }
            else if (category == FileCategory.Old)
            {
                list = this.OldFilesList;
            }

            try
            {
                List<PathFileComponent> deletableList = list.FindAll(c => c.IsSelected == true);
                foreach (PathFileComponent component in deletableList)
                {
                    list.Remove(component);
                }
                this.ReloadFiles(category, true);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            this.CheckFileCounts();

            if (category == FileCategory.New)
            {
                if (this.NewFilesListCount == 0)
                {
                    this._AllNewFilesSelected = false;
                }
            }
            else if (category == FileCategory.Old)
            {
                if (this.OldFilesListCount == 0)
                {
                    this._AllOldFilesSelected = false;
                }
            }
        }
        private void ToggleAllFilesSelection(FileCategory category)
        {
            if (category == FileCategory.New)
            {
                if (_AllNewFilesSelected)
                {
                    this.NewFilesList.ForEach(c => c.IsSelected = false);
                    this._AllNewFilesSelected = false;
                }
                else
                {
                    this.NewFilesList.ForEach(c => c.IsSelected = true);
                    this._AllNewFilesSelected = true;
                }
                ReloadFiles(FileCategory.New, true);
            }
            else if (category == FileCategory.Old)
            {
                if (_AllOldFilesSelected)
                {
                    this.OldFilesList.ForEach(c => c.IsSelected = false);
                    this._AllOldFilesSelected = false;
                }
                else
                {
                    this.OldFilesList.ForEach(c => c.IsSelected = true);
                    this._AllOldFilesSelected = true;
                }
                ReloadFiles(FileCategory.Old, true);
            }
        }

        private void LoadLanguageOptions()
        {
            List<ComboBoxItem> items = new List<ComboBoxItem>();
            foreach (string language in Localizer.Languages.Keys)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = language;
                items.Add(item);
            }
            cb_LanguageOptions.Items = items;
        }
        private void ChangeLanguageSelection(object sender, SelectionChangedEventArgs e)
        {
            string language = (string)((ComboBoxItem)cb_LanguageOptions.SelectedItem).Content;
            ChangeLanguage(language);
        }
        private void MatchSelectedLanguage()
        {
            var items = cb_LanguageOptions.Items;
            int index = items.Cast<ComboBoxItem>().ToList().FindIndex(item => (string)item.Content == Options.Language);
            cb_LanguageOptions.SelectedIndex = index;
        }

        private async void CheckForUpdates()
        {
            try
            {
                HttpResponseMessage response = await new HttpClient().GetAsync(VERSION_CHECK_URL);
                response.EnsureSuccessStatusCode();
                string newVer = await response.Content.ReadAsStringAsync();
                if (newVer.Trim() != VERSION.Trim())
                {
                    UpdateDialog updateDialog = new UpdateDialog(this, newVer);
                    updateDialog.Show();
                    updateDialog.Activate();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        private static string GetVersion()
        {
            string version = "";
            try
            {
                version = System.Reflection.Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            }
            catch (Exception e) { Debug.WriteLine(e); }
            return version;
        }

        private async Task<string[]> OpenFileBrowser()
        {
            var dialog = new OpenFileDialog()
            {
                Title = "Select file(s)",
                AllowMultiple = true,
            };
            return await dialog.ShowAsync(GetWindow());
        }
        private async Task<string> OpenFolderBrowser()
        {
            var dialog = new OpenFolderDialog()
            {
                Title = "Select output directory",

            };
            return await dialog.ShowAsync(GetWindow());
        }

        Window GetWindow() => (Window)this.VisualRoot;

        private void SetXDeltaLocations()
        {
            this.XDeltaOnSystemPath = false;

            if (File.Exists("xdelta3"))
            {
                XDELTA3_PATH = Path.GetFullPath("xdelta3");
                this.XDeltaOnSystemPath = true;
            }

            var values = Environment.GetEnvironmentVariable("PATH");
            foreach (var path in values.Split(Path.PathSeparator))
            {
                var fullPath = Path.Combine(path, "xdelta3");
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    fullPath += ".exe";
                }
                if (File.Exists(fullPath))
                {
                    XDELTA3_PATH = fullPath;
                    this.XDeltaOnSystemPath = true;
                }
            }

            if (!this.XDeltaOnSystemPath)
            {
                string location = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "exec");

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    XDELTA3_PATH = Path.Combine(location, XDELTA3_BINARY_WINDOWS);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    XDELTA3_PATH = Path.Combine(location, XDELTA3_BINARY_LINUX);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    XDELTA3_PATH = Path.Combine(location, XDELTA3_BINARY_MACOS);
                }
            }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            this.Console.CanClose = true;
            this.Console.Close();
            base.OnClosing(e);
        }

    }
}
