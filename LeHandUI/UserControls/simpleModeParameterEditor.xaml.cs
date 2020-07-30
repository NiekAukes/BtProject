using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace LeHandUI
{

    public partial class simpleModeParameterEditor : UserControl
    {
        public DockPanel ActiveControl = null;
        public simpleModeParameterEditor()
        {
            
            InitializeComponent();
            varChooser.ItemsSource = LuaParser.varnames;
            ActiveControl = KeyPressInput;
        }

        #region Callbacks
        private static object LowerValueCoerceValueCallback(DependencyObject target, object valueObject)
        {
            simpleModeParameterEditor targetSlider = (simpleModeParameterEditor)target;
            double value = (double)valueObject;

            return Math.Min(value, targetSlider.UpperValue);
        }

        private static object UpperValueCoerceValueCallback(DependencyObject target, object valueObject)
        {
            simpleModeParameterEditor targetSlider = (simpleModeParameterEditor)target;
            double value = (double)valueObject;

            return Math.Max(value, targetSlider.LowerValue);
        }
        #endregion
        #region dependecyProperties
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
            DependencyProperty.Register("Ticks", typeof(DoubleCollection), typeof(simpleModeParameterEditor), new UIPropertyMetadata(null));
        #endregion
        #region Value classes
        public double Minimum
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
        }
        #endregion

        private void control_loaded(object sender, RoutedEventArgs e)
        {
            lowerSlider.Minimum = 0;                upperSlider.Minimum = 0;
            lowerSlider.Maximum = 100;              upperSlider.Maximum = 100;
            lowerSlider.TickFrequency = 2;          upperSlider.TickFrequency = 2;
            lowerSlider.IsSnapToTickEnabled = true; upperSlider.IsSnapToTickEnabled = true;

            lowerSlider.Value = 20;
            upperSlider.Value = 80;

        }

        private void lowerSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void valueofsliderChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double lowerval = lowerSlider.Value; double upperval = upperSlider.Value;
            if(lowerval >= upperval)
            {
                upperSlider.Value++;
            }
            if(upperval <= lowerval)
            {
                lowerSlider.Value--;
            }

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
            Panel.SetZIndex(ActiveControl, 99);
        }

        private void MousePressChooser_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
