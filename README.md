# Barcode Scanner WPF

Since your question includes "know another way..." I'll offer what has worked for me: using the _rate_ of keystrokes to tell the difference between human typing and a much-more-rapid barcode scan and of course when to know that the scan is complete.

[![barcode scan][1]][1]

You may have to experiment with it. I only "just now" tried porting it to WPF. But the basic idea is something that I've used reliably in my Winforms app for many years with no issues. 

___

**WPF**

```
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
```


  [1]: https://i.stack.imgur.com/fUdva.png