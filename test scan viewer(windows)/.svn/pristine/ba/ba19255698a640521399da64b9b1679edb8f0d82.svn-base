using AntiKeyloggerUI.Auxiliary;
namespace AntiKeyloggerUI.Models
{
    public class AuthenticateUser : BindingProperty
    {

        private string _login;
        public string Login
        {
            get { return _login; }
            set
            {
                _login = value;
                OnPropertyChanged(nameof(Login));
            }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }
        
        private bool _isPasswordActivate;
        public bool IsPasswordActivate
        {
            get { return _isPasswordActivate; }
            set
            {
                _isPasswordActivate = value;
                OnPropertyChanged(nameof(IsPasswordActivate));
            }
        }
    }
}
