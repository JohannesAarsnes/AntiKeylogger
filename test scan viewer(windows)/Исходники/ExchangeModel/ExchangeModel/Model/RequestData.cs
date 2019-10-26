namespace ExchangeModel.Model
{
    public class RequestData
    {
        public byte[] Data { get; private set; }

        public RequestData(byte[] data)
        {
            this.Data = data;
        }
    }
}
