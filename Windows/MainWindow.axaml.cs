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
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
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
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public static readonly string VERSION = GetVersion();
        public static readonly string TITLE = "xDelta3 Cross GUI " + VERSION;
        public static string XDELTA3_PATH = "";

        public static readonly string LICENSE_FILE = "LICENSE.txt";
        public static readonly string NOTICE_FILE = "NOTICE.txt";

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

        #region Properties
        public string OldFilesHeader => string.Format(Localizer.Instance["OldFilesHeader"], OldFilesListCount);
        public string NewFilesHeader => string.Format(Localizer.Instance["NewFilesHeader"], NewFilesListCount);
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
        public List<PathFileComponent> OldFilesList { get; set; } = [];
        public List<PathFileComponent> NewFilesList { get; set; } = [];
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
                    OnPropertyChanged(nameof(OldFilesHeader));
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
                    OnPropertyChanged(nameof(NewFilesHeader));
                }
            }
        }

        public bool ShowTerminal
        {
            get => Config.ShowTerminal;
            set
            {
                Config.ShowTerminal = value;
                try
                {
                    if (value)
                    {
                        Console.Show();
                        Console.Activate();
                        Console.Focus();
                    }
                    else
                    {
                        Console.Hide();
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                    _Console = new Console();
                }
                OnPropertyChanged();
            }
        }
        #endregion

        private bool _AllOldFilesSelected = false;
        private bool _AllNewFilesSelected = false;

        private Console _Console = new();
        public Console Console => _Console;

        private readonly Config _Config = new();
        public Config Config { get { return _Config; } }

        public enum FileCategory { New, Old };

        public MainWindow()
        {
            Localizer.Instance.LoadLanguage();
            InitializeComponent();
            BindUI();
            Configure();
        }

        #region public
        public void OldFilesDropped(object? sender, DragEventArgs args)
        {
            HandleFileDrop(args, FileCategory.Old);
        }
        public async void AddOldFileClicked(object? sender, RoutedEventArgs args)
        {
            try
            {
                var url = await OpenFileBrowser();
                if (url != null)
                {
                    AddFiles(url, FileCategory.Old);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }
        public void MoveOldFileUpClicked(object? sender, RoutedEventArgs args)
        {
            MoveFilesUp(FileCategory.Old);
        }
        public void MoveOldFileDownClicked(object? sender, RoutedEventArgs args)
        {
            MoveFilesDown(FileCategory.Old);
        }
        public void DeleteOldFilesClicked(object? sender, RoutedEventArgs args)
        {
            DeleteFiles(FileCategory.Old);
        }
        public void ToggleAllOldFilesSelectionClicked(object? sender, RoutedEventArgs args)
        {
            ToggleAllFilesSelection(FileCategory.Old);
        }

        public void NewFilesDropped(object? sender, DragEventArgs args)
        {
            HandleFileDrop(args, FileCategory.New);
        }

        public async void AddNewFileClicked(object? sender, RoutedEventArgs args)
        {
            try
            {
                var url = await OpenFileBrowser();
                if (url != null)
                {
                    AddFiles(url, FileCategory.New);
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }
        public void MoveNewFileUpClicked(object? sender, RoutedEventArgs args)
        {
            MoveFilesUp(FileCategory.New);
        }
        public void MoveNewFileDownClicked(object? sender, RoutedEventArgs args)
        {
            MoveFilesDown(FileCategory.New);
        }
        public void DeleteNewFilesClicked(object? sender, RoutedEventArgs args)
        {
            DeleteFiles(FileCategory.New);
        }
        public void ToggleAllNewFilesSelectionClicked(object? sender, RoutedEventArgs args)
        {
            ToggleAllFilesSelection(FileCategory.New);
        }

        public void ReloadFiles(FileCategory category, bool forceReloadContents = false)
        {
            List<PathFileComponent> components = NewFilesList;
            StackPanel sp = sp_NewFilesDisplay;

            if (category == FileCategory.New)
            {
                components = NewFilesList;
                sp = sp_NewFilesDisplay;
            }
            else if (category == FileCategory.Old)
            {
                components = OldFilesList;
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
        public void ResetDefaultsClicked(object? sender, RoutedEventArgs args)
        {
            Config.ResetToDefault();
        }

        public void OpenInfoClicked(object? sender, RoutedEventArgs args)
        {
            var info = new InfoDialog();
            info.Show();
        }

        public void GoClicked(object? sender, RoutedEventArgs args)
        {
            bool failed = false;
            List<string> missingOldFiles = [];
            List<string> missingNewFiles = [];

            foreach (PathFileComponent component in OldFilesList)
            {
                if (!File.Exists(component.FullPath))
                {
                    missingOldFiles.Add(component.FullPath);
                    failed = true;
                }
            }

            foreach (PathFileComponent component in NewFilesList)
            {
                if (!File.Exists(component.FullPath))
                {
                    missingNewFiles.Add(component.FullPath);
                    failed = true;
                }
            }

            if (!Config.Validate())
            {
                failed = true;
            }

            if (failed)
            {
                ErrorDialog dialog = new(missingOldFiles, missingNewFiles);
                dialog.Show();
                dialog.Topmost = true;
                AlreadyBusy = false;
            }
            else
            {
                AlreadyBusy = true;
                PatchCreator.Instance.CreateReadme();
                PatchCreator.Instance.CopyNotice();
                if (Config.CopyExecutables)
                {
                    PatchCreator.Instance.CopyExecutables();
                }
                PatchCreator.Instance.CreatePatchingBatchFiles();
            }
        }

        public async void BrowseOutputDirectory(object? sender, RoutedEventArgs args)
        {
            try
            {
                string? url = await OpenFolderBrowser();
                if (!string.IsNullOrEmpty(url))
                {
                    Config.PatchFileDestination = url;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        public void UseShortNamesChecked(object? sender, RoutedEventArgs args)
        {
            ReloadFiles(FileCategory.New, true);
            ReloadFiles(FileCategory.Old, true);
        }

        public void SortListInPlaceByIndex(List<PathFileComponent> list)
        {
            list.Sort((x, y) => x.Index.CompareTo(y.Index));
        }

        public void ChangeLanguage(string? language)
        {
            if (language == null || !Localizer.Languages.ContainsKey(language))
            {
                language = "English";
            }

            CultureInfo.CurrentUICulture = new CultureInfo(Localizer.Languages[language]);
            Config.Language = language;
            Localizer.Instance.LoadLanguage();
        }
        #endregion

        #region private
        private void BindUI()
        {
            Title = TITLE;

            // Bindings
            btn_ToggleAllOldFilesSelection.Click += ToggleAllOldFilesSelectionClicked;
            btn_ToggleAllNewFilesSelection.Click += ToggleAllNewFilesSelectionClicked;
            btn_AddOld.Click += AddOldFileClicked;
            btn_UpOld.Click += MoveOldFileUpClicked;
            btn_DownOld.Click += MoveOldFileDownClicked;
            btn_DeleteOld.Click += DeleteOldFilesClicked;
            btn_AddNew.Click += AddNewFileClicked;
            btn_UpNew.Click += MoveNewFileUpClicked;
            btn_DownNew.Click += MoveNewFileDownClicked;
            btn_DeleteNew.Click += DeleteNewFilesClicked;
            btn_ResetDefaults.Click += ResetDefaultsClicked;
            btn_OpenInfo.Click += OpenInfoClicked;
            btn_Go.Click += GoClicked;
            btn_BrowsePathDestination.Click += BrowseOutputDirectory;
            chk_UseShortNames.Click += UseShortNamesChecked;
            cb_LanguageOptions.SelectionChanged += ChangeLanguageSelection;

            sv_OldFilesDisplay.AddHandler(DragDrop.DropEvent, OldFilesDropped);
            sv_NewFilesDisplay.AddHandler(DragDrop.DropEvent, NewFilesDropped);
        }
        private void Configure()
        {
            EqualFileCount = false;
            AlreadyBusy = false;
            PatchProgressIsIndeterminate = false;
            OldFilesListCount = 0;
            NewFilesListCount = 0;

            Localizer.Instance.PropertyChanged += Language_Changed;

            CreateDirectories();
            Config.LoadSaved();
            SetXDeltaLocations();

            LoadLanguageOptions();

            ChangeLanguage(Config.Language);
            MatchSelectedLanguage();

            Console.SetParent(this);
            CheckForUpdates();
        }

        private void Language_Changed(object? sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(OldFilesHeader));
            OnPropertyChanged(nameof(NewFilesHeader));
        }

        private void CheckFileCounts()
        {
            if (OldFilesList.Count != NewFilesList.Count || OldFilesList.Count == 0)
            {
                EqualFileCount = false;
            }
            else
            {
                EqualFileCount = true;
            }
            OldFilesListCount = OldFilesList.Count;
            NewFilesListCount = NewFilesList.Count;
        }
        private void AddFiles(string[] urls, FileCategory version)
        {
            List<PathFileComponent> filesList = NewFilesList;

            if (version == FileCategory.New)
            {
                filesList = NewFilesList;
            }
            else if (version == FileCategory.Old)
            {
                filesList = OldFilesList;
            }

            if (urls?.Length > 0)
            {
                foreach (string path in urls)
                {
                    filesList.Add(new PathFileComponent(this, path, filesList.Count, version));
                }
                ReloadFiles(version);
                CheckFileCounts();

                if (version == FileCategory.New)
                {
                    if (Config.PatchFileDestination == "")
                    {
                        Config.PatchFileDestination = Path.Combine(Path.GetDirectoryName(NewFilesList[0].FullPath) ?? "", "xDelta3_Output");
                    }
                }
            }
        }

        private void HandleFileDrop(DragEventArgs args, FileCategory fileCategory)
        {
            if (args.Data.Contains(DataFormats.Files))
            {
                List<String> urls = args.Data?.GetFiles()?.Select(f => Uri.UnescapeDataString(f.Path.AbsolutePath)).Where(f => File.Exists(f)).ToList() ?? [];
                AddFiles(urls.ToArray(), fileCategory);
            }
        }

        private void MoveFilesUp(FileCategory category)
        {
            List<PathFileComponent> list = NewFilesList;
            if (category == FileCategory.New)
            {
                list = NewFilesList;
            }
            else if (category == FileCategory.Old)
            {
                list = OldFilesList;
            }

            List<PathFileComponent> selectedList = list.FindAll(c => c.IsSelected == true);
            if (selectedList.Count > 0)
            {
                SortListInPlaceByIndex(selectedList);
                for (int i = 0; i < selectedList.Count; i++)
                {
                    PathFileComponent component = selectedList[i];
                    // Do not shift up if element before it was not shifted
                    if (component.Index != 0 && (i == 0 || (i > 0 && (list.IndexOf(component) - list.IndexOf(selectedList[i - 1]) > 1) || selectedList[i - 1]._Shifted == true)))
                    {
                        list[list.IndexOf(component) - 1].Index++;
                        component.Index--;
                        component._Shifted = true;
                        SortListInPlaceByIndex(list);
                    }
                }
                ReloadFiles(category, true);
            }
        }
        private void MoveFilesDown(FileCategory category)
        {
            List<PathFileComponent> list = NewFilesList;
            if (category == FileCategory.New)
            {
                list = NewFilesList;
            }
            else if (category == FileCategory.Old)
            {
                list = OldFilesList;
            }

            List<PathFileComponent> selectedList = list.FindAll(c => c.IsSelected == true);
            if (selectedList.Count > 0)
            {
                SortListInPlaceByIndex(selectedList);
                for (int i = selectedList.Count - 1; i >= 0; i--)
                {
                    PathFileComponent component = selectedList[i];
                    // Do not shift down if element after it was not shifted
                    if (component.Index != list.Count - 1 && (i == selectedList.Count - 1 || (i < selectedList.Count - 1 && (list.IndexOf(selectedList[i + 1]) - list.IndexOf(component)) > 1) || selectedList[i + 1]._Shifted == true))
                    {
                        list[list.IndexOf(component) + 1].Index--;
                        component.Index++;
                        component._Shifted = true;
                        SortListInPlaceByIndex(list);
                    }
                }
                ReloadFiles(category, true);
            }
        }
        private void DeleteFiles(FileCategory category)
        {
            List<PathFileComponent> list = NewFilesList;
            if (category == FileCategory.New)
            {
                list = NewFilesList;
            }
            else if (category == FileCategory.Old)
            {
                list = OldFilesList;
            }

            try
            {
                List<PathFileComponent> deletableList = list.FindAll(c => c.IsSelected == true);
                foreach (PathFileComponent component in deletableList)
                {
                    list.Remove(component);
                }
                ReloadFiles(category, true);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            CheckFileCounts();

            if (category == FileCategory.New)
            {
                if (NewFilesListCount == 0)
                {
                    _AllNewFilesSelected = false;
                }
            }
            else if (category == FileCategory.Old)
            {
                if (OldFilesListCount == 0)
                {
                    _AllOldFilesSelected = false;
                }
            }
        }
        private void ToggleAllFilesSelection(FileCategory category)
        {
            if (category == FileCategory.New)
            {
                if (_AllNewFilesSelected)
                {
                    NewFilesList.ForEach(c => c.IsSelected = false);
                    _AllNewFilesSelected = false;
                }
                else
                {
                    NewFilesList.ForEach(c => c.IsSelected = true);
                    _AllNewFilesSelected = true;
                }
                ReloadFiles(FileCategory.New, true);
            }
            else if (category == FileCategory.Old)
            {
                if (_AllOldFilesSelected)
                {
                    OldFilesList.ForEach(c => c.IsSelected = false);
                    _AllOldFilesSelected = false;
                }
                else
                {
                    OldFilesList.ForEach(c => c.IsSelected = true);
                    _AllOldFilesSelected = true;
                }
                ReloadFiles(FileCategory.Old, true);
            }
        }

        private void LoadLanguageOptions()
        {
            List<ComboBoxItem> items = [];
            foreach (string language in Localizer.Languages.Keys)
            {
                ComboBoxItem item = new()
                {
                    Content = language
                };
                items.Add(item);
            }
            cb_LanguageOptions.ItemsSource = items;
        }
        private void ChangeLanguageSelection(object? sender, SelectionChangedEventArgs e)
        {
            string? language = ((ComboBoxItem?)cb_LanguageOptions.SelectedItem)?.Content as string;
            ChangeLanguage(language);
            Config.SaveCurrent();
        }
        private void MatchSelectedLanguage()
        {
            var items = cb_LanguageOptions.Items;
            int index = items.Cast<ComboBoxItem>().ToList().FindIndex(item => (string?)item.Content == Config.Language);
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
                    UpdateDialog updateDialog = new(this, newVer);
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
                var ver = Assembly.GetEntryAssembly()?.GetName().Version;
                if (ver == null)
                {
                    return "0.0.0";
                }
                version = $"{ver.Major}.{ver.Minor}.{ver.Build}";
            }
            catch (Exception e) { Debug.WriteLine(e); }
            return version;
        }

        private async Task<string[]?> OpenFileBrowser()
        {
            return (await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Select file(s)",
                AllowMultiple = true
            })).Select(file => Uri.UnescapeDataString(file.Path.AbsolutePath)).Where(f => File.Exists(f)).ToArray();
        }

        private async Task<string?> OpenFolderBrowser()
        {
            return (await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = "Select output directory"
            })).Select(folder => Uri.UnescapeDataString(folder.Path.AbsolutePath)).Where(f => Directory.Exists(f)).FirstOrDefault();
        }

        Window? GetWindow() => VisualRoot as Window;

        private void SetXDeltaLocations()
        {
            XDeltaOnSystemPath = false;

            if (File.Exists("xdelta3"))
            {
                XDELTA3_PATH = Path.GetFullPath("xdelta3");
                XDeltaOnSystemPath = true;
            }

            var values = Environment.GetEnvironmentVariable("PATH");
            foreach (var path in values?.Split(Path.PathSeparator) ?? [])
            {
                var fullPath = Path.Combine(path, "xdelta3");
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    fullPath += ".exe";
                }
                if (File.Exists(fullPath))
                {
                    XDELTA3_PATH = fullPath;
                    XDeltaOnSystemPath = true;
                }
            }

            if (!XDeltaOnSystemPath)
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

        private void CreateDirectories()
        {
            try
            {
                if (!Directory.Exists(MainWindow.XDELTA3_APP_STORAGE))
                {
#if MacOS
                    var oldLocalAppData = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile, Environment.SpecialFolderOption.DoNotVerify), ".local/share/xdelta3-cross-gui");

                    if (Directory.Exists(oldLocalAppData))
                    {
                        Directory.Move(oldLocalAppData, MainWindow.XDELTA3_APP_STORAGE);
                    }
                    else
                    {
                         Directory.CreateDirectory(MainWindow.XDELTA3_APP_STORAGE);
                    }
#else
                    Directory.CreateDirectory(MainWindow.XDELTA3_APP_STORAGE);
#endif
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }
        #endregion

        new public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected override void OnClosing(WindowClosingEventArgs e)
        {
            Console.CanClose = true;
            Console.Close();
            PatchCreator.Instance.OnClosing();
            base.OnClosing(e);
        }

    }
}
