using AntiKeyloggerUI.Auxiliary;
using AntiKeyloggerUI.Models;

using System.Windows;
using System.Windows.Input;

namespace AntiKeyloggerUI.View
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        private ICommand _login;
        public ICommand CmdLogin
        {
            get
            {
                if (_login == null)
                    _login = new ActionCommand(DoLogin);

                return _login;
            }
        }
        public LoginView()
        {
            InitializeComponent();
            BtnLogin.Command = CmdLogin;
        }
        private void DoLogin(object obj)
        {
            this.DialogResult = true;
        }
        private void password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var item = this.DataContext as AuthenticateUser;
            if (password.SecurePassword.Length > 0)
            {
                item.IsPasswordActivate = true;
            }
            else
            {
                item.IsPasswordActivate = false;
            }
        }
    }
}
