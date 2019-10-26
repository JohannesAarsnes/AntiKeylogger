
using AntiKeyloggerUI.Auxiliary;
using InkhSums;
using System;

namespace AntiKeyloggerUI.Model
{
    public class ExchangePacket
    {
        private const int HEADER_SIZE = 5;
        private const int HASH_SIZE = 4;
        private const int MIN_PACK_SIZE = HEADER_SIZE + HASH_SIZE;
        private const uint PACK_HASH_START_VALUE = 0xFFFFFFFF;

        private const int CODE_INDEX = 0;
        private const int PAYLOAD_LENGTH_LOW_INDEX = 1;
        private const int PAYLOAD_LENGTH_HIGH_INDEX = 2;
        private const int UNIQUES_LOW_INDEX = 3;
        private const int UNIQUES_HIGH_INDEX = 4;
        private const int PAYLOAD_INDEX = 5;

        #region Properties

        /// <summary>
        /// Код операции
        /// </summary>
        public byte Code { get; set; }

        /// <summary>
        /// Длина полезной нагрузки
        /// </summary>
        public ushort PayloadLength { get; private set; }

        /// <summary>
        /// Счетчика мастера
        /// </summary>
        public ushort Unique { get; set; }

        /// <summary>
        /// Полезная нагрузка пакета
        /// </summary>
        private byte[] payload;
        public byte[] Payload
        {
            get { return payload; }
            set
            {
                payload = value;
                if (payload != null)
                    PayloadLength = (ushort)payload.Length;
            }
        }

        private Crc32MirrIEEE_802_3 crc32;

        #endregion

        public ExchangePacket()
        {
            crc32 = new Crc32MirrIEEE_802_3();
        }

        public ExchangePacket(byte[] initData)
        {
            crc32 = new Crc32MirrIEEE_802_3();
            Code = initData[CODE_INDEX];

            PayloadLength = (ushort)(initData[PAYLOAD_LENGTH_LOW_INDEX] << 0);
            PayloadLength |= (ushort)(initData[PAYLOAD_LENGTH_LOW_INDEX] << 8);

            Unique = (ushort)(initData[UNIQUES_LOW_INDEX] << 0);
            Unique |= (ushort)(initData[UNIQUES_HIGH_INDEX] << 8);
        }

        /// <summary>
        /// Получить пакет в виде массива байт
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            byte[] packet = new byte[MIN_PACK_SIZE + PayloadLength];
            packet[CODE_INDEX] = Code;
            packet[PAYLOAD_LENGTH_LOW_INDEX] = (byte)((PayloadLength >> 0)&0xFF);
            packet[PAYLOAD_LENGTH_HIGH_INDEX] = (byte)((PayloadLength >> 8)&0xFF);

            packet[UNIQUES_LOW_INDEX] = (byte)((Unique >> 0) & 0xFF);
            packet[UNIQUES_HIGH_INDEX] = (byte)((Unique >> 8) & 0xFF);

            if(Payload != null)
            {
                Array.Copy(Payload,0, packet, PAYLOAD_INDEX, Payload.Length);
            }
            int hashIndex = (packet.Length - HASH_SIZE);
            uint hash = crc32.Final(crc32.Make(packet,0, (uint)hashIndex, PACK_HASH_START_VALUE));
            packet[hashIndex + 0] = (byte)((hash >> 0)&0xFF);
            packet[hashIndex + 1] = (byte)((hash >> 8)&0xFF);
            packet[hashIndex + 2] = (byte)((hash >> 16)&0xFF);
            packet[hashIndex + 3] = (byte)((hash >> 24)&0xFF);
            return packet;
        }

        /// <summary>
        /// Из массива байт попытаться сформировать пакет
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool SetBytes(byte[] data)
        {
            if (data == null || data.Length < MIN_PACK_SIZE)
                return false;

            byte code = data[CODE_INDEX];
            ushort length = 0, unique = 0;
            uint outputHash = 0;
            Payload = null;
            uint inputHash = crc32.Final(crc32.Make(data,0, (uint)(data.Length -  HASH_SIZE), PACK_HASH_START_VALUE)); 

            length = (ushort)(data[PAYLOAD_LENGTH_LOW_INDEX] << 0);
            length |=(ushort)(data[PAYLOAD_LENGTH_HIGH_INDEX] << 8);

            unique = (ushort)(data[UNIQUES_LOW_INDEX] << 0);
            unique |= (ushort)(data[UNIQUES_HIGH_INDEX] << 8);

            if (length != (data.Length - MIN_PACK_SIZE))
                return false;

            int hashIndex = HEADER_SIZE + length;
            byte[] payload = data.SubArray(PAYLOAD_INDEX, length);
            byte[] hash = data.SubArray(hashIndex, HASH_SIZE);

            outputHash |= (uint)(hash[0]<< 0);
            outputHash |= (uint)(hash[1]<< 8);
            outputHash |= (uint)(hash[2]<< 16);
            outputHash |= (uint)(hash[3]<< 24);

            if (inputHash != outputHash)
                return false;

            Code = code;
            PayloadLength = length;
            

            if (PayloadLength>0)
                Payload = payload;
            Unique = unique;

            return true;
        }

        public void Reset() {

            PayloadLength = 0;
            Payload = null;
            Code = 0;
            Unique = 0;
        }
    }
}
