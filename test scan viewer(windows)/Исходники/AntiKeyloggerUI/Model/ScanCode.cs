namespace AntiKeyloggerUI.Model
{
    public class ScanCode
    {
        public byte Code { get; set; }
        public bool IsMake { get; set; }
        public bool IsBreak { get; set; }
        public bool isShiftPressed { get; set; }
        public bool isControlDown { get; set; }
        public bool isControlUp { get; set; }

        public void Reset()
        {
            IsMake = false;
            isShiftPressed = false;
            IsBreak = false;
            Code = 0x00;
        }
    }
}
