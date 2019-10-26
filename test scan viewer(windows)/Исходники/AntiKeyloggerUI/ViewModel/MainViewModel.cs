using AntiKeyloggerUI.Auxiliary;
using AntiKeyloggerUI.Auxiliary.Interfaces;
using AntiKeyloggerUI.Model;
using AntiKeyloggerUI.Models;
using AntiKeyloggerUI.Properties;
using AntiKeyloggerUI.View;

using ExchangeModel.Interface;
using ExchangeModel.Model;

using System;
using System.Windows.Input;

namespace AntiKeyloggerUI.ViewModel
{
    public class MainViewModel : BindingProperty, IExchangeEventProcess
    {
        #region Свойства

        public ICommand CmdSetting { get; private set; }
        public ICommand LoginCmd { get; private set; }
        public ICommand LogoutCmd { get; private set; }
        public ICommand ConnectCmd { get; private set; }
        public ICommand DisconnectCmd { get; private set; }
        public ICommand SentCmd { get; private set; }

        private bool _encryptedSide;
        public bool EncryptedSide
        {
            get { return _encryptedSide; }
            set
            {
                _encryptedSide = value;
                OnPropertyChanged(nameof(EncryptedSide));
            }
        }

        private bool _decryptedSide;
        public bool DecryptedSide
        {
            get { return _decryptedSide; }
            set
            {
                _decryptedSide = value;
                OnPropertyChanged(nameof(DecryptedSide));
            }
        }

        private Client _activeUser;
        public Client ActiveUser
        {
            get { return _activeUser; }
            set
            {
                _activeUser = value;
                OnPropertyChanged(nameof(ActiveUser));
            }
        }

        public ScanCodeHandler DisplayTextModel { get; private set; }
        public ToastViewModel Notificate { get; private set; } 
        public IExchanger ExchangerModel { get;  set; }
        public IAuthorization Authorization{ get; private set; }
        #endregion

        #region Конструктор

        public MainViewModel()
        {
            CommandInitialize();
            Notificate = new ToastViewModel();
            ExchangerModel = new Exchanger(this);
            Authorization = new AuthorizationService();
            DisplayTextModel = new ScanCodeHandler();
          
            DecryptedSide = true;
            EncryptedSide = true;
        }
        #endregion

        #region Методы
      
        private void CommandInitialize()
        {
            LoginCmd = new ActionCommand(DoLogin);
            LogoutCmd = new ActionCommand(DoLogout, CanLogout);
            ConnectCmd = new ActionCommand(DoConnect, CanConnect);
            DisconnectCmd = new ActionCommand(DoDisconnect, CanDisconnect);
            SentCmd = new ActionCommand(DoSentString, CanSent);
        }

        public void OnCatchException(Exception exception)
        {
            Notificate.ShowError(exception.Message);
        }

        public void OnReceiveData(Response response)
        {
            DisplayTextModel.OnReceiveScanCodes(response);
        }

        public void OnStateChanged(bool isConnected)
        {
            
        }


        #endregion

        #region Обработчики команд

        private void DoSentString(object obj)
        {
            DisplayTextModel.Register();
        }

        private void DoLogin(object obj)
        {
            AuthenticateUser user = new AuthenticateUser();
            LoginView view = new LoginView(){
                DataContext = user
            };
            view.ShowDialog();

            if (!(view.DialogResult.HasValue && view.DialogResult.Value)) {
                ActiveUser = null;
                return;
            }

            ActiveUser = Authorization.Authorize(user);
            if (ActiveUser == null) {
                Notificate.ShowError("Такого пользователя нет");
                return;
            }
            Notificate.ShowSuccess("Добро пожаловать " + ActiveUser.Name);
        }

        private void DoLogout(object obj)
        {
            ActiveUser = null;
            DoDisconnect(null);
        }

        private void DoDisconnect(object obj)
        {
            try
            {
                ExchangerModel.StopExchanger();
                DisplayTextModel.Stop();
                Notificate.ShowSuccess("Отключение от устройства");
            }
            catch(Exception ex)
            { Notificate.ShowError(ex.Message); }
        }

        private void DoConnect(object obj)
        {
            try
            {
                ExchangerModel.RunExchanger();
                DisplayTextModel.Run();
                Notificate.ShowSuccess("Подключение к " + ExchangerModel.Portname);
            }
            catch (Exception ex)
            { Notificate.ShowError(ex.Message); }
        }

        private bool CanLogout(object arg)
        {
            return (ActiveUser == null) ? false : true;
        }

        private bool CanSent(object arg)
        {
            if (ActiveUser == null)
                return false;

            return true;
        }

        private bool CanDisconnect(object arg)
        {
            if (ActiveUser == null)
                return false;

            return true;
        }

        private bool CanConnect(object arg)
        {
            if (ActiveUser == null)
                return false;

            return true;
        }

        #endregion

    }
}
