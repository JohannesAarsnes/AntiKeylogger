using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace AntiKeyloggerUI.Auxiliary
{
    public class WindowCromeResizeHelper
    {
        /// <summary>
        /// Извлекает информацию о мониторе.
        /// </summary>
        /// <param name="hMonitor"></param>
        /// <param name="lpmi"></param>
        /// <returns></returns>
        [DllImport("user32")]
        internal static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

        /// <summary>
        /// Извлекает дескриптор монитора, 
        /// </summary>
        /// <param name="handle">Дескриптор окна</param>
        /// <param name="flags">Определяет пересекает ли окно</param>
        /// <returns></returns>
        [DllImport("User32")]
        internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

        /// <summary>
        /// Пользовательская функция обратного вызова, 
        /// которая обрабатывает сообщения, отправленные в окно
        /// </summary>
        /// <param name="hwnd">Дескриптор оконной процедуры, которая получает сообщение.</param>
        /// <param name="msg">Сообщение окну</param>
        /// <param name="wParam">Дополнительная информация о сообщении</param>
        /// <param name="lParam">Дополнительная информация о сообщении</param>
        /// <param name="handled">Признак обработанного сообщения</param>
        /// <returns></returns>
        private static IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                // Отправляется в окно, когда размер или положение окна собирается измениться. 
                case 0x0024:
                    WmGetMinMaxInfo(hwnd, lParam);
                    handled = true;
                    break;
            }
            return (IntPtr)0;
        }

        /// <summary>
        /// Получение информации окна
        /// </summary>
        /// <param name="hwnd">Дескриптор окна</param>
        /// <param name="lParam">дополнительный параметр</param>
        private static void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
        {
            // преобразование параметра в структуру
            MINMAXINFO mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

            // Флаг признак получения дескриптора монитора, ближайшего к окну.
            int MONITOR_DEFAULTTONEAREST = 0x00000002;

            // получение дескриптора монитора
            IntPtr monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);

            // получили дескриптор
            if (monitor != IntPtr.Zero)
            {
                MONITORINFO monitorInfo = new MONITORINFO();

                // получить информацию о мониторе
                GetMonitorInfo(monitor, monitorInfo);

                // преобразование информации
                RECT rcWorkArea = monitorInfo.rcWork;
                RECT rcMonitorArea = monitorInfo.rcMonitor;

                // переопределение размером окна относительно размеров монитора
                mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.left   - rcMonitorArea.left);
                mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.top    - rcMonitorArea.top);
                mmi.ptMaxSize.x     = Math.Abs(rcWorkArea.right  - rcWorkArea.left);
                mmi.ptMaxSize.y     = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
            }

            Marshal.StructureToPtr(mmi, lParam, true);
        }

        /// <summary>
        /// Инициализация обработчика сообщенияй окну
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void SourceInitialize(object sender, EventArgs e)
        {
            IntPtr handle = (new WindowInteropHelper(sender as Window)).Handle;
            HwndSource.FromHwnd(handle).AddHook(new HwndSourceHook(WindowProc));
        }

    }

    /// <summary>
    /// Определяет x - и y - координаты точки.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int x;
        public int y;
        public POINT(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    /// <summary>
    /// Содержит информацию о максимальном размере и положении окна, 
    /// а также его минимальном и максимальном размере отслеживания
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct MINMAXINFO
    {
        public POINT ptReserved;
        public POINT ptMaxSize;
        public POINT ptMaxPosition;
        public POINT ptMinTrackSize;
        public POINT ptMaxTrackSize;
    };

    /// <summary>
    ///  Содержит информацию о мониторе.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class MONITORINFO
    {
        public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));
        public RECT rcMonitor = new RECT();
        public RECT rcWork = new RECT();
        public int dwFlags = 0;
    }

    /// <summary>
    /// Определяет координаты левого верхнего и правого нижнего углов прямоугольника.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;

        public static readonly RECT Empty = new RECT();
        public int Width { get { return Math.Abs(right - left); } }
        public int Height { get { return bottom - top; } }

        public RECT(int left, int top, int right, int bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }

        public RECT(RECT rcSrc)
        {
            left = rcSrc.left;
            top = rcSrc.top;
            right = rcSrc.right;
            bottom = rcSrc.bottom;
        }

        public bool IsEmpty
        {
            get { return left >= right || top >= bottom; }
        }

        public override string ToString()
        {
            if (this == Empty)
            { return "RECT {Empty}"; }
            return "RECT { left : " + left + " / top : " + top + " / right : " + right + " / bottom : " + bottom + " }";
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Rect)) { return false; }
            return (this == (RECT)obj);
        }

        /// <summary>
        /// Return the HashCode for this struct (not garanteed to be unique)
        /// </summary>
        public override int GetHashCode()
        {
            return left.GetHashCode() + top.GetHashCode() + right.GetHashCode() + bottom.GetHashCode();
        }
        /// <summary> 
        /// Determine if 2 RECT are equal (deep compare)
        /// </summary>
        public static bool operator ==(RECT rect1, RECT rect2)
        {
            return (rect1.left == rect2.left && rect1.top == rect2.top && rect1.right == rect2.right && rect1.bottom == rect2.bottom);
        }
        /// <summary> 
        /// Determine if 2 RECT are different(deep compare)
        /// </summary>
        public static bool operator !=(RECT rect1, RECT rect2)
        {
            return !(rect1 == rect2);
        }
    }
}
