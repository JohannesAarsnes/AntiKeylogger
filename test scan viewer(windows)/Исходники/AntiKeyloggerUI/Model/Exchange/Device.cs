using AntiKeyloggerUI.Auxiliary;
using AntiKeyloggerUI.Model.Exchange;
using InkhSums;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace AntiKeyloggerUI.Model
{
    public class Device : BindingProperty
    {
        #region Константы

        /// <summary>
        /// Маркер начала пакета
        /// </summary>
        private const byte FEND = 0xC0;

        /// <summary>
        /// Маркер замены FEND
        /// </summary>
        private const byte FESC = 0xDB;

        /// <summary>
        /// Маркер замены FESC (первое значение)
        /// </summary>
        private const byte TFEND = 0xDC;

        /// <summary>
        /// Маркер замены FESC (второе значение)
        /// </summary>
        private const byte TFESC = 0xDD;

        /// <summary>
        /// Начальное значение CRC
        /// </summary>
        public const uint CRC_START_VALUE = 0xFFFFFFFF;


        /// <summary>
        /// Размер заголовка пакета
        /// </summary>
        private const byte HEAD_MIN_RAW_SIZE = 5;


        /// <summary>
        /// Размер контрольной суммы пакета
        /// </summary>
        private const byte CRC_RAW_SIZE = 4;

        /// <summary>
        /// Минимальный размер пакета заголовок пакета и контрольная сумма
        /// </summary>
        private const byte PACK_MIN_RAW_SIZE = HEAD_MIN_RAW_SIZE + CRC_RAW_SIZE;

        /// <summary>
        /// Первый байт размера пакета
        /// </summary>
        private const byte IND_LEN = 1;

        /// <summary>
        /// Второй байт размера пакета
        /// </summary>
        private const byte IND_LEN1 = 2;

        #endregion

        #region Параметры линии связи

        private string _portName;
        public string PortName
        {
            get { return _portName; }
            set
            {
                _portName = value;
                OnPropertyChanged(nameof(PortName));
            }
        }

        private BaudRate _baudRate;
        public BaudRate PortBaudRateValue
        {
            get { return _baudRate; }
            set
            {
                _baudRate = value;
                OnPropertyChanged(nameof(PortBaudRateValue));
            }
        }

        private Parity _portParity;
        public Parity PortParity
        {
            get { return _portParity; }
            set
            {
                _portParity = value;
                OnPropertyChanged(nameof(PortParity));
            }
        }

        private StopBits _portStopbits;
        public StopBits PortStopbits
        {
            get { return _portStopbits; }
            set
            {
                _portStopbits = value;
                OnPropertyChanged(nameof(PortStopbits));
            }
        }

        private DataBits _dataBits;
        public DataBits PortDataBitsValue
        {
            get { return _dataBits; }
            set
            {
                _dataBits = value;
                OnPropertyChanged(nameof(PortDataBitsValue));
            }
        }

        #endregion

        #region События на канале связи

        /// <summary>
        /// Событие приема данных 
        /// </summary>
        public event EventHandler<CommunicationReceiveEvent> SerialReceiveEvent;

        /// <summary>
        /// Событие изменения состояния
        /// </summary>
        public event EventHandler<CommunicationStateChangeEvent> SerialStateChangeEvent;

        /// <summary>
        /// Событие возникновение ошибки
        /// </summary>
        public event EventHandler<CommunicationErrorEvent> SerialErrorEvent;

        #endregion

        #region Переменные

        /// <summary>
        /// Канал связи с устройством
        /// </summary>
        private SerialPort port;

        /// <summary>
        /// Поток обработки входящих пакетов с канала связи
        /// </summary>
        private Thread receiver;

        /// <summary>
        /// Признак состояния канала связи
        /// </summary>
        public bool IsConnect { get; set; }

        /// <summary>
        /// Буффер приема пакетов
        /// </summary>
        private byte[] pack;
        #endregion

        #region Свойства


        #endregion

        #region Конструктор

        /// <summary>
        /// Конструктор c флагом инициализации параметров канала связи
        /// </summary>
        public Device()
        {
            pack = new byte[1024];
            DefaultParametrs();
        }

        #endregion

        #region Методы

        /// <summary>
        /// Подключение по порту к устройству
        /// </summary>
        public void Connect(bool isDefault)
        {
            try
            {
                if (IsConnect || port != null)
                    Disconnect();

                if (isDefault)
                    DefaultParametrs();

                port = new SerialPort();
                InitParametrs(port);
                ExchangePacket packet = new ExchangePacket();
                packet.Code = (byte)ResponseCode.POSITIVE;
                packet.Payload = Encoding.ASCII.GetBytes("a");
                byte[] staf = WakeConvertHelper.Staffing(packet.GetBytes());

                receiver = new Thread(Receive)
                {
                    Priority = ThreadPriority.AboveNormal,
                    IsBackground = true
                };

 
                port.ErrorReceived += OnErrorReceive;
                port.Open();
                OnSerialStateChangeEvent(true);
                receiver.Start();
            }
            catch (Exception ex)
            {
                OnSerialErrorEvent(0,null, ex);
            }
        }

        /// <summary>
        /// Отключиться от порта
        /// </summary>
        public void Disconnect()
        {
            try
            {
                if (!IsConnect)
                    return;

                OnSerialStateChangeEvent(false);
                int readBytes = port.BytesToRead;
                byte[] pack = new byte[readBytes];
                port.Read(pack, 0, readBytes);
                port.Close();
                receiver.Join(200);
                receiver = null;
            }
            catch (Exception ex)
            {
                OnSerialErrorEvent(0,null, ex);
            }
        }

        /// <summary>
        /// Отправка данных по порту
        /// </summary>
        public void Transmit(byte[] data, int dataLength)
        {
            try
            {
                if (port == null || !port.IsOpen)
                    throw new Exception("Нет подключения");

                byte[] staffingData = WakeConvertHelper.Staffing(data);

                if (staffingData == null)
                    return;

                port.Write(staffingData, 0, staffingData.Length);
            }
            catch (Exception ex)
            {
                OnSerialErrorEvent(0,null, ex);
            }
        }

        /// <summary>
        /// Приемник данных
        /// </summary>
        private void Receive()
        {
            Crc32MirrIEEE_802_3 crcAlg = new Crc32MirrIEEE_802_3();
            List<byte> ReceivedBytes = new List<byte>(port.ReadBufferSize);

            uint crc32 = CRC_START_VALUE;
            int readBytes = 0;
            byte tmpByte = 0;
            bool destaff = false;
            bool isStart = false;

            try
            {
                while (IsConnect)
                {
                    readBytes = port.BytesToRead;
                    if (readBytes > 0 && readBytes < pack.Length)
                    {
                        port.Read(pack, 0, readBytes);
                        for (int idx = 0; idx < readBytes; idx++)
                        {
                            tmpByte = pack[idx];
                            if (tmpByte == FEND)
                            {
                                ReceivedBytes.Clear();
                                crc32 = CRC_START_VALUE;
                                isStart = true;
                                continue;
                            }

                            if (!isStart)
                                continue;

                            if (tmpByte == FESC)
                            {
                                destaff = true;
                                continue;
                            }

                            if (destaff)
                            {
                                destaff = false;
                                if (tmpByte == TFEND)
                                    ReceivedBytes.Add(FEND);
                                else if (tmpByte == TFESC)
                                    ReceivedBytes.Add(FESC);
                                else
                                {
                                    ReceivedBytes.Clear();
                                    isStart = false;
                                    continue;
                                }
                            }
                            else
                                ReceivedBytes.Add(tmpByte);

                            int size = 0;

                            if (ReceivedBytes.Count >= HEAD_MIN_RAW_SIZE)
                                size = ((ReceivedBytes[IND_LEN1] << 8) | ReceivedBytes[IND_LEN]);

                            if (ReceivedBytes.Count <= (HEAD_MIN_RAW_SIZE + size))
                                crc32 = crcAlg.Step(ReceivedBytes[ReceivedBytes.Count - 1], crc32);

                            if (ReceivedBytes.Count == (HEAD_MIN_RAW_SIZE + size + CRC_RAW_SIZE))
                            {
                                uint h = BitConverter.ToUInt32(ReceivedBytes.ToArray(), (ReceivedBytes.Count - 4));
                                if (crc32 == h)
                                {
                                    OnSerialReceiveEvent(ReceivedBytes.ToArray());
                                    isStart = false;
                                     break;
                                }
                                else
                                {
                                    ReceivedBytes.Clear();
                                    crc32 = CRC_START_VALUE;
                                    isStart = false;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                OnSerialErrorEvent(0, null, ex);
            }
        }

        /// <summary>
        /// Сброс параметров канала связи
        /// </summary>
        public void ResetParametrs()
        {
            DefaultParametrs();
            PortName = string.Empty;
        }

        #endregion

        #region Приватные методы

        /// <summary>
        /// Инициализация канала связи введенными пользователем параметрами
        /// </summary>
        private void InitParametrs(SerialPort comport)
        {
            if (comport == null)
                throw new Exception("Использование не про инициализированную переменную");

            comport.PortName = (string.IsNullOrEmpty(PortName) ? "COM1" : PortName);
            comport.Parity = PortParity;
            comport.StopBits = PortStopbits;
            comport.BaudRate = (int)PortBaudRateValue;
            comport.DataBits = (int)PortDataBitsValue;
        }

        /// <summary>
        /// Значения параметров по умолчанию
        /// </summary>
        private void DefaultParametrs()
        {
            PortName = "COM15";
            PortParity = Parity.None;
            PortStopbits = StopBits.One;
            PortDataBitsValue = DataBits.DB_8;
            PortBaudRateValue = BaudRate.BDR_115200;
        }

        /// <summary>
        /// Обработчик событий ошибок канала связи
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnErrorReceive(object sender, SerialErrorReceivedEventArgs e)
        {
            OnSerialErrorEvent(1, "Ошибка канала связи", null);
        }

        /// <summary>
        /// Инициализация события приема данных
        /// </summary>
        /// <param name=""></param>
        private void OnSerialReceiveEvent(byte[] data)
        {
  
            SerialReceiveEvent?.Invoke(this,
                new CommunicationReceiveEvent
                {
                    Buffer = data,
                    Size = data.Length
                });
        }

        /// <summary>
        /// Инициализация события изменения состояния 
        /// </summary>
        /// <param name="state"></param>
        private void OnSerialStateChangeEvent(bool state)
        {
            IsConnect = state;
            SerialStateChangeEvent?.Invoke(this,
                new CommunicationStateChangeEvent
                {
                    State = IsConnect
                });
        }

        /// <summary>
        /// Инициализация события возникновения ошибки 
        /// </summary>
        private void OnSerialErrorEvent(int error, string message, Exception exception)
        {
            SerialErrorEvent?.Invoke(this,
                new CommunicationErrorEvent
                {
                    ErrorType = error,
                    Message = message,
                    Exception = exception
                });
        }
        #endregion
    }
}
