using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using Microsoft.Win32;
using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Xml;

namespace ConfigLoader
{
    public partial class MainWindow : Window
    {
        string baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        string windowsUser = Environment.UserName;
        string caminhoArquivo = $@"C:\Users\{Environment.UserName}\AppData\LocalLow\AcmoreGames\CheatersCheetah\cfg\skate.gg";

        public MainWindow()
        {
            InitializeComponent(); checkfolder(); loadavalon(); ReloadCfgs();
        }

        void loadavalon()
        {
            using (var xshd_stream = File.OpenRead(Path.Combine(Environment.CurrentDirectory, @"bin\json.xshd")))
            using (var xshd_reader = new XmlTextReader(xshd_stream))
            {
                AvalonEditor.SyntaxHighlighting = HighlightingLoader.Load(xshd_reader, HighlightingManager.Instance);
                AvalonEditor.Options.EnableEmailHyperlinks = false;
                AvalonEditor.Options.EnableHyperlinks = false;
                AvalonEditor.Options.AllowScrollBelowDocument = true;
                AvalonEditor.Options.EnableTextDragDrop = true;
                AvalonEditor.Options.EnableVirtualSpace = true;
            }
        }

        void checkfolder() => Directory.CreateDirectory("cfgs");

        public void ReloadCfgs()
        {
            cfgbox.Items.Clear();
            foreach (var ext in new[] { "*.gg", "*.cfg", "*.txt" })
                foreach (var file in new DirectoryInfo("./cfgs").GetFiles(ext))
                    cfgbox.Items.Add(file.Name);
        }

        private void apply_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Your current cfg will be replaced with the one in the editor. Continue?", "SkateManager", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                File.WriteAllText(caminhoArquivo, GetCurrent().Text);
            ReloadCfgs();
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Backup will be created. Continue?", "SkateManager", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                var save = new SaveFileDialog { Filter = "Config Files (*.gg *.cfg)|*.gg; *.cfg", Title = "Backup Config", FileName = "MyBackup", InitialDirectory = baseDir };
                if (save.ShowDialog() == true)
                    File.Copy(caminhoArquivo, save.FileName, true);
            }
            ReloadCfgs();
        }

        private void load_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Your cfg will reset to default. Continue?", "SkateManager", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                File.WriteAllText(caminhoArquivo, "");
            ReloadCfgs();
        }

        public ICSharpCode.AvalonEdit.TextEditor GetCurrent() => AvalonEditor;

        private void save_Click(object sender, RoutedEventArgs e)
        {
            var save = new SaveFileDialog { Filter = "Config Files (*.cfg *.txt)|*.cfg; *.txt", Title = "Save Config", FileName = "MyConfig", InitialDirectory = baseDir };
            if (save.ShowDialog() == true)
                File.WriteAllText(save.FileName, GetCurrent().Text);
            ReloadCfgs();
        }

        private void LoadItem_Click(object sender, RoutedEventArgs e) => LoadSelectedConfig();

        private void RefreshItem_Click(object sender, RoutedEventArgs e) => ReloadCfgs();

        private void cfgbox_MouseDoubleClick(object sender, MouseButtonEventArgs e) => LoadSelectedConfig();

        void LoadSelectedConfig()
        {
            try { GetCurrent().Text = File.ReadAllText($"./cfgs/{cfgbox.SelectedItem}"); } catch { }
            ReloadCfgs();
        }
    }
}