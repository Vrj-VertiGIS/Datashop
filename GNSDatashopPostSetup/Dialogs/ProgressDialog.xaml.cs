using System;

using System.Windows;
using System.Windows.Controls;

using System.Windows.Forms;

using System.Windows.Media;
using System.Windows.Media.Animation;

using ProgressBar=System.Windows.Controls.ProgressBar;

namespace GEOCOM.GNSDatashop.GNSDatashopPostSetup.Dialogs
{
  
    public partial class ProgressDialog : Dialog
    {
        public ProgressDialog()
        {
            InitializeComponent();
            ProgressBarValueThreadSafe = 0;
        }

        public override string Caption
        {
            get
            {
                return "Progress";
            }
        }

        public double ProgressBarValueThreadSafe
        {
            get
            {
                double value = 0;
                MethodInvoker readValue = delegate { value = progressBar.Value; };
                Dispatcher.Invoke(readValue);
                return value;

                
            }
            set
            {

                MethodInvoker readValue = delegate
                                              {
                                                  Duration duration = new Duration(TimeSpan.FromMilliseconds(500));
                                                  DoubleAnimation doubleanimation = new DoubleAnimation(value, duration);
                                                  progressBar.BeginAnimation(ProgressBar.ValueProperty, doubleanimation);
                                                  progressBar.Value = value;
                                              };
                Dispatcher.Invoke(readValue);
            }
        }

        public void WriteMessage(string text, params string[] args)
        {
            WriteMessage(text, false, args);
        }

        public void WriteBoldMessage(string text, params string[] args)
        {
            WriteMessage(text, true, args);
        }

        public void WriteMessage(string text, bool bold, params string[] args)
        {
            FontWeight fontWeight = (bold) ? FontWeights.Bold : FontWeights.Normal;
            CreateAndAddTextBlockThreadSafe(text, fontWeight, Brushes.Black, args);
        }

        public void WriteError(string text, params string[] args)
        {
            CreateAndAddTextBlockThreadSafe(text, FontWeights.Bold, Brushes.Red, args);
        }

        public void WriteNewLine()
        {
            WriteMessage(string.Empty, false);
         
        }

        private void CreateAndAddTextBlockThreadSafe(string text, FontWeight fontWeight, Brush foreground, params string[] args)
        {
            MethodInvoker invoker = (() => CreateAndAddTextBlock(text, fontWeight, foreground, args));
            Dispatcher.BeginInvoke(invoker, null);
        }

        private void CreateAndAddTextBlock(string format, FontWeight fontWeight, Brush foreground, params string[] args)
        {
            TextBlock message = new TextBlock();
            message.Text = string.Format(format, args);
            message.TextWrapping = TextWrapping.Wrap;
            message.FontWeight = fontWeight;
            message.Foreground = foreground;
      
            messageStack.Children.Add(message);
          //  ScrollView.ScrollToBottom();
        }

    }
}
