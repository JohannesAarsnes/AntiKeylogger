using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace AntiKeyloggerUI.Auxiliary
{
    /// <summary>
    /// Класс поддержки событий, генерируемых при закрытии окна
    /// </summary>
    public sealed class WindowClosingBehavior
    {
        static WindowClosingBehavior()
        {
            ClosedProperty = DependencyProperty.RegisterAttached(
                "Closed", typeof(ICommand), typeof(WindowClosingBehavior), new UIPropertyMetadata(OnClosedChanged));
            ClosingProperty = DependencyProperty.RegisterAttached(
                "Closing", typeof(ICommand), typeof(WindowClosingBehavior), new UIPropertyMetadata(OnClosingChanged));
        }

        public static readonly DependencyProperty ClosedProperty;
        public static readonly DependencyProperty ClosingProperty;

        #region Get\Set methods of attached properties

        public static ICommand GetClosed(DependencyObject element)
        {
            return (ICommand)element.GetValue(ClosedProperty);
        }

        public static void SetClosed(DependencyObject element, ICommand value)
        {
            element.SetValue(ClosedProperty, value);
        }

        public static ICommand GetClosing(DependencyObject element)
        {
            return (ICommand)element.GetValue(ClosingProperty);
        }

        public static void SetClosing(DependencyObject element, ICommand value)
        {
            element.SetValue(ClosingProperty, value);
        }

        #endregion


        private static void OnClosedChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
        {
            Window window = element as Window;
            if (window != null)
            {
                if (args.NewValue != null)
                    window.Closed += WindowClosed;
                else
                    window.Closed -= WindowClosed;
            }
        }

        private static void OnClosingChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
        {
            Window window = element as Window;
            if (window != null)
            {
                if (args.NewValue != null)
                    window.Closing += WindowClosing;
                else
                    window.Closing -= WindowClosing;
            }
        }

        /// <summary>
        /// Работа с командой Closed при событии Closed сопровождаемого окна
        /// </summary>
        private static void WindowClosed(object sender, EventArgs args)
        {
            ICommand closedCommand = GetClosed((DependencyObject)sender);
            if (closedCommand != null)
                closedCommand.Execute(null);
        }

        /// <summary>
        /// Работа с командой Closing при событии Closing сопровождаемого окна
        /// </summary>
        private static void WindowClosing(object sender, CancelEventArgs args)
        {
            ICommand closingCommand = GetClosing((DependencyObject)sender);
            if (closingCommand != null)
            {
                if (closingCommand.CanExecute(null))
                    closingCommand.Execute(null);
                else
                    args.Cancel = true;
            }
        }
    }
}