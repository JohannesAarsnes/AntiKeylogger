
using AntiKeyloggerUI.Models;

namespace AntiKeyloggerUI.Model
{
    public class Client : AuthenticateUser
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }


        private string _role;
        public string Role
        {
            get { return _role; }
            set
            {
                _role = value;
                OnPropertyChanged(nameof(Role));
            }
        }

        private int _accessKey;
        public int AccessKey
        {
            get { return _accessKey; }
            set
            {
                _accessKey = value;
                OnPropertyChanged(nameof(AccessKey));
            }
        }

    }
}
