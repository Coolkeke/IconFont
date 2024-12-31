using LayUI.Wpf.Global;
using LayUI.Wpf.Tools;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace IconFont.Shell.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "字体图标查看器";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        private FontFamily _FontFamily;
        public FontFamily FontFamily
        {
            get { return _FontFamily; }
            set { SetProperty(ref _FontFamily, value); }
        }
        public MainWindowViewModel()
        {
        }
        private bool _IsActive;
        public bool IsActive
        {
            get { return _IsActive; }
            set { SetProperty(ref _IsActive, value); }
        }
        private Dictionary<string, string> _Items;
        public Dictionary<string, string> Items
        {
            get { return _Items; }
            set { SetProperty(ref _Items, value); }
        }
        private DelegateCommand<string> _CopyCommand;
        public DelegateCommand<string> CopyCommand =>
            _CopyCommand ?? (_CopyCommand = new DelegateCommand<string>(ExecuteCopyCommand));

        void ExecuteCopyCommand(string value)
        {
            Clipboard.SetDataObject($"<TextBlock FontFamily=\"{FontFamily.Source}\" Text=\"{value}\" />");
            LayMessage.Success($"复制成功", "Root");
        }
        private Task<Dictionary<string, string>> GetIconsAsync(GlyphTypeface glyphTypeface)
        {
            return Task.Run(() =>
            {
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                foreach (KeyValuePair<int, ushort> item in glyphTypeface.CharacterToGlyphMap)
                {
                    string text = char.ConvertFromUtf32(item.Key);

                    if (!dictionary.ContainsKey(text))
                    {
                        dictionary.Add(text, "&#x" + item.Key.ToString("x4") + ";");
                    }
                }
                return dictionary;
            });
        }
        private string _FileName;
        public string FileName
        {
            get { return _FileName; }
            set { SetProperty(ref _FileName, value); }
        }
        private DelegateCommand _ReadCommand;
        public DelegateCommand ReadCommand =>
            _ReadCommand ?? (_ReadCommand = new DelegateCommand(ExecuteReadCommand));

        async void ExecuteReadCommand()
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Font files (*.ttf)|*.ttf"; // Add filter to specify only font files
                openFileDialog.Multiselect = false;
                if (openFileDialog.ShowDialog() != true) return;
                IsActive = true;
                // 获取字体文件路径
                FileName = openFileDialog.FileName;
                var glyphTypeface = new GlyphTypeface(new Uri(FileName, UriKind.Absolute));
                // 获取字体名
                var fontName = glyphTypeface.FamilyNames.Values.FirstOrDefault();
                var fontUri = new Uri(FileName, UriKind.Absolute);
                FontFamily = new FontFamily($"{fontUri}#{fontName}");
                Items = await GetIconsAsync(glyphTypeface);
            }
            catch (Exception ex)
            {
                LayMessage.Error($" {ex.Message}", "Root");
            }
            finally
            {
                IsActive = false;
            }

        }
        internal void Loaded()
        {
        }
    }
}
