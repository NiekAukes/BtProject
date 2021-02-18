using Microsoft.TeamFoundation.Framework.Client;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace LeHandUI
{

    public partial class simpleModeParameterEditor : UserControl
    {
        public DockPanel ActiveControl = null;
        private int width;
        public int MouseMouseWidth {get { return width; } set { width = value; }
        }
        public simpleModeParameterEditor()
        {
            
            InitializeComponent();
            KeyPressInput.Opacity = 0; KeyPressInput.IsEnabled = false;
            MousePressInput.Opacity = 0; MousePressInput.IsEnabled = false;
            MouseMoveInput.Opacity = 0; MouseMoveInput.IsEnabled = false;

            //Waarom staat hier new Bluetooth?
            //Bluetooth bluetooth = new Bluetooth();


            varChooser.ItemsSource = LuaParser.varnames;
            ActiveControl = KeyPressInput;

            KeyPressChooser.TextChanged += KeyPressChooser_TextChanged;
        }


        private void KeyPressChooser_TextChanged(object sender, TextChangedEventArgs e)
        {
            Border target = KeyPressChooserBorder;
            if (CheckAsciiTable(((TextBox)sender).Text.ToUpper().Trim(new char[] { '\u007f', '\0' })))
            {
                target.BorderBrush = Brushes.Green;
            }
            else
            {
                target.BorderBrush = Brushes.Red;
            }
        }



        #region Callbacks
        private static object LowerValueCoerceValueCallback(DependencyObject target, object valueObject)
        {
            simpleModeParameterEditor targetSlider = (simpleModeParameterEditor)target;
            double value = (double)valueObject;

            return Math.Min(value, targetSlider.upperSlider.Value);
        }

        private static object UpperValueCoerceValueCallback(DependencyObject target, object valueObject)
        {
            simpleModeParameterEditor targetSlider = (simpleModeParameterEditor)target;
            double value = (double)valueObject;

            return Math.Max(value, targetSlider.lowerSlider.Value);
        }
        #endregion
        #region dependecyProperties
        /*
        public static readonly DependencyProperty MinimumProperty =
    DependencyProperty.Register("Minimum", typeof(double), typeof(simpleModeParameterEditor), new UIPropertyMetadata(0d));
        public static readonly DependencyProperty LowerValueProperty =
            DependencyProperty.Register("LowerValue", typeof(double), typeof(simpleModeParameterEditor), new UIPropertyMetadata(0d, null, LowerValueCoerceValueCallback));
        public static readonly DependencyProperty UpperValueProperty =
            DependencyProperty.Register("UpperValue", typeof(double), typeof(simpleModeParameterEditor), new UIPropertyMetadata(1d, null, UpperValueCoerceValueCallback));
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(simpleModeParameterEditor), new UIPropertyMetadata(1d));
        public static readonly DependencyProperty IsSnapToTickEnabledProperty =
            DependencyProperty.Register("IsSnapToTickEnabled", typeof(bool), typeof(simpleModeParameterEditor), new UIPropertyMetadata(false));
        public static readonly DependencyProperty TickFrequencyProperty =
            DependencyProperty.Register("TickFrequency", typeof(double), typeof(simpleModeParameterEditor), new UIPropertyMetadata(0.1d));
        public static readonly DependencyProperty TickPlacementProperty =
            DependencyProperty.Register("TickPlacement", typeof(TickPlacement), typeof(simpleModeParameterEditor), new UIPropertyMetadata(TickPlacement.None));
        public static readonly DependencyProperty TicksProperty =
            DependencyProperty.Register("Ticks", typeof(DoubleCollection), typeof(simpleModeParameterEditor), new UIPropertyMetadata(null));*/
        #endregion
        #region Value classes
        /*public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public double LowerValue
        {
            get { return (double)GetValue(LowerValueProperty); }
            set { SetValue(LowerValueProperty, value); }
        }

        public double UpperValue
        {
            get { return (double)GetValue(UpperValueProperty); }
            set { SetValue(UpperValueProperty, value); }
        }

        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public bool IsSnapToTickEnabled
        {
            get { return (bool)GetValue(IsSnapToTickEnabledProperty); }
            set { SetValue(IsSnapToTickEnabledProperty, value); }
        }

        public double TickFrequency
        {
            get { return (double)GetValue(TickFrequencyProperty); }
            set { SetValue(TickFrequencyProperty, value); }
        }

        public TickPlacement TickPlacement
        {
            get { return (TickPlacement)GetValue(TickPlacementProperty); }
            set { SetValue(TickPlacementProperty, value); }
        }

        public DoubleCollection Ticks
        {
            get { return (DoubleCollection)GetValue(TicksProperty); }
            set { SetValue(TicksProperty, value); }
        }*/
        #endregion

        private void control_loaded(object sender, RoutedEventArgs e)
        {
            lowerSlider.Minimum = 0;                                upperSlider.Minimum = 0;
            lowerSlider.Maximum = 100;                              upperSlider.Maximum = 100;
            lowerSlider.TickFrequency = 2;                          upperSlider.TickFrequency = 2;
            lowerSlider.TickPlacement = TickPlacement.None;  upperSlider.TickPlacement = TickPlacement.None;
            lowerSlider.IsSnapToTickEnabled = true;                 upperSlider.IsSnapToTickEnabled = true;
            lowerSlider.SmallChange = 1;                            upperSlider.SmallChange = 1;
            lowerSlider.LargeChange = 0;                            upperSlider.LargeChange = 0;
            lowerSlider.IsMoveToPointEnabled = false;               upperSlider.IsMoveToPointEnabled = false;


        }

        
        public void initializeVars(FileData data)
        {
            lowerSlider.Value = data.beginRange;
            //LowerValue = data.beginRange;
            upperSlider.Value = data.endRange;
            //UpperValue = data.endRange;
            varChooser.SelectedIndex = data.variable;
            actionChooser.SelectedIndex = data.actionId;
        }
        public bool CheckAsciiTable(string s)
        {
            return SimpleMode.ascii_table.ContainsKey(s);
        }
        public Logic ParseToLogic()
        {
            Logic logic = new Logic();
            logic.beginrange = lowerSlider.Value / 100.0;
            logic.endrange = upperSlider.Value / 100.0;
            logic.variable = varChooser.SelectedIndex;
            //actionChooser
            //keyMousePressChooser
            switch(actionChooser.SelectedIndex)
            {
                case 0:
                    logic.action = new Kpress('c');
                    break;
                case 1:
                    logic.action = new Mpress(0);
                    break;
                case 2:
                    logic.action = new MMove(0, 0);
                    break;
                case 3:
                    logic.action = new end();
                    break;
            }

            return logic;
        }

        #region Handlers for textboxes and comboboxes and shit
        private void valueofsliderChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            double lowerval = lowerSlider == null ? 0 : lowerSlider.Value; double upperval = upperSlider == null ? 0 : upperSlider.Value;
            if (lowerval >= upperval && upperSlider != null)
            {
                upperSlider.Value++;
            }
            if (upperval <= lowerval && lowerSlider != null)
            {
                lowerSlider.Value--;
            }
        }

        private void actionChooser_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cbox = (ComboBox)sender;
            ActiveControl.IsEnabled = false;
            ActiveControl.Opacity = 0;
            Panel.SetZIndex(ActiveControl, -1);
            switch (cbox.SelectedIndex)
            {
                case 0:
                    ActiveControl = KeyPressInput;
                    break;
                case 1:
                    ActiveControl = MousePressInput;
                    break;
                case 2:
                    ActiveControl = MouseMoveInput;
                    break;
                case 3:
                    ActiveControl = nulpanel;
                    break;

            }
            ActiveControl.IsEnabled = true;
            ActiveControl.Opacity = 1;
            Panel.SetZIndex(ActiveControl, 999);
        }

        private void MousePressChooser_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void MouseMoveBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int result;
            
            if (((TextBox)sender).Text != null) {
                if (!int.TryParse(((TextBox)sender).Text, out result))
                {
                    e.Handled = true;
                    _ = ((TextBox)sender).Name == MouseMoveBox1.Name ? MouseMoveBorder1.BorderBrush = Brushes.Red : MouseMoveBorder2.BorderBrush = Brushes.Red;
                }
                else
                    _ = ((TextBox)sender).Name == MouseMoveBox1.Name ? MouseMoveBorder1.BorderBrush = Brushes.Green : MouseMoveBorder2.BorderBrush = Brushes.Green;

            }
        }
        #endregion

        public class MouseMoveWidth : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

        }
        
    }
}
