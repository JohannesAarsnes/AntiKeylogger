using AntiKeyloggerUI.Auxiliary;
using ExchangeModel.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WindowsInput;

namespace AntiKeyloggerUI.Model
{
    public class DisplayText : BindingProperty
    {
        #region Переменные



        private bool isCapsLockPressed;
        private bool isShiftPressed;
        private ScanCode currentCode;
        private ConcurrentQueue<Response> queue;
        private StringBuilder packetRawData;
        private bool isProcessorRun; 
        
        #endregion

        #region Свойства

        private int _index;
        public int Index
        {
            get { return _index; }
            set
            {
                _index = value;
                OnPropertyChanged(nameof(Index));
            }
        }

        private bool _isFocused;
        public bool IsFocused
        {
            get { return _isFocused; }
            set
            {
                _isFocused = value;
                OnPropertyChanged(nameof(IsFocused));
            }
        }

        private string _inputString;
        public string InputString
        {
            get { return _inputString; }
            set
            {
                _inputString = value;
                OnPropertyChanged(nameof(InputString));
            }
        }

        private StringBuilder _rawReceiveData;
        public string RawReceiveData
        {
            get { return _rawReceiveData.ToString(); }
        }

        private StringBuilder _parseReceiveData;
        public string ParseReceiveData
        {
            get { return _parseReceiveData.ToString(); }
        }

        
        #endregion

        #region Конструктор

        public DisplayText()
        {
            queue = new ConcurrentQueue<Response>();
            currentCode = new ScanCode();
            _rawReceiveData = new StringBuilder();
            _parseReceiveData = new StringBuilder();
            packetRawData = new StringBuilder();     
            InputString = string.Empty;
        }

        #endregion

        #region Методы

        public void OnReceiveScanCodes(Response response)
        {
            if (response == null)
                return;

            queue.Enqueue(response);
        }

        private void ParseScanCode(byte[] scanCodes)
        {
            foreach (byte _byte in scanCodes)
            {
                if (_byte == ScanCodeHelper.CONTROL_KEY)
                {
                    if (currentCode.isControlDown)
                        currentCode.isControlUp = true;
                    else
                        currentCode.isControlDown = true;
                    continue;
                }

                if (_byte == ScanCodeHelper.RIGHT_SHFT || _byte == ScanCodeHelper.LEFT_SHFT)
                {
                    if (currentCode.IsBreak)
                    {
                        isShiftPressed = false;
                        currentCode.IsBreak = false;
                    }
                    else
                    {
                        isShiftPressed = true;

                    }
                    continue;
                }

                if (_byte == ScanCodeHelper.BREAK_CODE)
                {
                    currentCode.IsBreak = true;
                    continue;
                }

                if (!currentCode.IsMake)
                {
                    currentCode.Code = _byte;
                    currentCode.IsMake = true;

                    continue;
                }

                if (currentCode.IsMake && currentCode.IsBreak)
                {

                    if (currentCode.Code == _byte)
                    {
                        currentCode.isShiftPressed = isShiftPressed;
                        ProcessScanCodes();
                        isShiftPressed = false;
                    }
                    currentCode.Reset();
                }

            }
        }

        public void Initialize()
        {
            Task.Factory.StartNew(() =>
            {
                isProcessorRun = true;
                while (isProcessorRun)
                {
                    try
                    {
                        if (queue.Count <= 0)
                            continue;

                        queue.TryDequeue(out Response response);

                        if (response == null)
                            continue;

                        ParseScanCode(response.ScanCodeBytes);
                        packetRawData.Append(ArrayConvertionHelper.ArrayToHexString(response.PacketBytes));
                        Thread.Sleep(1);
                    }
                    catch (Exception ex)
                    {
                        Console.Write(ex.Message);
                    }
                }
            });
        }

        public void Reset()
        {
            isProcessorRun = false;
            InputString = string.Empty;

            Response response;
            while (queue.TryDequeue(out response)){ }
            _rawReceiveData.Clear();
            _parseReceiveData.Clear();
            packetRawData.Clear();

            InputString = string.Empty;
            
            OnPropertyChanged(nameof(RawReceiveData));
            OnPropertyChanged(nameof(ParseReceiveData));
        }

        public void Register()
        {
            if (string.IsNullOrEmpty(InputString) || packetRawData.Length == 0)
            {
                InputString = string.Empty;
                return;
            }
                
            _parseReceiveData.Append(string.Format("{0} : {1}{2}", 
                DateTime.Now.ToLongTimeString(), 
                InputString, 
                Environment.NewLine));
            InputString = string.Empty;

            _rawReceiveData.Append(string.Format("{0} : {1}{2}", 
                DateTime.Now.ToLongTimeString(), 
                packetRawData.ToString(), 
                Environment.NewLine));
            packetRawData.Clear();

            
            OnPropertyChanged(nameof(RawReceiveData));
            OnPropertyChanged(nameof(ParseReceiveData));
        }

        private void ProcessScanCodes()
        {
            if (currentCode.isControlUp && currentCode.isControlDown)
            {
                SimulateKeyPress(currentCode);
                return;
            }

            if (currentCode.Code == ScanCodeHelper.CAPS_LOCK)
            {
                isCapsLockPressed = !isCapsLockPressed;
                return;
            }

            bool isUpper = (currentCode.isShiftPressed) ? !isCapsLockPressed : isCapsLockPressed;
            if (isUpper)
            {
                if (ScanCodeHelper.Instance.UpperCaseCodes.ContainsKey(currentCode.Code))
                    DisplayCode(ScanCodeHelper.Instance.UpperCaseCodes[currentCode.Code]);
                else
                    SimulateKeyPress(currentCode);
            }
            else
            {
                if (ScanCodeHelper.Instance.LowerCaseCodes.ContainsKey(currentCode.Code))
                    DisplayCode(ScanCodeHelper.Instance.LowerCaseCodes[currentCode.Code]);
                else
                    SimulateKeyPress(currentCode);
            }
        }

        private void SimulateKeyPress(ScanCode currentCode)
        {
            ushort key = currentCode.Code;
            byte shift = ScanCodeHelper.CONTROL_KEY;

            if (currentCode.isControlDown)
            {
                currentCode.isControlDown = false;
                currentCode.isControlUp = false;
                key |= (ushort)(shift << 8);
            }

            if (ScanCodeHelper.Instance.VirtualKeyCodes.ContainsKey(key))
            {
                VirtualKeyCode code = ScanCodeHelper.Instance.VirtualKeyCodes[key];

                if (code == VirtualKeyCode.LEFT)
                    Index--;

                if (code == VirtualKeyCode.RIGHT && Index < InputString.Length - 1)
                    Index++;

                if (code == VirtualKeyCode.SPACE)
                    Index++;

                if (code == VirtualKeyCode.BACK && Index != 0)
                    Index--;

                UiDispatcherHelper.BeginInvokeOnUi(() =>
                {
                    InputSimulator.SimulateKeyPress(code);
                });
            }

        }

        private void DisplayCode(byte code)
        {
            UiDispatcherHelper.BeginInvokeOnUi(() =>
            {
                if (Index < InputString.Length - 1)
                {
                    List<char> chars = new List<char>(InputString.ToCharArray());
                    if (Index > chars.Count)
                        Index = chars.Count;

                    chars.Insert(Index, (char)code);
                    InputString = ConvertToString(chars);
                }
                else
                {
                    InputString += (char)code;
                }
                Index++;
            });
        }

        private string ConvertToString(List<char> chars)
        {
            string temp = string.Empty;
            if (chars == null || chars.Count <= 0)
                return temp;

            foreach (char _char in chars)
                temp += _char;

            return temp;
        }

        #endregion
    }
}
