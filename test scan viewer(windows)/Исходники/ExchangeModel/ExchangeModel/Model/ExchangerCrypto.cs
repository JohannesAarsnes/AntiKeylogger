
namespace ExchangeModel.Model
{
    public class ExchangerCrypto
    {
        private readonly byte[] privateKey;
        private byte[] vector; 
        public byte[] PublicKey { get; private set; }
        public byte[] CommonKey { get; set; }

        public ExchangerCrypto()
        {
            PublicKey = new byte[24];
            privateKey = new byte[24];
        }


        public byte[] DecodeRSAInfo(byte[] encodedData)
        {
            return encodedData;
        }

        public byte[] DecodeAESInfo(byte[] encodedData)
        {
            return encodedData;
        }

        public byte[] GenerateVector()
        {
            vector = new byte[16];
            return vector;
        }

        public bool GenerateKey()
        {
            return true;
        }
    }
}
