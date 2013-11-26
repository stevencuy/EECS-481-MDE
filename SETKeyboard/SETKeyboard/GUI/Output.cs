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
using System.Windows.Threading;


namespace SETKeyboard.GUI
{
    class Output
    {
        //private string destination_folder = "C:\\Users\\Jason\\Desktop\\SET\\"; // this only works if directory exists
        private string destination_folder = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        private MainWindow window;
        private List<OutputButton> outputKeys = new List<OutputButton>();
        private SpeechSynthesizer speech;
        private BackgroundWorker TTS_bworker = new BackgroundWorker();
        private BackgroundWorker TTF_bworker = new BackgroundWorker();
        private BackgroundWorker TTC_bworker = new BackgroundWorker();
        private DispatcherTimer timer;
        private int dwellTime;
        public Output(MainWindow window, double height, double width)
        {
            this.window = window;
            speech = new SpeechSynthesizer();
            dwellTime = 1;
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
            speak.MouseEnter += new MouseEventHandler(Speak_Hover);

            OutputButton ttf = new OutputButton(outputStrings[1], button_width, button_height, 0, button_height + margin * 3, 0, 0);
            //OutputButton ttf = new OutputButton(outputStrings[1], button_width, button_height, button_width + margin * 2, margin, 0, 0);
            outputKeys.Add(ttf);
            window.OUTPUTGrid.Children.Add(ttf);
            ttf.MouseEnter += new MouseEventHandler(TTF_Hover);

            OutputButton ttc = new OutputButton(outputStrings[2], button_width, button_height, 0, button_height * 2 + margin * 4, 0, 0);
            outputKeys.Add(ttc);
            window.OUTPUTGrid.Children.Add(ttc);
            ttc.MouseEnter += new MouseEventHandler(TTC_Hover);

            TTS_BWorker_Init();
            TTF_BWorker_Init();
            TTC_BWorker_Init();

        }


        private void Speak_Hover(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(dwellTime);

            OutputButton outputKey = (OutputButton)sender;

            timer.Tick += (sender2, eventArgs) =>
            {
                timer.Stop();
                outputKeys[0].Background = Brushes.MediumSpringGreen;
                outputKeys[0].Content = "Speaking...";
                outputKeys[0].IsEnabled = false;
                TextRange tr = new TextRange(window.SETConsole.Document.ContentStart, window.SETConsole.Document.ContentEnd);
                TTS_bworker.RunWorkerAsync();

                Speak_Hover(sender, e);
            };

            outputKey.MouseLeave += (s, e2) =>
            {
                timer.Stop();
            };

            timer.Start();
        }

        private void TTF_Hover(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(dwellTime);

            OutputButton outputKey = (OutputButton)sender;
            timer.Tick += (sender2, eventArgs) =>
            {
                timer.Stop();

                outputKeys[1].IsEnabled = false;
                if (!Directory.Exists(destination_folder))
                {
                    outputKeys[1].Content = "Error: Directory does not exist";
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
                    while (File.Exists(@destination_folder + date + index + ".txt"))
                    {
                        i++;
                        index = "-" + i;
                    }

                    System.IO.File.WriteAllText(@destination_folder + date + index + ".txt", tr.Text);
                    //System.IO.File.WriteAllText(@destination_folder + "test.txt", tr.Text);
                    outputKeys[1].Content = "File successfully written!";
                    outputKeys[1].Background = Brushes.MediumSpringGreen;
                    TTF_bworker.RunWorkerAsync();
                }

                TTF_Hover(sender, e);

            };

            outputKey.MouseLeave += (s, e2) =>
            {
                timer.Stop();
            };

            timer.Start();
        }

        private void TTC_Hover(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(dwellTime);

            OutputButton outputKey = (OutputButton)sender;

            timer.Tick += (sender2, eventArgs) =>
            {
                timer.Stop();

                outputKeys[2].IsEnabled = false;
                TextRange tr = new TextRange(window.SETConsole.Document.ContentStart, window.SETConsole.Document.ContentEnd);
                string content = ((string)tr.Text).Substring(0, ((string)tr.Text).Length - 2) + '\0';
                Clipboard.SetText(content);
                outputKeys[2].Background = Brushes.MediumSpringGreen;
                outputKeys[2].Content = "Copied!";
                window.FocusCaret();
                TTC_bworker.RunWorkerAsync();

                TTC_Hover(sender, e);
            };

            outputKey.MouseLeave += (s, e2) =>
            {
                timer.Stop();
            };

            timer.Start();
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
