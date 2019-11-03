using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Diagnostics;
using System.Globalization;
using Google.Apis.Auth.OAuth2;
using System.IO;
using Google.Cloud.TextToSpeech.V1;

namespace VoiceControl
{
    public partial class Form1 : Form
    {
        SpeechRecognitionEngine speechRecongnitionEngine = new SpeechRecognitionEngine();
        SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer();
        string[] commands_array = new string[] { "open google chrome", "open chrome", "exit google chrome", "Good bye", "bye bye", "Exit your self", "Stop listening", "Stop listening to me", "hi how are you"};

        Dictionary<string, bool> recognized = new Dictionary<string, bool>();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Choices commands = new Choices();
            commands.Add(commands_array);
            GrammarBuilder grammarBuilder = new GrammarBuilder();
            grammarBuilder.Append(commands);
            grammarBuilder.Culture = CultureInfo.CurrentCulture;
            string grammerPhrase = grammarBuilder.DebugShowPhrases;

            Grammar grammar = new Grammar(grammarBuilder);
            speechRecongnitionEngine.LoadGrammarAsync(grammar);
            speechRecongnitionEngine.SetInputToDefaultAudioDevice();

            speechRecongnitionEngine.SpeechRecognized += SpeechRecongnitionEngine_SpeechRecognized;
            speechRecongnitionEngine.SpeechRecognitionRejected += SpeechRecongnitionEngine_SpeechRecognitionRejected;
            speechRecongnitionEngine.RecognizeAsync(RecognizeMode.Multiple);

            //speechSynthesizer.SetOutputToWaveFile("sample_file.wav");
        }

        private void SpeechRecongnitionEngine_SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        { 
            float confidence = e.Result.Confidence;
            string command = e.Result.Text.ToLower().Trim();
            if (String.IsNullOrEmpty(command))
            {
                return;
            }
            if (recognized.ContainsKey(command))
            {
                return;
            }
            if (confidence < 0.5)
            {
                return;
            }
            switch (command)
            {
                case "hi how are you":
                    speechSynthesizer.SpeakAsync("I am fine, how are you ?");
                    recognized.Add(command, true);
                    break;
                case "open google chrome":
                case "open chrome":
                    speechSynthesizer.SpeakAsync("Opening Chrome");
                    Process.Start("chrome.exe", "http://www.google.com");
                    recognized.Add(command, true);
                    break;
                case "exit google chrome":
                case "exit chrome":
                    speechSynthesizer.SpeakAsync("Exiting Chrome!");
                    recognized.Add(command, true);
                    break;
                case "good bye":
                case "bye bye":
                case "exit your self":
                    speechSynthesizer.Speak("Exiting now");
                    Environment.Exit(0);
                    recognized.Add(command, true);
                    break;
                case "stop listening":
                case "stop listening to me":
                    speechSynthesizer.Speak("Okay I will not listen to you now");
                    speechRecongnitionEngine.RecognizeAsyncStop();
                    recognized.Add(command, true);
                    break;
                default:
                    //MessageBox.Show("Command unrecognized, Please try again!");
                    break;
            }
            recognized.Remove(command);
            writeLog("<command> " + e.Result.Text + " <unrecognized> <confidence> " + confidence + " <" + DateTime.Now.ToString("yyyy_MM_dd HH:mm:ss") + ">");
            //MessageBox.Show("Command unrecognized, Please try again! " + e.Result.Text + ".");
        }

        private void SpeechRecongnitionEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            float confidence = e.Result.Confidence;
            string command = e.Result.Text.ToLower().Trim();
            if (String.IsNullOrEmpty(command))
            {
                return;
            }
            if (recognized.ContainsKey(command))
            {
                return;
            }
            if (confidence < 0.5)
            {
                return;
            }
            switch (command)
            {
                case "hi how are you":
                    speechSynthesizer.SpeakAsync("I am fine, how are you ?");
                    recognized.Add(command, true);
                    break;
                case "open google chrome":
                case "open chrome":
                    speechSynthesizer.SpeakAsync("Opening Chrome");
                    Process.Start("chrome.exe", "http://www.google.com");
                    recognized.Add(command, true);
                    break;
                case "exit google chrome":
                case "exit chrome":
                    Process[] chromeInstances = Process.GetProcessesByName("chrome");
                    chromeInstances[0].Close();
                    speechSynthesizer.SpeakAsync("Exiting Chrome!");
                    recognized.Add(command, true);
                    break;
                case "good bye":
                case "bye bye":
                case "exit your self":
                    speechSynthesizer.Speak("Exiting now");
                    Environment.Exit(0);
                    recognized.Add(command, true);
                    break;
                //case "stop listening":
                //case "stop listening to me":
                //    speechSynthesizer.Speak("Okay I will not listen to you now");
                //    speechRecongnitionEngine.RecognizeAsyncStop();
                //    recognized.Add(command, true);
                //    break;
                default:
                    //MessageBox.Show("Command unrecognized, Please try again!");
                    break;
            }
            recognized.Remove(command);
            writeLog("<command> " + e.Result.Text + " <recognized> <confidence> " + confidence + " <" + DateTime.Now.ToString("yyyy_MM_dd HH:mm:ss") + ">");
        }

        public void writeLog(string log)
        {
            richTextBox1.AppendText(log + Environment.NewLine);
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
