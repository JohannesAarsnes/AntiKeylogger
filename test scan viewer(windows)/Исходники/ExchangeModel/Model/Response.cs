namespace ExchangeModel.Model
{
    public class Response
    {
        public Response(byte[] scanCodeBytes, byte[] packetBytes)
        {
            ScanCodeBytes = scanCodeBytes;
            PacketBytes = packetBytes;
        }

        public byte[] ScanCodeBytes { get; private set; }
        public byte[] PacketBytes { get; private set; }   
    }
}
