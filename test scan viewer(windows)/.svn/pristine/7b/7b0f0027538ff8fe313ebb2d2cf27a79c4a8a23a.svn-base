using AntiKeyloggerUI.Auxiliary;

using System;
using System.Windows;

using ToastNotifications;
using ToastNotifications.Core;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace AntiKeyloggerUI.Models
{
    public class ToastViewModel :BindingProperty
    {
        #region Fields
        private readonly Notifier _notifier;
        #endregion
             
        #region Constructors
        public ToastViewModel()
        {

            _notifier = new Notifier(cfg =>
            {
                cfg.PositionProvider = new WindowPositionProvider(
                    parentWindow: Application.Current.MainWindow,
                    corner: Corner.BottomRight,
                    offsetX: 5,
                    offsetY: 5);

                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    notificationLifetime: TimeSpan.FromSeconds(3),
                    maximumNotificationCount: MaximumNotificationCount.FromCount(3));

                cfg.Dispatcher = Application.Current.Dispatcher;

                cfg.DisplayOptions.TopMost = false;
                cfg.DisplayOptions.Width = 400;

            });

            _notifier.ClearMessages();
        }
        #endregion
     
        #region Methods
        public void OnUnloaded()
        {
            _notifier.Dispose();
        }
        internal void ClearMessages(string msg)
        {
            UiDispatcherHelper.BeginInvokeOnUi(() =>
            {
                _notifier.ClearMessages(msg);
            });
        }

        public void ShowInformation(string message)
        {
            UiDispatcherHelper.BeginInvokeOnUi(() =>
            {
                _notifier.ShowInformation(message);
            });

        }
        public void ShowInformation(string message, MessageOptions opts)
        {
            UiDispatcherHelper.BeginInvokeOnUi(() =>
            {
                _notifier.ShowInformation(message, opts);
            });
        }

        public void ShowSuccess(string message)
        {
            UiDispatcherHelper.BeginInvokeOnUi(() =>
            {
                _notifier.ShowSuccess(message);
            });
        }
        public void ShowSuccess(string message, MessageOptions opts)
        {
            UiDispatcherHelper.BeginInvokeOnUi(() =>
            {
                _notifier.ShowSuccess(message, opts);
            });
        }
       
        public void ShowWarning(string message, MessageOptions opts)
        {
            UiDispatcherHelper.BeginInvokeOnUi(() =>
            {
                _notifier.ShowWarning(message, opts);
            });
        }

        public void ShowError(string message)
        {
            UiDispatcherHelper.BeginInvokeOnUi(() =>
            {
                _notifier.ShowError(message);
            });
        }
        public void ShowError(string message, MessageOptions opts)
        {
            UiDispatcherHelper.BeginInvokeOnUi(() =>
            {
                _notifier.ShowError(message, opts);
            });
        }
        #endregion
    }
}