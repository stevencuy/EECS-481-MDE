using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SETKeyboard.GUI
{
    public class ReadWrite
    {
        MainWindow window;
        public String consoleText;
        private String tabs_directory;
        private String settings_directory;
        private const String search_pattern = "*.ctab";
        private List<String> tab_file_names;
        private List<String> settings_file_names;

        private String dwellTime;
        private String backColor;
        private String hoverColor;
        private String selectColor;
        private String tabColor;
        private String tabHoverColor;
        private String tabSelectColor;
        private String consoleColor;
        private String wordColor;
        private string fontColor;

        public ReadWrite(MainWindow window)
        {
            this.window = window;
            SyncSettings(window);
            tabs_directory = Directory.GetCurrentDirectory() + "\\ctab_files";
            settings_directory = Directory.GetCurrentDirectory() + "\\settings";
        }

        public void Write()
        {
            WriteTabs();
            WriteSettings();
        }

        public void SyncSettings(MainWindow window)
        {
            dwellTime = window.dwellTime.ToString();
            backColor = window.backColor.ToString();
            hoverColor = window.hoverColor.ToString();
            selectColor = window.selectColor.ToString();
            tabColor = window.tabColor.ToString();
            tabHoverColor = window.tabColor.ToString();
            tabSelectColor = window.tabSelectColor.ToString();
            consoleColor = window.consoleColor.ToString();
            wordColor = window.wordColor.ToString();
            fontColor = window.fontColor.ToString();

            this.window = window;
        }

        public void LoadSettings()
        {
            settings_file_names = new List<String>();
            if (Directory.Exists(settings_directory))
            {
                string[] files = Directory.GetFiles(@settings_directory, "*.config");
                settings_file_names = files.ToList();
                if(settings_file_names.Count > 0)
                    ReadSettingsFile(settings_file_names[0]);
            }
           
        }

        public void ReadSettingsFile(String file_name)
        {
            using (StreamReader r = new StreamReader(file_name))
            {
                dwellTime = r.ReadLine();
                backColor = r.ReadLine();
                hoverColor = r.ReadLine();
                selectColor = r.ReadLine();
                tabColor = r.ReadLine();
                tabHoverColor = r.ReadLine();
                tabSelectColor = r.ReadLine();
                consoleColor = r.ReadLine();
                wordColor = r.ReadLine();
                fontColor = r.ReadLine();
                window.dwellTime = Convert.ToInt32(dwellTime);
                window.backColor = (SolidColorBrush)new BrushConverter().ConvertFromString(backColor);
                window.hoverColor = (SolidColorBrush)new BrushConverter().ConvertFromString(hoverColor);
                window.selectColor = (SolidColorBrush)new BrushConverter().ConvertFromString(selectColor);
                window.tabColor = (SolidColorBrush)new BrushConverter().ConvertFromString(tabColor);
                window.tabColor = (SolidColorBrush)new BrushConverter().ConvertFromString(tabHoverColor);
                window.tabSelectColor = (SolidColorBrush)new BrushConverter().ConvertFromString(tabSelectColor);
                window.consoleColor = (SolidColorBrush)new BrushConverter().ConvertFromString(consoleColor);
                window.wordColor = (SolidColorBrush)new BrushConverter().ConvertFromString(wordColor);
                window.fontColor = (SolidColorBrush)new BrushConverter().ConvertFromString(fontColor);
            }
        }
        public void WriteSettings()
        {
            if (!Directory.Exists(settings_directory))
            {
                Directory.CreateDirectory(settings_directory);
            }
            foreach (string file_name in settings_file_names)
            {
                File.Delete(file_name);
            }
            WriteSettingsFile("SETKeyboard");
        }

        public void WriteSettingsFile(String name)
        {
            String file_name = settings_directory + "\\" + name + ".config";
            using (StreamWriter writer = new StreamWriter(file_name))
            {
                writer.WriteLine(dwellTime);
                writer.WriteLine(backColor);
                writer.WriteLine(hoverColor);
                writer.WriteLine(selectColor);
                writer.WriteLine(tabColor);
                writer.WriteLine(tabHoverColor);
                writer.WriteLine(tabSelectColor);
                writer.WriteLine(consoleColor);
                writer.WriteLine(wordColor);
                writer.WriteLine(fontColor);
            }
        }

        public void ReadTabs()
        {
            tab_file_names = new List<String>();
            if(Directory.Exists(tabs_directory))
            {
                string[] files = Directory.GetFiles(@tabs_directory, search_pattern);
                tab_file_names = files.ToList();
                foreach (string file_name in tab_file_names)
                {
                    readTabFile(file_name);
                }
            }
            
        }
        public void WriteTabs()
        {
            if(!Directory.Exists(tabs_directory))
            {
                Directory.CreateDirectory(tabs_directory);
            }
            foreach (string file_name in tab_file_names)
            {
                File.Delete(file_name);
            }
            foreach (var pair in window.tabPhrases)
            {
                if(pair.Value.Count > 0)
                    writeTabFile(pair.Key);
            }
        }
        public void readTabFile(String file_name)
        {
            HashSet<String> phrases = new HashSet<String>();
            using (StreamReader r = new StreamReader(file_name))
            {
                String content = r.ReadLine();
                String tab_name = content.Substring(1, content.IndexOf('>') - 1);
                while( (content = r.ReadLine()) != null)
                {
                    phrases.Add(content);
                }
                window.tabPhrases.Add(tab_name, phrases);
            }
        }

        public void writeTabFile(String name)
        {
            String file_name = tabs_directory + "\\" + name + ".ctab";
            using (StreamWriter writer = new StreamWriter(file_name))
            {
                String header = "<" + name + ">";
                writer.WriteLine(header);
                foreach (String phrase in window.tabPhrases[name])
                {
                    writer.WriteLine(phrase);
                }
            }
        }
    }
}
