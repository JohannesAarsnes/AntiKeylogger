using AntiKeyloggerUI.Auxiliary.Interfaces;
using AntiKeyloggerUI.Models;
using AntiKeyloggerUI.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace AntiKeyloggerUI.Model
{
    public class AuthorizationService : IAuthorization
    {
        private List<Client> clientList;


        public AuthorizationService()
        {
            clientList = new List<Client>();
            initializeClient();
        }

        private void initializeClient() {

            string filename = Settings.Default.UserInfoFilepath;
            if (!File.Exists(filename)) {
                throw new Exception("Отсутствует информация о пользователях");
            }
            FileStream fileStream = new FileStream(filename, FileMode.Open);
            XElement userInfo = XElement.Load(fileStream);
            XElement users = userInfo.Element("users");
            
            foreach (XElement element in users.Elements())
            {

                if (element.Name == "user")
                {
                    Client client = new Client();
                    client.Name = element.Attribute("name").Value;
                    client.Login = element.Attribute("login").Value;
                    client.Password = element.Attribute("password").Value;
                    // client.AccessKey = int.Parse(element.Attribute("access_rights").Value);
                    clientList.Add(client);
                }
            }

            if(clientList.Count <= 0)
            {
                throw new Exception("Отсутствует информация о пользователях");
            }
        }

        

        public Client Authorize(AuthenticateUser user)
        {
            // Преобразование логина и пароля в хеш сумму
            //byte[] hashLogin = Crypto.GetSHA256Hash(null);
            string hashLogin = user.Login;
            //byte[] hashPassword = Crypto.GetSHA256Hash(null); ;
            string hashPassword = user.Password;
            
            foreach (Client client in clientList) {
                if (hashLogin.Equals(client.Login) && hashPassword.Equals(client.Password))
                    return client;
            }
            return null;
        }
    }
}
