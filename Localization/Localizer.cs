using System.Collections.Generic;
using System.ComponentModel;
using System.Resources;
using System.Runtime.CompilerServices;

namespace xdelta3_cross_gui.Localization
{
    class Localizer : INotifyPropertyChanged
    {
        public static readonly Dictionary<string, string> Languages = new()
        {
            { "Deutsch", "de"},
            { "English", "en-US" },
            { "Español", "es" },
            { "Magyar", "hu" },
            { "中文(简体)", "zh-Hans" },
            { "日本語", "ja" }
        };

        private const string IndexerName = "Item";
        private const string IndexerArrayName = "Item[]";
        private ResourceManager? resources;

        private Localizer()
        {
        }

        public void LoadLanguage()
        {
            resources = new ResourceManager(typeof(Language));
            Invalidate();
        }

        public string this[string key]
        {
            get
            {
                return resources?.GetString(key)?.Replace(@"\\n", "\n") ?? $"Missing:{key}";
            }
        }

        private static Localizer? _instance;
        public static Localizer Instance => _instance ??= new Localizer();

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public void Invalidate()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(IndexerName));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(IndexerArrayName));
        }
    }
}
