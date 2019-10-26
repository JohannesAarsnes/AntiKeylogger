using System.Collections.Generic;
using WindowsInput;

namespace ExchangeModel.Model
{
    public class ScanCodeHelper
    {
        public const byte MANIPULATION_CODE = 0xE0;
        public const byte BREAK_CODE = 0xF0;
        public const byte LEFT_SHFT = 0x12;
        public const byte RIGHT_SHFT = 0x59;
        public const byte CAPS_LOCK = 0x58;
        public const byte CONTROL_KEY = 0xE0;

        public Dictionary<byte, byte> LowerCaseCodes { get; private set; }
        public Dictionary<byte, byte> UpperCaseCodes { get; private set; }
        public Dictionary<ushort, VirtualKeyCode> VirtualKeyCodes { get; private set; }

        private static ScanCodeHelper _instance;
        public static ScanCodeHelper Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ScanCodeHelper();

                return _instance;
            }
        }

        private ScanCodeHelper()
        {
            InitializeLowerTable();
            InitializeUpperTable();
            InitializeVirtualTable();
        }


        private void InitializeLowerTable()
        {
            LowerCaseCodes = new Dictionary<byte, byte>();
            // lower case alphabet
            LowerCaseCodes.Add(0x1c, 0x61);
            LowerCaseCodes.Add(0x32, 0x62);
            LowerCaseCodes.Add(0x21, 0x63);
            LowerCaseCodes.Add(0x23, 0x64);
            LowerCaseCodes.Add(0x24, 0x65);
            LowerCaseCodes.Add(0x2b, 0x66);
            LowerCaseCodes.Add(0x34, 0x67);
            LowerCaseCodes.Add(0x33, 0x68);
            LowerCaseCodes.Add(0x43, 0x69);
            LowerCaseCodes.Add(0x3b, 0x6a);
            LowerCaseCodes.Add(0x42, 0x6b);
            LowerCaseCodes.Add(0x4b, 0x6c);
            LowerCaseCodes.Add(0x3a, 0x6d);
            LowerCaseCodes.Add(0x31, 0x6e);
            LowerCaseCodes.Add(0x44, 0x6f);
            LowerCaseCodes.Add(0x4d, 0x70);
            LowerCaseCodes.Add(0x15, 0x71);
            LowerCaseCodes.Add(0x2d, 0x72);
            LowerCaseCodes.Add(0x1b, 0x73);
            LowerCaseCodes.Add(0x2c, 0x74);
            LowerCaseCodes.Add(0x3c, 0x75);
            LowerCaseCodes.Add(0x2a, 0x76);
            LowerCaseCodes.Add(0x1d, 0x77);
            LowerCaseCodes.Add(0x22, 0x78);
            LowerCaseCodes.Add(0x35, 0x79);
            LowerCaseCodes.Add(0x1a, 0x7a);

            // digits
            LowerCaseCodes.Add(0x45, 0x30);
            LowerCaseCodes.Add(0x16, 0x31);
            LowerCaseCodes.Add(0x1e, 0x32);
            LowerCaseCodes.Add(0x26, 0x33);
            LowerCaseCodes.Add(0x25, 0x34);
            LowerCaseCodes.Add(0x2e, 0x35);
            LowerCaseCodes.Add(0x36, 0x36);
            LowerCaseCodes.Add(0x3d, 0x37);
            LowerCaseCodes.Add(0x3e, 0x38);
            LowerCaseCodes.Add(0x46, 0x39);

            // 
            LowerCaseCodes.Add(0x4e, 0x2d);
            LowerCaseCodes.Add(0x55, 0x3d);
            LowerCaseCodes.Add(0x54, 0x5b);
            LowerCaseCodes.Add(0x5b, 0x5d);
            LowerCaseCodes.Add(0x5d, 0x5c);
            LowerCaseCodes.Add(0x4c, 0x3b);
            LowerCaseCodes.Add(0x52, 0x27);
            LowerCaseCodes.Add(0x0e, 0x60);
            LowerCaseCodes.Add(0x41, 0x2c);
            LowerCaseCodes.Add(0x49, 0x2e);
            LowerCaseCodes.Add(0x4a, 0x2f);


        }

        private void InitializeUpperTable()
        {
            UpperCaseCodes = new Dictionary<byte, byte>();
            // upper case aphabets
            UpperCaseCodes.Add(0x1c, 0x41);
            UpperCaseCodes.Add(0x32, 0x42);
            UpperCaseCodes.Add(0x21, 0x43);
            UpperCaseCodes.Add(0x23, 0x44);
            UpperCaseCodes.Add(0x24, 0x45);
            UpperCaseCodes.Add(0x2b, 0x46);
            UpperCaseCodes.Add(0x34, 0x47);
            UpperCaseCodes.Add(0x33, 0x48);
            UpperCaseCodes.Add(0x43, 0x49);
            UpperCaseCodes.Add(0x3b, 0x4a);
            UpperCaseCodes.Add(0x42, 0x4b);
            UpperCaseCodes.Add(0x4b, 0x4c);
            UpperCaseCodes.Add(0x3a, 0x4d);
            UpperCaseCodes.Add(0x31, 0x4e);
            UpperCaseCodes.Add(0x44, 0x4f);
            UpperCaseCodes.Add(0x4d, 0x50);
            UpperCaseCodes.Add(0x15, 0x51);
            UpperCaseCodes.Add(0x2d, 0x52);
            UpperCaseCodes.Add(0x1b, 0x53);
            UpperCaseCodes.Add(0x2c, 0x54);
            UpperCaseCodes.Add(0x3c, 0x55);
            UpperCaseCodes.Add(0x2a, 0x56);
            UpperCaseCodes.Add(0x1d, 0x57);
            UpperCaseCodes.Add(0x22, 0x58);
            UpperCaseCodes.Add(0x35, 0x59);
            UpperCaseCodes.Add(0x1a, 0x5a);

            // separators
            UpperCaseCodes.Add(0x16, 0x21);
            UpperCaseCodes.Add(0x1e, 0x22);
            UpperCaseCodes.Add(0x26, 0x23);
            UpperCaseCodes.Add(0x25, 0x24);
            UpperCaseCodes.Add(0x2e, 0x25);
            UpperCaseCodes.Add(0x36, 0x5e);
            UpperCaseCodes.Add(0x3d, 0x26);
            UpperCaseCodes.Add(0x3e, 0x2a);
            UpperCaseCodes.Add(0x46, 0x28);
            UpperCaseCodes.Add(0x45, 0x29);

            //
            UpperCaseCodes.Add(0x4e, 0x5f);
            UpperCaseCodes.Add(0x55, 0x2b);
            UpperCaseCodes.Add(0x54, 0x7b);
            UpperCaseCodes.Add(0x5b, 0x7d);
            UpperCaseCodes.Add(0x5d, 0x7c);
            UpperCaseCodes.Add(0x4c, 0x3a);
            UpperCaseCodes.Add(0x52, 0x22);
            UpperCaseCodes.Add(0x0e, 0x7e);
            UpperCaseCodes.Add(0x41, 0x3c);
            UpperCaseCodes.Add(0x49, 0x3e);
            UpperCaseCodes.Add(0x4a, 0x3f);
        }

        private void InitializeVirtualTable()
        {

            VirtualKeyCodes = new Dictionary<ushort, VirtualKeyCode>();

            VirtualKeyCodes.Add(0x5a, VirtualKeyCode.RETURN);
            VirtualKeyCodes.Add(0x76, VirtualKeyCode.ESCAPE);
            VirtualKeyCodes.Add(0x66, VirtualKeyCode.BACK);
            VirtualKeyCodes.Add(0x0D, VirtualKeyCode.TAB);
            VirtualKeyCodes.Add(0x29, VirtualKeyCode.SPACE);

            VirtualKeyCodes.Add(0x05, VirtualKeyCode.F1);
            VirtualKeyCodes.Add(0x06, VirtualKeyCode.F2);
            VirtualKeyCodes.Add(0x04, VirtualKeyCode.F3);
            VirtualKeyCodes.Add(0x0c, VirtualKeyCode.F4);
            VirtualKeyCodes.Add(0x03, VirtualKeyCode.F5);
            VirtualKeyCodes.Add(0x0b, VirtualKeyCode.F6);
            VirtualKeyCodes.Add(0x83, VirtualKeyCode.F7);
            VirtualKeyCodes.Add(0x0A, VirtualKeyCode.F8);
            VirtualKeyCodes.Add(0x01, VirtualKeyCode.F9);
            VirtualKeyCodes.Add(0x09, VirtualKeyCode.F10);
            VirtualKeyCodes.Add(0x78, VirtualKeyCode.F11);
            VirtualKeyCodes.Add(0x07, VirtualKeyCode.F12);

            VirtualKeyCodes.Add(0xe07C, VirtualKeyCode.PRINT);
            VirtualKeyCodes.Add(0x7e, VirtualKeyCode.SCROLL);
            VirtualKeyCodes.Add(0xe07e, VirtualKeyCode.PAUSE);
            VirtualKeyCodes.Add(0xe070, VirtualKeyCode.INSERT);
            VirtualKeyCodes.Add(0xe06c, VirtualKeyCode.HOME);
            VirtualKeyCodes.Add(0xe071, VirtualKeyCode.DELETE);
            VirtualKeyCodes.Add(0xe069, VirtualKeyCode.END);

            VirtualKeyCodes.Add(0xe06b, VirtualKeyCode.LEFT);
            VirtualKeyCodes.Add(0xe074, VirtualKeyCode.RIGHT);
            VirtualKeyCodes.Add(0xe072, VirtualKeyCode.DOWN);
            VirtualKeyCodes.Add(0xe075, VirtualKeyCode.UP);

            VirtualKeyCodes.Add(0x77, VirtualKeyCode.NUMLOCK);
        }
    }
}
