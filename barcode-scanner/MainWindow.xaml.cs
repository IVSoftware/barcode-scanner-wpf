using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace barcode_scanner
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            PreviewTextInput += (sender, e) =>
            {
                foreach (char ch in e.Text)
                {
                    detectScan(ch);
                }
            };
        }
        private void detectScan(char @char)
        {
            if (_keyCount == 0) _buffer.Clear();
            int charCountCapture = ++_keyCount;
            _buffer.Append(@char);
            Task
                .Delay(TimeSpan.FromSeconds(SECONDS_PER_CHARACTER_MIN_PERIOD))
                .GetAwaiter()
                .OnCompleted(() =>
                {
                    if (charCountCapture.Equals(_keyCount))
                    {
                        _keyCount = 0;
                        if (_buffer.Length > SCAN_MIN_LENGTH)
                        {
                            Application.Current.Dispatcher.Invoke(() => 
                            {
                                var barcode = _buffer.ToString().Trim();
                                // Optional remove from currently focused textbox.
                                if(Keyboard.FocusedElement is TextBox textBox && textBox.Text.Contains(barcode)) 
                                {
                                    textBox.Text = textBox.Text.Replace(barcode, string.Empty);
                                }
                                MessageBox.Show(barcode);
                            });
                        }
                    }
                });
        }
        private readonly StringBuilder _buffer = new StringBuilder();
        int _keyCount = 0;
        const int SCAN_MIN_LENGTH = 8;
        const double SECONDS_PER_CHARACTER_MIN_PERIOD = 0.1;
    }
}