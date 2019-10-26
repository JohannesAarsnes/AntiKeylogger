namespace ExchangeModel.Interface
{
    public interface IExchanger
    {
        string Portname { get;}
        void RunExchanger();
        void StopExchanger();
    }
}
