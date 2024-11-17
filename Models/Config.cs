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
        private readonly JsonSerializerOptions jsonSerializerOptions = new()
        {
            WriteIndented = true,
            IncludeFields = true,
            TypeInfoResolver = JsonTypeInfoResolver.Combine(
                ConfigContext.Default
                )
        };

        public string Language { get; set; } = "English";
        private string _patchExtention { get; set; } = "vcdiff";
        private string _patchSubdirectory { get; set; } = "vcdiff";
        private string _patchFileDestination { get; set; } = "";
        private string _xDeltaArguments { get; set; } = "-B 1073741824 -e -9 -S djw -vfs";
        private string _zipName { get; set; } = "patch";
        private bool _showFullPaths { get; set; } = false;
        private bool _copyExecutables { get; set; } = true;
        private bool _zipFilesWhenDone { get; set; } = false;
        private bool _showTerminal { get; set; } = false;
        private int _maximumThreads { get; set; } = 2;

        public string PatchExtention
        {
            get => _patchExtention;
            set
            {
                if (value != _patchExtention)
                {
                    _patchExtention = value;
                    OnPropertyChanged();
                    SaveCurrent();
                }
            }
        }
        public string PatchSubdirectory
        {
            get => _patchSubdirectory;
            set
            {
                if (value != _patchSubdirectory)
                {
                    _patchSubdirectory = value;
                    OnPropertyChanged();
                    SaveCurrent();
                }
            }
        }
        public string XDeltaArguments
        {
            get => _xDeltaArguments;
            set
            {
                if (value != _xDeltaArguments)
                {
                    _xDeltaArguments = value;
                    OnPropertyChanged();
                    SaveCurrent();
                }
            }
        }

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public string PatchFileDestination
        {
            get => _patchFileDestination;
            set
            {
                if (value != _patchFileDestination)
                {
                    _patchFileDestination = value;
                    OnPropertyChanged();
                }
            }
        }
        public string ZipName
        {
            get => _zipName;
            set
            {
                if (value != _zipName)
                {
                    _zipName = value;
                    OnPropertyChanged();
                }
            }
        }
        public bool ShowFullPaths
        {
            get => _showFullPaths;
            set
            {
                if (value != _showFullPaths)
                {
                    _showFullPaths = value;
                    OnPropertyChanged();
                    SaveCurrent();
                }
            }
        }
        public bool CopyExecutables
        {
            get => _copyExecutables;
            set
            {
                if (value != _copyExecutables)
                {
                    _copyExecutables = value;
                    OnPropertyChanged();
                    SaveCurrent();
                }
            }
        }
        public bool ZipFilesWhenDone
        {
            get => _zipFilesWhenDone;
            set
            {
                if (value != _zipFilesWhenDone)
                {
                    _zipFilesWhenDone = value;
                    OnPropertyChanged();
                    SaveCurrent();
                }
            }
        }
        public bool ShowTerminal
        {
            get => _showTerminal;
            set
            {
                if (value != _showTerminal)
                {
                    _showTerminal = value;
                    OnPropertyChanged();
                }
            }
        }
        public int MaximumThreads
        {
            get => _maximumThreads;
            set
            {
                if (value != _maximumThreads)
                {
                    if (value >= 1)
                    {
                        _maximumThreads = value;
                        OnPropertyChanged();
                        SaveCurrent();
                    }
                    else
                    {
                        OnPropertyChanged();
                    }
                }
            }
        }

        public Config()
        {
            ResetToDefault();
        }

        public void LoadSaved()
        {
            try
            {
                string savedSettings = "";
                savedSettings = File.ReadAllText(Path.Combine(MainWindow.XDELTA3_APP_STORAGE, "options.json"));

                var json = JsonSerializer.Deserialize<Config>(savedSettings, jsonSerializerOptions) ?? new();

                Language = json.Language;
                PatchExtention = json.PatchExtention;
                PatchSubdirectory = json.PatchSubdirectory;
                XDeltaArguments = json.XDeltaArguments;
                ZipName = json.ZipName;
                ShowFullPaths = json.ShowFullPaths;
                CopyExecutables = json.CopyExecutables;
                ZipFilesWhenDone = json.ZipFilesWhenDone;
                MaximumThreads = json.MaximumThreads;
                ShowTerminal = json.ShowTerminal;
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
                await File.WriteAllTextAsync(Path.Combine(MainWindow.XDELTA3_APP_STORAGE, "options.json"), json);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        public void ResetToDefault()
        {
            PatchExtention = "vcdiff";
            PatchSubdirectory = "vcdiff";
            XDeltaArguments = "-B 1073741824 -e -9 -S djw -vfs";
            ZipName = "patch";
            PatchFileDestination = "";
            ShowFullPaths = false;
            CopyExecutables = true;
            ZipFilesWhenDone = false;
            MaximumThreads = 2;
            ShowTerminal = false;
        }

        public bool Validate()
        {
            bool valid = true;

            if (string.IsNullOrEmpty(PatchFileDestination))
            {
                valid = false;
            }

            if (string.IsNullOrEmpty(PatchExtention) || !IsValidFilePath(PatchExtention))
            {
                PatchExtention = "vcdiff";
                valid = false;
            }
            if (string.IsNullOrEmpty(PatchSubdirectory) || !IsValidFilePath(PatchSubdirectory))
            {
                PatchSubdirectory = "vcdiff";
                valid = false;
            }
            if (ZipFilesWhenDone && (string.IsNullOrEmpty(ZipName) || !IsValidFilePath(ZipName)))
            {
                ZipName = "patch";
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
