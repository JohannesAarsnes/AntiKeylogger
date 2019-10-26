using ExchangeModel.Enum;
using ExchangeModel.Event;
using ExchangeModel.Interface;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ExchangeModel.Model
{
    public class Exchanger : IExchanger
    {
        #region Поля

        public string Portname { get; private set; }
        private readonly SerialLine serialLine;

        private readonly IExchangeEventProcess eventProcessor;
        private readonly ExchangerCrypto exchangerCrypto;
        
        private ushort masterCounter;

        private bool isReceiveData;
        private bool isRunExchange;
        private byte[] receiveData;

        #endregion

        #region Конструктор

        public Exchanger(IExchangeEventProcess eventProcessor)
        {
            this.eventProcessor = eventProcessor;
            serialLine = new SerialLine();
            exchangerCrypto = new ExchangerCrypto();
            Portname = serialLine.PortName;

            serialLine.SerialErrorEvent += OnSerialLineErrorEvent;
            serialLine.SerialStateChangeEvent += OnSerialLineStateChangedEvent;
            serialLine.SerialReceiveEvent += OnSerialLineReceiveEvent;
        }

        #endregion

        #region Методы

        public void RunExchanger()
        {
            if (!exchangerCrypto.GenerateKey())
                throw new Exception("Не удалось сгенерировать ключ");

            if (serialLine.IsConnect)
            {
                throw new Exception("Попытка повторного запуска сервиса");
            }

            if (!Connect())
            {
                throw new Exception("Не удалось подключиться к устройству");
            }

            if (!InitializeExchange())
            {
                serialLine.Disconnect();
                throw new Exception("Не удалось проинициализировать устройство");
            }

            RunMonitoring();
        }

        public void StopExchanger()
        {
            // повторная попытка подключения
            if (!serialLine.IsConnect)
                throw new Exception("Попытка повторного отключения сервиса");
            isRunExchange = false;
            serialLine.Disconnect();
        }

        private bool Connect()
        {
            int counter = 5;
            while (counter > 0)
            {
                counter--;

                try
                {
                    serialLine.Connect();
                    return true;
                }
                catch (Exception)
                {
                    Thread.Sleep(200);
                    continue;
                }
            }

            return false;
        }

        private void RunMonitoring()
        {
            Task.Factory.StartNew(() =>
            {
                byte initilizeCounter = 0;
                isRunExchange = true;
                while (isRunExchange)
                {
                    if (!serialLine.IsConnect)
                    {
                        if (!Connect())
                        {
                            isRunExchange = false;
                            eventProcessor.OnCatchException(new Exception("Потеря связи с устройством, без возможности подключения"));
                            continue;
                        }

                        if (!InitializeExchange())
                        {
                            initilizeCounter++;
                            serialLine.Disconnect();
                            if (initilizeCounter >= 5)
                            {
                                initilizeCounter = 0;
                                isRunExchange = false;
                                eventProcessor.OnCatchException(new Exception("Не удалось проинициализировать устройство"));
                                continue;
                            }
                            Thread.Sleep(100);
                            continue;
                        }

                        initilizeCounter = 0;
                    }

                    PollingDevice();
                }
            });
        }

        private bool InitializeExchange()
        {
            Packet packet = new Packet
            {
                Code = (byte)RequestCode.INIT,
                Payload = exchangerCrypto.PublicKey,
                Unique = 0
            };

            byte[] message = packet.GetBytes();
            serialLine.Transmit(message);
            packet.Reset();

            byte[] encodedData = GetResponse();
            byte[] decodedData = exchangerCrypto.DecodeRSAInfo(encodedData);
            if (!packet.SetBytes(decodedData))
            {
                return false;
            }

            if (packet.Code == (byte)ResponseCode.NEGATIVE)
            {
                return false;
            }

            exchangerCrypto.CommonKey = packet.Payload;
            return true;
        }

        private bool PollingDevice()
        {
            Packet packet = new Packet
            {
                Code = (byte)RequestCode.REPORT,
                Payload = exchangerCrypto.GenerateVector(),
                Unique = masterCounter
            };

            serialLine.Transmit(packet.GetBytes());
            byte[] decodeData = exchangerCrypto.DecodeAESInfo(GetResponse());

            if (decodeData == null)
                return false;
            
            if (!packet.SetBytes(decodeData) || packet.Payload == null || packet.Code == (byte)ResponseCode.NEGATIVE){
                return true;
            }

            eventProcessor.OnReceiveData(new Response(packet.Payload, decodeData));
            return true;
        }

        private byte[] GetResponse()
        {
            int counter = 2000000;
            while (counter > 0)
            {
                if (isReceiveData)
                {
                    isReceiveData = false;
                    return receiveData;
                }

                counter--;
                Thread.Sleep(1);

            }
            return null;
        }

        private void OnSerialLineStateChangedEvent(object sender, CommunicationStateChangeEvent e)
        {
            eventProcessor.OnStateChanged(e.State);
        }

        private void OnSerialLineErrorEvent(object sender, CommunicationErrorEvent e)
        {
            serialLine.Disconnect();
            eventProcessor.OnCatchException(new Exception("Устройство отключено"));
        }

        private void OnSerialLineReceiveEvent(object sender, CommunicationReceiveEvent e)
        {
            try
            {
                if (isReceiveData)
                    return;

                receiveData = e.Buffer;
                isReceiveData = true;
            }
            catch (Exception ex)
            {
                isReceiveData = false;
                eventProcessor.OnCatchException(ex);
            }
        }
        #endregion
    }
}
