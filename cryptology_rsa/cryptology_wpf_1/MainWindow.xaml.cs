using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.IO;
using Microsoft.Win32;

namespace cryptology_wpf_1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string currentFileName;
        const string uaAlphabet = "АБВГҐДЕЄЖЗИІЇЙКЛМНОПРСТУФХЦЧШЩЬЮЯ ";
        const string enAlphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ ";
        int p, q, n, e_, d,euler;
        public MainWindow()
        {
            InitializeComponent();
        }

        public enum Direction { Left, Right }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        private static Dictionary<char, int> Alphabet { get; set; }
        private static void GenerateAlphabet(string alp)
        {
            Alphabet = new Dictionary<char, int>();
            char[] alphabet = alp.ToCharArray();
            for (int i = 0; i < alphabet.Length; ++i)
            {
                Alphabet.Add(alphabet[i], i);
            }
        }
        //encryption
        private void EncryptButton_Click(object sender, RoutedEventArgs e)
        {
            p = int.Parse(pField.Text);
            q = int.Parse(qField.Text);
            n = p * q;
            euler = (p - 1) * (q - 1);
            e_ = int.Parse(eField.Text);
            for (int i = 2; ; i++)
            {
                if ((i*e_)%euler==1)
                {
                    d = i;
                    break;
                }
            }
            publicKeyFiled.Text = $"({e_},{n})"; 
            privateKeyFiled.Text = $"({d},{n})";
            if ((bool)ukrainianCheckBox.IsChecked)
            {
                GenerateAlphabet(uaAlphabet);
            }
            else if ((bool)englishCheckBox.IsChecked)
            {
                GenerateAlphabet(enAlphabet);

            }
            string rawText = rawTextBox.Text.ToUpper();
            string result = Encode(rawText, e_, n);
            finalTextBox.Text = result;

        }
        private static string Encode(string message, int E, int n)
        {
            string result = String.Empty;
            foreach (char symbol in message)
            {
                int num = Alphabet[symbol];
                result += Math.Pow(num, E) % n + " ";
            }
            result = result.Substring(0, result.Length - 1);
            return result;
        }

        public void WriteToFile(string fileName)
        {
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                writer.WriteLine(finalTextBox.Text);
            }
        }

        //decryption       
        private void DecryptButton_Click(object sender, RoutedEventArgs e)
        {
            p = int.Parse(pField.Text);
            q = int.Parse(qField.Text);
            n = p * q;
            euler = (p - 1) * (q - 1);
            e_ = int.Parse(eField.Text);
            for (int i = 2; ; i++)
            {
                if ((i * e_) % euler == 1)
                {
                    d = i;
                    break;
                }
            }
            publicKeyFiled.Text = $"({e_},{n})";
            privateKeyFiled.Text = $"({d},{n})";
            if ((bool)ukrainianCheckBox.IsChecked)
            {
                GenerateAlphabet(uaAlphabet);
            }
            else
            {
                GenerateAlphabet(enAlphabet);

            }
            string firstText = rawTextBox.Text.ToString();
            string decoded = Decode(firstText, d, n);
            finalTextBox.Text = decoded.ToLower();

        }
        private static string Decode(string message, int D, int n)
        {
            string result = String.Empty;
            string[] digits = message.Split(' ');
            for (int i = 0; i < digits.Length; ++i)
            {
                int d = Convert.ToInt32(digits[i]);
                int r = (int)(Math.Pow(d, D) % n);
                result += Alphabet.FirstOrDefault(x => x.Value == r).Key;
            }
            return result;
        }

        //other buttons
        private void CreateFileButton_Click(object sender, RoutedEventArgs e)
        {
            currentFileName = null;
            rawTextBox.Text = String.Empty;
            finalTextBox.Text = String.Empty;
        }
        private void SaveFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentFileName == null)
            {
                currentFileName = "../../resultFile.txt";
            }
            WriteToFile(currentFileName);
            MessageBox.Show($"{currentFileName}\nSuccessfully saved!");
        }

        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
                rawTextBox.Text = File.ReadAllText(openFileDialog.FileName);
            currentFileName = openFileDialog.FileName;
        }
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("This text Caesar Cipher encryption application was created by Ksenia Klakovych.");
        }

        private void PrintFileButton_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            printDialog.ShowDialog();
        }
        //Vigenere
        //генерування повторюваного пароля

        private void SwitchTextButton_Click(object sender, RoutedEventArgs e)
        {
            string first = rawTextBox.Text;
            rawTextBox.Text = finalTextBox.Text;
            finalTextBox.Text = first;
        }
    }
}
