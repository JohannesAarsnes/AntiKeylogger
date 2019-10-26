using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using AntiKeyloggerUI.Auxiliary;
using AntiKeyloggerUI.Auxiliary.Events;
using AntiKeyloggerUI.Auxiliary.Interfaces;
using AntiKeyloggerUI.Model.Exchange;

namespace AntiKeyloggerUI.Model
{
    public class Exchanger : BindingProperty, IExchanger
    {
        #region Поля

        private const int REQUEST_INTERVAL = 1000 *3;

        /// <summary>
        /// Событие получения данных от устройства
        /// </summary>
        public event EventHandler<DataReceiveEvent> ReceiveDataEvent;
       
        #endregion

        #region Свойства

        /// <summary>
        ///  Список доступных устройств
        /// </summary>
        private List<Device> _deviceList;
        public ReadOnlyCollection<Device> DeviceList { get => _deviceList.AsReadOnly(); }

        /// <summary>
        /// Выбранное устройство для обмена
        /// </summary>
        private Device _selectedDevice;
        public Device SelectedDevice
        {
            get { return _selectedDevice; }
            set
            {
                _selectedDevice = value;
                OnPropertyChanged(nameof(SelectedDevice));

            }
        }

        /// <summary>
        /// Инициализировано ли устройство
        /// </summary>
        private bool isDeviceInit;

        /// <summary>
        /// Счетчик мастера запросов
        /// </summary>
        private ushort requestCounter;
        #endregion

        #region Конструктор

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public Exchanger()
        {
            _deviceList = new List<Device>();
            SelectedDevice = new Device();
            _deviceList.Add(SelectedDevice);
            OnPropertyChanged(nameof(DeviceList));
        }

        #endregion

        #region Методы

        /// <summary>
        /// Подключение к устройству для обмена
        /// </summary>
        /// <returns></returns>
        public bool Connect()
        {
            if (_selectedDevice == null)
                return false;

    
            _selectedDevice.SerialReceiveEvent += OnDataReceive;
            _selectedDevice.SerialErrorEvent += OnErrorReceive;
            _selectedDevice.SerialStateChangeEvent += OnStateChanged;
            _selectedDevice.Connect(true);
            
            RunRetryRequest();
            return true;
        }



        /// <summary>
        /// Отключения от устройства
        /// </summary>
        /// <returns></returns>
        public bool Disconnect()
        {
            if (_selectedDevice == null)
                return false;

            _selectedDevice.Disconnect();
            return true;
        }

        /// <summary>
        /// Поиск доступных устройств
        /// </summary>
        /// <returns></returns>
        public bool FindDevice()
        {
            return true;
        }

        /// <summary>
        /// Получение данных от устройства еще не расформированные
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDataReceive(object sender, CommunicationReceiveEvent e)
        {
            if (sender == null || e == null || e.Buffer == null)
                return;

            byte[] payload = Crypto.Decrypt(e.Buffer);
            ExchangePacket packet = new ExchangePacket();

            if (!packet.SetBytes(payload))
                throw new Exception("Получен не корректный пакет");

            if(packet.Code == (byte)ResponseCode.NEGATIVE)
                throw new Exception("Запрос не был выполнен");

            isDeviceInit = true;
            if (packet.Payload == null)
                return;

            ReceiveDataEvent.Invoke(this, new DataReceiveEvent { Data = packet.Payload });
        }

        private void OnStateChanged(object sender, CommunicationStateChangeEvent e)
        {
            
        }

        private void OnErrorReceive(object sender, CommunicationErrorEvent e)
        {
            throw e.Exception;
        }

        /// <summary>
        /// Отправка публичного ключа пока устройство не ответит своим зашифрованным пакетом
        /// </summary>
        private void RunRetryRequest() {
            
            isDeviceInit = false;
            Task.Factory.StartNew(()=> {

                while (true)
                {
                    if (SelectedDevice == null || !SelectedDevice.IsConnect)
                        return;
                    requestCounter++;
                    if (!isDeviceInit)
                    {
                        byte[] initRequest = GetInitPacket();
                        SelectedDevice.Transmit(initRequest, initRequest.Length);
                    }
                    else {
                        byte[] reportRequest = GetReportPacket();
                        SelectedDevice.Transmit(reportRequest, reportRequest.Length);
                    }

                    Thread.Sleep(REQUEST_INTERVAL);     
                }

            });
        }

        /// <summary>
        /// Получить пакет опрос
        /// </summary>
        /// <returns></returns>
        private byte[] GetReportPacket()
        {
            ExchangePacket packet = new ExchangePacket();
            packet.Code = (byte)RequestCode.GET_REPORT;
            packet.Unique = requestCounter;
            return packet.GetBytes();
        }

        /// <summary>
        /// Сформировать пакет с публичным ключом
        /// </summary>
        /// <returns></returns>
        private byte[] GetInitPacket()
        {
            ExchangePacket packet = new ExchangePacket();
            packet.Code = (byte)RequestCode.INIT;
            packet.Payload = Crypto.PublicKey;
            packet.Unique = requestCounter;
            return packet.GetBytes();
        }

        #endregion
    }
}
