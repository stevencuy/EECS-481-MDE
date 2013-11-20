using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Speech.Synthesis;
using System.Media;
using System.ComponentModel;


namespace SETKeyboard.GUI
{
    class Output
    {
        //private string destination_folder = "C:\\Users\\Jason\\Desktop\\SET\\"; // this only works if directory exists
        private string destination_folder = Directory.GetCurrentDirectory();
        private MainWindow window;
        private List<OutputButton> outputKeys = new List<OutputButton>();
        private SpeechSynthesizer speech;
        private BackgroundWorker TTS_bworker = new BackgroundWorker();
        private BackgroundWorker TTF_bworker = new BackgroundWorker();
        private BackgroundWorker TTC_bworker = new BackgroundWorker();
        public Output(MainWindow window, double height, double width)
        {
            this.window = window;
            speech = new SpeechSynthesizer();

            string[] outputStrings = new string[3];
			outputStrings[0] = "Speak";
            outputStrings[1] = "Print Console to Text File";
            outputStrings[2] = "Copy Console to Clipboard";
			
            int button_width = (int)(width / 3) * 2;
            int margin = (int)height / 22;
            int button_height = (int)(height / 4) - margin;
			/*
            int margin = 5;
            int button_width = (int)((width - margin * 4) / 3);
            int button_height = (int)((width - margin * 2));
			*/
            OutputButton speak = new OutputButton(outputStrings[0], button_width, button_height, 0, margin * 2, 0, 0);
            //OutputButton speak = new OutputButton(outputStrings[0], button_width, button_height, margin, margin, 0, 0);
            outputKeys.Add(speak);
            window.OUTPUTGrid.Children.Add(speak);
            speak.Click += new RoutedEventHandler(Speak_Click);

            OutputButton ttf = new OutputButton(outputStrings[1], button_width, button_height, 0, button_height + margin * 3, 0, 0);
            //OutputButton ttf = new OutputButton(outputStrings[1], button_width, button_height, button_width + margin * 2, margin, 0, 0);
            outputKeys.Add(ttf);
            window.OUTPUTGrid.Children.Add(ttf);
            ttf.Click += new RoutedEventHandler(TTF_Click);

            OutputButton ttc = new OutputButton(outputStrings[2], button_width, button_height, 0, button_height * 2 + margin * 4, 0, 0);
            outputKeys.Add(ttc);
            window.OUTPUTGrid.Children.Add(ttc);
            ttc.Click += new RoutedEventHandler(TTC_Click);

            TTS_BWorker_Init();
            TTF_BWorker_Init();
            TTC_BWorker_Init();

            /*add lock button
            ToggleButton lockButton = new ToggleButton();
            lockButton.Height = button_height;
            lockButton.Width = (3 * button_width) / 2;
            lockButton.Margin = new Thickness(0, button_height * 3, 0, 0);
            lockButton.Content = "Lock";
            lockButton.VerticalAlignment = VerticalAlignment.Top;
            lockButton.HorizontalAlignment = HorizontalAlignment.Left;
            window.QWERTYGrid.Children.Add(lockButton);
            lockButton.Name = "lockButton";
			*/
            
        }


        private void Speak_Click(object sender, RoutedEventArgs e)
        {
            outputKeys[0].Content = "Speaking...";
            outputKeys[0].IsEnabled = false;
			TextRange tr = new TextRange(window.SETConsole.Document.ContentStart, window.SETConsole.Document.ContentEnd);
            TTS_bworker.RunWorkerAsync();
        }
        // Uses the Intel SDK good-sounding, prone to double-clicks TTS voice
        /*private void Intel_Speak(string msg)
        {
            outputKeys[0].IsEnabled = false;
        }*/
        private void TTF_Click(object sender, RoutedEventArgs e)
        {
            outputKeys[1].IsEnabled = false;
            if(!Directory.Exists(destination_folder))
            {
                outputKeys[1].Content = "Directory does not exist!\nPlease check directory settings in options menu";
                outputKeys[1].Background = Brushes.Crimson;
            }
            else
            {
                TextRange tr = new TextRange(window.SETConsole.Document.ContentStart, window.SETConsole.Document.ContentEnd);
                outputKeys[1].Content = "Working...";
                outputKeys[1].Background = Brushes.MediumSpringGreen;
                string date = DateTime.Now.ToString("yyyy-MM-dd-HH-mm") + "-output";
                string index = "";
                int i = 0;
                while(File.Exists(@destination_folder + date + index + ".txt"))
                {
                    i++;
                    index = "-" + i;
                }
                
                System.IO.File.WriteAllText(@destination_folder + date + index + ".txt", tr.Text);
                //System.IO.File.WriteAllText(@destination_folder + "test.txt", tr.Text);
                outputKeys[1].Content = "File successfully written!";
                outputKeys[1].Background = Brushes.LightGray;
                TTF_bworker.RunWorkerAsync();
            }
        }

        private void TTC_Click(object sender, RoutedEventArgs e)
        {
            outputKeys[2].IsEnabled = false;
            TextRange tr = new TextRange(window.SETConsole.Document.ContentStart, window.SETConsole.Document.ContentEnd);
            string content = ((string)tr.Text).Substring(0, ((string)tr.Text).Length - 2) + '\0';
            Clipboard.SetText(content);
            outputKeys[2].Content = "Copied!";
			window.FocusCaret();
            TTC_bworker.RunWorkerAsync();
        }
        
        private void TTS_BWorker_Init()
        {
            TTS_bworker.DoWork += new DoWorkEventHandler(TTS_BWorker_Work);
            TTS_bworker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(TTS_BWorker_Complete);
        }

        private void TTS_BWorker_Work(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            TextRange tr = new TextRange(window.SETConsole.Document.ContentStart, window.SETConsole.Document.ContentEnd);
            Voice.Synthesis.Speak(tr.Text);
        }

        private void TTS_BWorker_Complete(object sender, RunWorkerCompletedEventArgs e)
        {
            outputKeys[0].IsEnabled = true;
            outputKeys[0].Background = Brushes.LightGray;
            outputKeys[0].Content = "Speak";
        }

        private void TTF_BWorker_Init()
        {
            TTF_bworker.DoWork += new DoWorkEventHandler(Sleep_BWorker_Work);
            TTF_bworker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(TTF_BWorker_Complete);
        }

        private void TTC_BWorker_Init()
        {
            TTC_bworker.DoWork += new DoWorkEventHandler(Sleep_BWorker_Work);
            TTC_bworker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(TTC_BWorker_Complete);
        }

        private void Sleep_BWorker_Work(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            System.Threading.Thread.Sleep(1500);
        }

        private void TTF_BWorker_Complete(object sender, RunWorkerCompletedEventArgs e)
        {
            outputKeys[1].IsEnabled = true;
            outputKeys[1].Background = Brushes.LightGray;
            outputKeys[1].Content = "Print Console to Text File";
        }
        private void TTC_BWorker_Complete(object sender, RunWorkerCompletedEventArgs e)
        {
            outputKeys[2].IsEnabled = true;
            outputKeys[2].Background = Brushes.LightGray;
            outputKeys[2].Content = "Copy Console to Clipboard";
        }

        /* Uses the Microsoft Sam GUI-friendly, garbage-sounding TTS voice
        private void Microsoft_Speak(string msg)
        {
            speech.Dispose();
            if (msg != "")
            {
                outputKeys[0].IsEnabled = false;
                outputKeys[0].Content = "Speaking...";
                outputKeys[0].Background = Brushes.MediumSpringGreen;
                speech = new SpeechSynthesizer();
                //speech.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Child);
                speech.SpeakAsync(msg);
                speech.SpeakCompleted += new EventHandler<SpeakCompletedEventArgs>(Speech_Completed);
            }
        }
        */

        /*
        private void Speech_Completed(object sender, EventArgs e)
        {
            outputKeys[0].Background = Brushes.LightGray;
            outputKeys[0].Content = "Speak";
            //outputKeys[0].IsEnabled = true;
        }
        */

    }
}
