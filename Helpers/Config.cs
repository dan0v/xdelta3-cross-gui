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

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace xdelta3_cross_gui
{
    [JsonSerializable(typeof(Config))]
    public partial class ConfigContext : JsonSerializerContext { }
    public class Config : INotifyPropertyChanged
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        private readonly JsonSerializerOptions jsonSerializerOptions = new() {
            WriteIndented = true,
            IncludeFields = true,
            TypeInfoResolver = JsonTypeInfoResolver.Combine(
                ConfigContext.Default
                )
        };

        public string Language { get; set; } = "English";
        private string _PatchExtention { get; set; } = "vcdiff";
        private string _PatchSubdirectory { get; set; } = "vcdiff";
        private string _PatchFileDestination { get; set; } = "";
        private string _XDeltaArguments { get; set; } = "-B 1073741824 -e -9 -S djw -vfs";
        private string _ZipName { get; set; } = "patch";
        private bool _ShowFullPaths { get; set; } = false;
        private bool _CopyExecutables { get; set; } = true;
        private bool _ZipFilesWhenDone { get; set; } = false;
        private bool _ShowTerminal { get; set; } = false;

        public string PatchExtention
        {
            get => _PatchExtention;
            set
            {
                if (value != _PatchExtention)
                {
                    _PatchExtention = value;
                    OnPropertyChanged();
                }
            }
        }
        public string PatchSubdirectory
        {
            get => _PatchSubdirectory;
            set
            {
                if (value != _PatchSubdirectory)
                {
                    _PatchSubdirectory = value;
                    OnPropertyChanged();
                }
            }
        }
        public string XDeltaArguments
        {
            get => _XDeltaArguments;
            set
            {
                if (value != _XDeltaArguments)
                {
                    _XDeltaArguments = value;
                    OnPropertyChanged();
                }
            }
        }
        public string PatchFileDestination
        {
            get => _PatchFileDestination;
            set
            {
                if (value != _PatchFileDestination)
                {
                    _PatchFileDestination = value;
                    OnPropertyChanged();
                }
            }
        }
        public string ZipName
        {
            get => _ZipName;
            set
            {
                if (value != _ZipName)
                {
                    _ZipName = value;
                    OnPropertyChanged();
                }
            }
        }
        public bool ShowFullPaths
        {
            get => _ShowFullPaths;
            set
            {
                if (value != _ShowFullPaths)
                {
                    _ShowFullPaths = value;
                    OnPropertyChanged();
                }
            }
        }
        public bool CopyExecutables
        {
            get => _CopyExecutables;
            set
            {
                if (value != _CopyExecutables)
                {
                    _CopyExecutables = value;
                    OnPropertyChanged();
                }
            }
        }
        public bool ZipFilesWhenDone
        {
            get => _ZipFilesWhenDone;
            set
            {
                if (value != _ZipFilesWhenDone)
                {
                    _ZipFilesWhenDone = value;
                    OnPropertyChanged();
                }
            }
        }
        public bool ShowTerminal
        {
            get => _ShowTerminal;
            set
            {
                if (value != _ShowTerminal)
                {
                    _ShowTerminal = value;
                    OnPropertyChanged();
                }
            }
        }
        public Config()
        {
            this.ResetToDefault();
        }

        public void LoadSaved()
        {
            try
            {
                string savedSettings = "";
                savedSettings = File.ReadAllText(Path.Combine(MainWindow.XDELTA3_APP_STORAGE, "options.json"));

                var json = JsonSerializer.Deserialize<Config>(savedSettings, jsonSerializerOptions) ?? new();

                this.Language = json.Language;
                this.PatchExtention = json.PatchExtention;
                this.PatchSubdirectory = json.PatchSubdirectory;
                this.PatchFileDestination = json.PatchFileDestination;
                this.XDeltaArguments = json.XDeltaArguments;
                this.ZipName = json.ZipName;
                this.ShowFullPaths = json.ShowFullPaths;
                this.CopyExecutables = json.CopyExecutables;
                this.ZipFilesWhenDone = json.ZipFilesWhenDone;
                this.ShowTerminal = json.ShowTerminal;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Failed to load saved config\n" + e);
            }
        }

        public async void SaveCurrent()
        {
            string json = JsonSerializer.Serialize(this, jsonSerializerOptions);

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
                await File.WriteAllTextAsync(Path.Combine(MainWindow.XDELTA3_APP_STORAGE, "options.json"), json);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        public void ResetToDefault()
        {
            this.PatchExtention = "vcdiff";
            this.PatchSubdirectory = "vcdiff";
            this.XDeltaArguments = "-B 1073741824 -e -9 -S djw -vfs";
            this.ZipName = "patch";
            this.PatchFileDestination = "";
            this.ShowFullPaths = false;
            this.CopyExecutables = true;
            this.ZipFilesWhenDone = false;
            this.ShowTerminal = false;
        }

        public bool Validate()
        {
            bool valid = true;

            if (string.IsNullOrEmpty(this.PatchFileDestination))
            {
                valid = false;
            }

            if (string.IsNullOrEmpty(this.PatchExtention) || !IsValidFilePath(this.PatchExtention))
            {
                this.PatchExtention = "vcdiff";
                valid = false;
            }
            if (string.IsNullOrEmpty(this.PatchSubdirectory) || !IsValidFilePath(this.PatchSubdirectory))
            {
                this.PatchSubdirectory = "vcdiff";
                valid = false;
            }
            if (this.ZipFilesWhenDone && (string.IsNullOrEmpty(this.ZipName) || !IsValidFilePath(this.ZipName)))
            {
                this.ZipName = "patch";
                valid = false;
            }

            return valid;
        }

        private static bool IsValidFilePath(string path)
        {
            bool valid = true;
            foreach (char a in Path.GetInvalidFileNameChars())
            {
                if (path.Contains(a))
                {
                    valid = false;
                }
            }
            foreach (char a in Path.GetInvalidPathChars())
            {
                if (path.Contains(a))
                {
                    valid = false;
                }
            }
            return valid;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
