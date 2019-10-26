

using ExchangeModel.Auxiliary;
using ExchangeModel.Enum;
using ExchangeModel.Event;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace ExchangeModel.Model
{
    public class SerialLine 
    {
        #region Переменные
        private SerialPort serilPort;
        private Thread receiverThread;
        private byte[] receiveBuffer;

        #endregion

        #region Свойства

        public event EventHandler<CommunicationReceiveEvent> SerialReceiveEvent;
        public event EventHandler<CommunicationStateChangeEvent> SerialStateChangeEvent;
        public event EventHandler<CommunicationErrorEvent> SerialErrorEvent;

        public string PortName { get; private set; }
        public BaudRate PortBaudRateValue { get; private set; }
        public Parity PortParity { get; private set; }
        public StopBits PortStopbits { get; private set; }
        public DataBits PortDataBitsValue { get; private set; }
        public bool IsConnect { get; private set; }
      
        #endregion

        #region Конструктор

        /// <summary>
        /// Конструктор c флагом инициализации параметров канала связи
        /// </summary>
        public SerialLine()
        {
            receiveBuffer = new byte[256];
            DefaultParametrs();
        }

        #endregion

        #region Публичные методы

        /// <summary>
        /// Подключение по порту к устройству
        /// </summary>
        public void Connect()
        {
                if (IsConnect || serilPort != null)
                    Disconnect();

                serilPort = new SerialPort();
                InitParametrs(serilPort);
                serilPort.ErrorReceived += OnErrorReceive;
                serilPort.Open();
                OnSerialStateChangeEvent(true);
                receiverThread = new Thread(Receive)
                {
                    Priority = ThreadPriority.AboveNormal,
                    IsBackground = true
                };
                receiverThread.Start();

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
                serilPort.Close();
                receiverThread.Join(200);
                receiverThread = null;
            }
            catch (Exception ex)
            {
                OnSerialErrorEvent(ex);
            }
        }

        /// <summary>
        /// Отправка данных по порту
        /// </summary>
        public void Transmit(byte[] data)
        {
            try
            {
                if (serilPort == null || !serilPort.IsOpen)
                    throw new Exception("Нет подключения");

                byte[] staffingData = StaffingHelper.Staffing(data);

                if (staffingData == null)
                    return;

                serilPort.Write(staffingData, 0, staffingData.Length);
            }
            catch (Exception ex)
            {
                OnSerialErrorEvent(ex);
            }
        }

        /// <summary>
        /// Приемник данных
        /// </summary>
        private void Receive()
        {
            List<byte> ReceivedBytes = new List<byte>(serilPort.ReadBufferSize);

            uint crc32 = CheckSum.PACK_SUM_START;
            int readBytes = 0;
            byte tmpByte = 0;
            bool destaff = false;
            bool isStart = false;

            try
            {
                while (IsConnect)
                {
                    readBytes = serilPort.BytesToRead;
                    if (readBytes > 0 && readBytes < receiveBuffer.Length)
                    {
                        serilPort.Read(receiveBuffer, 0, readBytes);
                        for (int idx = 0; idx < readBytes; idx++)
                        {
                            tmpByte = receiveBuffer[idx];
                            if (tmpByte == StaffingHelper.FEND)
                            {
                                ReceivedBytes.Clear();
                                crc32 = CheckSum.PACK_SUM_START;
                                isStart = true;
                                continue;
                            }

                            if (!isStart)
                                continue;

                            if (tmpByte == StaffingHelper.FESC)
                            {
                                destaff = true;
                                continue;
                            }

                            if (destaff)
                            {
                                destaff = false;
                                if (tmpByte == StaffingHelper.TFEND)
                                    ReceivedBytes.Add(StaffingHelper.FEND);
                                else if (tmpByte == StaffingHelper.TFESC)
                                    ReceivedBytes.Add(StaffingHelper.FESC);
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

                            if (ReceivedBytes.Count >= Packet.HEADER_SIZE)
                                size = ((ReceivedBytes[Packet.PAYLOAD_LENGTH_HIGH_INDEX] << 8) | ReceivedBytes[Packet.PAYLOAD_LENGTH_LOW_INDEX]);

                            if (ReceivedBytes.Count <= (Packet.HEADER_SIZE + size))

                                crc32 = CheckSum.Crc32_IEEE_802_Step(ReceivedBytes[ReceivedBytes.Count - 1], crc32);

                            if (ReceivedBytes.Count == (Packet.HEADER_SIZE + size + Packet.HASH_SIZE))
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
                                    crc32 = CheckSum.PACK_SUM_START;
                                    isStart = false;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
               OnSerialErrorEvent(ex);
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
            PortName = "COM3";
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
            OnSerialErrorEvent(new Exception("Ошибка на последовательном порту"));
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
        private void OnSerialErrorEvent(Exception exception)
        {
            SerialErrorEvent?.Invoke(this,
                new CommunicationErrorEvent
                {
                    Exception = exception
                });
        }
        #endregion
    }
}
