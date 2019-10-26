using System;

namespace ExchangeModel.Event
{
    public class CommunicationErrorEvent: EventArgs
    {
        public int ErrorType { get;  set; }
        public string Message { get;  set; }
        public Exception Exception { get; set; }
    }
}
