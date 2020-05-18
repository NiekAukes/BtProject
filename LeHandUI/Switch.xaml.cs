using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

//Colors: #FFB01D1D and #FF42A720
//RGB code (176,29,29) and (66,167,32)

namespace LeHandUI
{
    partial class Switch : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _name2;

        public string Name2
        {
            get { return _name2; }
            set
            {
                if (value != _name2)
                {
                    _name2 = value;
                    OnPropertyChanged("Name2");
                }
            }
        }
        private string _name1;

        public string Name1
        {
            get { return _name1; }
            set
            {
                if (value != _name1)
                {
                    _name1 = value;
                    OnPropertyChanged("Name1");
                }
            }
        }
    }
    public partial class Switch : UserControl
    {
        public bool state = false;
        int beginValue = 10;
        int endValue = 288;
        Duration timeSpan = new Duration(TimeSpan.FromMilliseconds(750));
        public static DoubleAnimation rectRight = new DoubleAnimation();
        public static DoubleAnimation rectLeft = new DoubleAnimation();
        public static ColorAnimation rectTurnGreen = new ColorAnimation();
        public static ColorAnimation rectTurnRed = new ColorAnimation();

        //public string Name1 { get; set; }
        //public string Name2 { get; set; }

        void DoRectToRight()
        {
            rectRight.From = beginValue;
            rectRight.To = endValue;
            rectRight.Duration = timeSpan;
            rectRight.EasingFunction = new QuarticEase();
            SliderRect.BeginAnimation(TranslateTransform.XProperty,rectRight);

            rectTurnRed.From = Color.FromRgb(66,167,32);
            rectTurnRed.To = Color.FromRgb(176,29,29);
            rectTurnRed.Duration = timeSpan;
            //SliderRect.BeginAnimation(ForegroundProperty, rectTurnRed);
        }
        void DoRectToLeft()
        {
            rectLeft.From = endValue;
            rectLeft.To = beginValue;
            rectLeft.Duration = timeSpan;
            rectLeft.EasingFunction = new QuarticEase();
            SliderRect.BeginAnimation(TranslateTransform.XProperty, rectLeft);

            rectTurnGreen.From = Color.FromRgb(176,29,29);
            rectTurnGreen.To = Color.FromRgb(66,167,32);
            rectTurnGreen.Duration = timeSpan;
           // SliderRect.BeginAnimation(ForegroundProperty, rectTurnGreen);
        }
        
        public Switch()
        {
            InitializeComponent();
            /*DoubleAnimation doubleanimation1 = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(50)));
            SliderRect.BeginAnimation(OpacityProperty, doubleanimation1);*/
            Name1 = "Simple";
            Name2 = "Advanced";
        }

        private void SliderRect_MouseDown(object sender, MouseEventArgs e)
        {
            //if state is false, it is on easy mode, and to the left. If state is true the rect is to the right and on advanced mode
            if (state == false)
            {
                DoRectToRight();
            }
            if (state == true)
            {
                DoRectToLeft();
            }
            state = !state;
        }
    }
}
