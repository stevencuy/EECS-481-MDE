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
    public class TabReadWrite
    {
        MainWindow window;
        public String consoleText;
        private String tabs_directory;
        private const String search_pattern = "*.ctab";
        private List<String> file_names;
        public TabReadWrite(MainWindow window)
        {
            this.window = window;
            tabs_directory = Directory.GetCurrentDirectory() + "\\ctab_files";
        }

        public void Read()
        {
            file_names = new List<String>();
            if(Directory.Exists(tabs_directory))
            {
                string[] files = Directory.GetFiles(@tabs_directory, search_pattern);
                file_names = files.ToList();
                foreach (string file_name in file_names)
                {
                    readFile(file_name);
                }
            }
            
        }
        public void Write()
        {
            if(!Directory.Exists(tabs_directory))
            {
                Directory.CreateDirectory(tabs_directory);
            }
            foreach (string file_name in file_names)
            {
                File.Delete(file_name);
            }
            foreach (var pair in window.tabPhrases)
            {
                if(pair.Value.Count > 0)
                    writeFile(pair.Key);
            }
        }
        public void readFile(String file_name)
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

        public void writeFile(String name)
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
