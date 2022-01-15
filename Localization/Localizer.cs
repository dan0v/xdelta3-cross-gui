using System.Collections.Generic;
using System.ComponentModel;
using System.Resources;

namespace xdelta3_cross_gui.Localization
{
    class Localizer : INotifyPropertyChanged
    {
        public static readonly Dictionary<string, string> Languages = new Dictionary<string, string>
        {
            { "Deutsch", "de"},
            { "English", "en-US" },
            { "Español", "es" },
            { "Magyar", "hu" },
            { "中文(简体)", "zh-Hans" }
        };

        private const string IndexerName = "Item";
        private const string IndexerArrayName = "Item[]";
        private ResourceManager resources;

        private Localizer()
        {
        }

        public void LoadLanguage()
        {
            resources = new ResourceManager(typeof(Language));
            Invalidate();
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

        private static Localizer _instance = null;
        public static Localizer Instance => _instance ??= new Localizer();

        public event PropertyChangedEventHandler PropertyChanged;

        public void Invalidate()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(IndexerName));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(IndexerArrayName));
        }
    }
}
