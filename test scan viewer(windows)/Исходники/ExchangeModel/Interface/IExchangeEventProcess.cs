using ExchangeModel.Model;
using System;

namespace ExchangeModel.Interface
{
    public interface IExchangeEventProcess
    {
        void OnCatchException(Exception exception);
        void OnReceiveData(Response response);
        void OnStateChanged(bool isConnected);
    }
}
