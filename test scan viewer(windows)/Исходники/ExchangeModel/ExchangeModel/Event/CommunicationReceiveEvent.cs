
using System;

namespace ExchangeModel.Event
{
    public class CommunicationReceiveEvent :EventArgs
    {
        public byte[] Buffer { get; set; }
        public int Size { get; set; }
        public bool IsCorrectPack { get; set; }
        public string Message { get; set; }
    }
}
