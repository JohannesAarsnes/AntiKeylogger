using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace AntiKeyloggerUI.Auxiliary
{
    /// <summary>
    /// Вспомогательный класс, предоставляющий методы преобразования массивов
    /// </summary>
    public static class ArrayConvertionHelper
    {
        /// <summary>
        /// Преобразовать байтовый массив в строку с HEX представлением элементов массива
        /// </summary>
        /// <param name="arr"> Массив </param>
        /// <returns></returns>
        public static String ArrayToHexString( byte[] arr )
        {
            return ArrayToHexString(arr, 0, (ushort)arr.Length);
        }

        /// <summary>
        /// Преобразовать байтовый массив в строку с HEX представлением элементов массива
        /// </summary>
        /// <param name="arr"> Массив </param>
        /// <param name="byteCount"> Количество используемых байт массива </param>
        /// <param name="offset"> Первый используемый байт массива </param>
        /// <returns></returns>
        public static String ArrayToHexString( byte[] arr, ushort offset, ushort byteCount )
        {


            StringBuilder builder = new StringBuilder( );
            for (int index = offset; index < offset + byteCount; index++)
                builder.Append(String.Format("{0:x2}", arr[index]));

            return builder.ToString( );
        }

        /// <summary>
        /// Сформировать байтовый массив на основе строки с HEX представлением элементов массива
        /// </summary>
        /// <param name="str"> Строка </param>
        /// <returns></returns>
        public static byte[] ArrayFromHexString( String str )
        {

            List<byte> list = new List<byte>( );
            for (int index = 0; index < str.Length; index += 2)
                list.Add(Byte.Parse(str[index] + "" + str[index + 1], NumberStyles.HexNumber));

            return list.ToArray( );
        }
    }
}
