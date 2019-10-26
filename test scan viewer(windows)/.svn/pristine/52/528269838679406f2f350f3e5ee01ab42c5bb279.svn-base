using AntiKeyloggerUI.Auxiliary.Interfaces;

namespace AntiKeyloggerUI.Model
{
    public class Crypto 
    {
        public static byte[] PublicKey { get; private set; }
        private static byte[] privateKey;
        private const int KEY_LENGTH = 24;

        static Crypto() 
        {
            PublicKey = new byte[KEY_LENGTH];
            privateKey = new byte[KEY_LENGTH];
        }


        public static byte[] Decrypt(byte[] data)
        {
            return data;
        }


        public static byte[] GetPublicKey(bool user)
        {
            return PublicKey;
        }

        public static byte[] GetSHA256Hash(byte[] data)
        {
            return new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15 }; ;
        }
    }
}
