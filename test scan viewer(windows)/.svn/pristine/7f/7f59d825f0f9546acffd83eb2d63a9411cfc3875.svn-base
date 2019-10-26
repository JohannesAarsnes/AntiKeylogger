using System;
using System.Windows;
using System.Windows.Controls;

namespace AntiKeyloggerUI.Auxiliary.Helper
{
    class TextBoxEx : TextBox
    {
        public static readonly DependencyProperty CurrentPositionIndexProperty;
        public static readonly DependencyProperty IsControlFocusedProperty;


        public bool IsControlFocused
        {
            get { return (bool)GetValue(IsControlFocusedProperty); }
            set { SetValue(IsControlFocusedProperty, value); }
        }



        public int CurrentPositionIndex
        {
            get { return (int)GetValue(CurrentPositionIndexProperty); }
            set { SetValue(CurrentPositionIndexProperty, value); }
        }

        public TextBoxEx()
        {
            this.GotFocus += OnCustomGotFocus;
            this.LostFocus += OnCustoLostFocus;
        }

        private void OnCustoLostFocus(object sender, RoutedEventArgs e)
        {
            IsControlFocused = false;       }

        private void OnCustomGotFocus(object sender, RoutedEventArgs e)
        {
            IsControlFocused = true;
        }

        static TextBoxEx()
        {
            CurrentPositionIndexProperty = DependencyProperty.RegisterAttached("CurrentPositionIndex", typeof(int), typeof(TextBoxEx),
                                                         new UIPropertyMetadata(0, new PropertyChangedCallback(OnPositionChanged)));

            IsControlFocusedProperty = DependencyProperty.RegisterAttached("IsControlFocused", typeof(bool), typeof(TextBoxEx), 
                new UIPropertyMetadata(false));


        }

        private static void OnPositionChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            TextBox control = o as TextBox;
            if (control == null)
                return;


            control.SelectionStart = (int)e.NewValue;
        }


        private void OnFocusChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender == null || !(sender is TextBox))
                return;

            TextBox control = sender as TextBox;
            IsControlFocused = control.IsFocused;
        }

    }
}
