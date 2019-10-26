
using System;
using System.Collections.Generic;

namespace ExchangeModel.Model
{
    public static class StaffingHelper
    {
        #region Константы

        /// <summary>
        /// Маркер начала пакета
        /// </summary>
        public const byte FEND = 0xC0;

        /// <summary>
        /// Маркер замены FEND
        /// </summary>
        public const byte FESC = 0xDB;

        /// <summary>
        /// Маркер замены FESC (первое значение)
        /// </summary>
        public const byte TFEND = 0xDC;

        /// <summary>
        /// Маркер замены FESC (второе значение)
        /// </summary>
        public const byte TFESC = 0xDD;

        #endregion


        /// <summary>
        /// Стаффинг
        /// </summary>
        public static byte[] Staffing(byte[] data)
        {
            if (data == null || data.Length == 0)
                return null;

            List<byte> convertedList = new List<byte>();
            //
            // Начало пакета
            convertedList.Add(FEND);
            //
            // Изменяем значения
            for (int idx = 0; idx < data.Length; idx++)
            {
                byte tmpByte = data[idx];
                //
                // Определили 0xC0
                if (tmpByte == FEND)
                {
                    convertedList.Add(FESC);
                    convertedList.Add(TFEND);
                    continue;
                }

                //
                // Определили 0xDB
                if (tmpByte == FESC)
                {
                    convertedList.Add(FESC);
                    convertedList.Add(TFESC);
                    continue;
                }

                convertedList.Add(tmpByte);
            }
            return convertedList.ToArray();
        }
    }
}
