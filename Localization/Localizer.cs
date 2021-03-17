using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Text;

namespace xdelta3_cross_gui.Localization
{
    class Localizer : INotifyPropertyChanged
    {
        public static readonly Dictionary<string, string> Languages = new Dictionary<string, string>
        {
            { "Deutsch", "de"},
            { "English", "en-US" },
            { "Español", "es" },
            { "Magyar", "hu" }
        };

        private const string IndexerName = "Item";
        private const string IndexerArrayName = "Item[]";
        private ResourceManager resources;

        public Localizer()
        {
        }

        public bool LoadLanguage()
        {
            resources = new ResourceManager(typeof(Language));
            Invalidate();
            return true;
        }

        public string Language { get; private set; }

        public string this[string key]
        {
            get
            {
                string ret = resources.GetString(key).Replace(@"\\n", "\n") ?? $"Missing:{key}";
                return ret;
            }
        }

        public static Localizer Instance { get; set; } = new Localizer();
        public event PropertyChangedEventHandler PropertyChanged;

        public void Invalidate()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(IndexerName));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(IndexerArrayName));
        }
    }
}
