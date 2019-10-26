
using InkhSums;
using System;
using System.Collections.Generic;

namespace AntiKeyloggerUI.Auxiliary
{
    public static class WakeConvertHelper
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

        /// <summary>
        /// Начальное значение CRC
        /// </summary>
        public const uint CRC_START_VALUE = 0xFFFFFFFF;


        /// <summary>
        /// Размер заголовка пакета
        /// </summary>
        private const byte HEAD_MIN_RAW_SIZE = 8;


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
        private const byte IND_LEN = 4;

        /// <summary>
        /// Второй байт размера пакета
        /// </summary>
        private const byte IND_LEN1 = 5;
        #endregion

        /// <summary>
        /// Дестаффинг
        /// </summary>
        public static byte[] DeStaffing(byte[] data)
        {
            if (data == null || data.Length == 0)
                return null;

            Crc32MirrIEEE_802_3 crcAlg = new Crc32MirrIEEE_802_3();
            List<byte> convertedList = new List<byte>();
            uint crc32 = CRC_START_VALUE;
            bool isReceive = false;
            bool destaff = false;
            bool isStart = false;

            for (int idx = 0; idx < data.Length; idx++)
            {
                byte tmpByte = data[idx];
                //
                // Определили начало пакета 
                if (tmpByte == FEND)
                {
                    // список должен быть пустым
                    convertedList.Clear();
                    // признак готовности полученного пакета
                    isReceive = false;
                    // признак того что уже получен стартовый байт
                    isStart = true;
                    continue;
                }
                //
                // Пока не получим стартовый байт все байты игнорируются
                if (!isStart)
                    continue;
                //
                // Байт дестафинга
                if (tmpByte == FESC)
                {
                    destaff = true;
                    continue;
                }
                //
                // Определение последовательности дестафинга
                if (destaff)
                {
                    destaff = false;
                    //
                    // если заменяем 0xC0
                    if (tmpByte == TFEND)
                        convertedList.Add(FEND);
                    //
                    // если заменяем 0xDB
                    else if (tmpByte == TFESC)
                        convertedList.Add(FESC);
                    // не корректное значение (нет стаффинга)
                    else
                    {
                        convertedList.Clear();
                        isReceive = false;
                        isStart = false;
                        continue;
                    }
                }
                else
                    convertedList.Add(tmpByte);
            }

            int size = 0;
            //
            // Получен заголовок пакета, определение длины данных
            if (convertedList.Count >= HEAD_MIN_RAW_SIZE)
                size = ((convertedList[IND_LEN1] << 8) | convertedList[IND_LEN]);
            //
            // Получение всех данных
            if (convertedList.Count <= (HEAD_MIN_RAW_SIZE + size))
                crc32 = crcAlg.Step(convertedList[convertedList.Count - 1], crc32);
            //
            // Получили все данные
            if (convertedList.Count == (HEAD_MIN_RAW_SIZE + size + CRC_RAW_SIZE))
            {
                //
                // Определяем содержимое пакета
                if (crc32 == BitConverter.ToUInt32(convertedList.ToArray(), convertedList.Count - 4))
                {
                    //Удаление контрольной суммы
                    convertedList.RemoveRange((convertedList.Count - 4), 4);
                    isReceive = true;
                    isStart = false;
                    return convertedList.ToArray();
                }
                else
                {
                    convertedList.Clear();
                    crc32 = CRC_START_VALUE;
                    isReceive = false;
                    isStart = false;
                }
            }

            return null;
        }

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
