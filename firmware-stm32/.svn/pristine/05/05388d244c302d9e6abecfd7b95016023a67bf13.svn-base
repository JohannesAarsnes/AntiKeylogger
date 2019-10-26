#include "exchanger.h"
#include "crc.h"
#include "custom_aes196cbc.h"
#include "usbd_cdc_if.h"

//******************************************************************************
// Определение для обмена
//******************************************************************************
#define RX_BUFFER_SIZE           (100)
#define TX_BUFFER_SIZE           (100)
#define COMMON_KEY_SIZE          (24)
#define MAX_REPORT_SIZE          (6)

#define INIT_PACK_HASH_INDEX     (sizeof(HEADER) + COMMON_KEY_SIZE)
#define INIT_BUFFER_SIZE         (INIT_PACK_HASH_INDEX   + sizeof(CHECKSUM))
#define REPORT_BUFFER_SIZE       (sizeof(HEADER) + MAX_REPORT_SIZE + sizeof(CHECKSUM))
//******************************************************************************

//******************************************************************************
// Буфферы обмена
//******************************************************************************
uint8_t g_u8ReceiveBuffer[RX_BUFFER_SIZE];
uint8_t g_u8TransmitBuffer[TX_BUFFER_SIZE];

uint8_t g_u8InitBuffer[INIT_BUFFER_SIZE];
uint8_t g_u8ReportBuffer[REPORT_BUFFER_SIZE];
//******************************************************************************




uint32_t a = 45;
uint32_t c = 21;
uint32_t m = 67;
uint32_t seed = 2;

//******************************************************************************
//
//******************************************************************************
bool isReceivePack;
uint16_t masterCounter;
STAFFING_RESTORER receiver;
//******************************************************************************

const uint8_t Modulus[] =
{
  0xB5, 0x05, 0xFC, 0xA2, 0xCA, 0x33, 0x48, 0xD5, 0x9B, 0xF3, 0x00, 0x5F, 0x7C, 0xFD, 0xC4, 0x56, 0x4C, 0x25, 0x07,
  0x67, 0xE9, 0xC9, 0x40, 0x24, 0x69, 0x79, 0x61, 0x41, 0x98, 0x1D, 0x6A, 0xF5, 0x6A, 0x1A, 0x84, 0xB5, 0xA9, 0xA4,
  0xB3, 0x33, 0x5F, 0xA0, 0x25, 0xA8, 0x7F, 0x4B, 0x4D, 0x0B, 0xA0, 0x60, 0xB8, 0xBE, 0xF9, 0x34, 0x0B, 0xE4, 0x5F,
  0xDB, 0x05, 0x76, 0x20, 0x38, 0x90, 0xA0, 0x71, 0xCE, 0xE9, 0xB0, 0x59, 0x4B, 0x95, 0x12, 0x7B, 0xB4, 0x80, 0xED,
  0xC7, 0x43, 0xBD, 0xCE, 0x27, 0xFD, 0x2B, 0xEC, 0xD0, 0x33, 0x00, 0x24, 0x32, 0x9E, 0xED, 0xAF, 0x3C, 0x1A, 0x12,
  0x13, 0xB2, 0x8D, 0x32, 0xD1, 0x83, 0xEA, 0xF4, 0x1A, 0x9A, 0x46, 0x3A, 0x08, 0x8C, 0xD4, 0xBA, 0x67, 0xDA, 0x91,
  0x26, 0x79, 0x49, 0xBA, 0xAA, 0x54, 0x26, 0x56, 0x03, 0x76, 0xA7, 0x70, 0x58, 0x9E, 0xA8, 0x37, 0x60, 0xB8, 0xC5,
  0xC1, 0xF9, 0xDD, 0x54, 0x18, 0x4D, 0x7F, 0x91, 0xCC, 0x0A, 0xBB, 0x08, 0xC3, 0x05, 0x3C, 0x04, 0x8B, 0xDC, 0xD0,
  0xE9, 0x7A, 0x16, 0x28, 0x53, 0x0D, 0x20, 0x74, 0x0B, 0xD1, 0xD5, 0x0F, 0x16, 0x48, 0x06, 0xB2, 0x5F, 0x1E, 0x0A,
  0xC9, 0xDD, 0x9E, 0x17, 0xE5, 0x00, 0xD6, 0xB9, 0x2D, 0x40, 0xE6, 0xA8, 0xDC, 0x7F, 0xAE, 0x5B, 0x6B, 0x7F, 0x76,
  0x27, 0xF7, 0xED, 0x0C, 0xF5, 0x1D, 0xC1, 0x6F, 0xA4, 0x00, 0x45, 0x8A, 0x22, 0x09, 0x84, 0xD1, 0xB4, 0xB1, 0x18,
  0x44, 0x76, 0xC9, 0xD6, 0xA7, 0xC6, 0x72, 0x5B, 0x43, 0x48, 0x91, 0x85, 0xBB, 0x7F, 0xB1, 0x44, 0x73, 0x45, 0xF5,
  0x5A, 0x7E, 0x72, 0x3D, 0xA1, 0x8C, 0x43, 0xAE, 0x83, 0xD9, 0xB4, 0xCB, 0x1D, 0xDC, 0x26, 0x3F, 0x7F, 0x1E, 0xFE,
  0x83, 0x6C, 0x9A, 0x0D, 0xEA, 0xE1, 0x94, 0x55, 0xF1
};

const uint8_t PublicExponent[] =
{
  0x01, 0x00, 0x01
};

RSApubKey_stt PubKey_st;


//******************************************************************************
// Применение стаффинга
//******************************************************************************
uint16_t Transmit(uint8_t *data, uint16_t lenght){
  
  if((data == NULL) || TX_BUFFER_SIZE < (lenght*2))
    return 0;
  
  uint8_t *buffer = g_u8TransmitBuffer;
  uint16_t iterator = 0;
  buffer[iterator++] = FEND;
  
  for(int idx  = 0; idx < lenght; idx++){
    uint8_t byte =  data[idx];
    
    if(byte == FEND) {											                        			  
      buffer[iterator++] = FESC;								    			  
      buffer[iterator++] = TFEND;								    			  
    }                                                                   			  
    else if(byte== FESC) {											                        			  
      buffer[iterator++] = FESC;							        			  
      buffer[iterator++] = TFESC;								    			  
    }else {																    			  
      buffer[iterator++] = byte;										  
    }             
  }
  
  if(iterator > 0){
      CDC_Transmit_FS(g_u8TransmitBuffer, iterator);
  }
  return iterator;
}
//******************************************************************************

//******************************************************************************
//Получить случайное число
//******************************************************************************
uint32_t GetRand(){
  seed = (a * seed + c) % m;
  return seed;
}
//******************************************************************************

//******************************************************************************
//Получить случайное число
//******************************************************************************
void GenerateCommonKey(){
  uint8_t* buffer = &g_u8InitBuffer[PACK_DATA_INDEX];
  for(int idx = 0; idx  < COMMON_KEY_SIZE; idx++){
    buffer[idx] = GetRand();
  }
}
//******************************************************************************

//******************************************************************************
// Обновлние пакета инициализации
//******************************************************************************
void UpdateInitPack(){
     // код ответа
  g_u8InitBuffer[PACK_CODE_INDEX] = POSITIVE;
  
  // длина данных
  uint16_t length = COMMON_KEY_SIZE;
  g_u8InitBuffer[PACK_SIZE_LOW_INDEX] =((length >> 0)&0xFF);
  g_u8InitBuffer[PACK_SIZE_HIGH_INDEX] =((length >> 8)&0xFF);
  
  // уникальное число от мастера
  g_u8InitBuffer[PACK_COUNTER_LOW_INDEX] = ((masterCounter >> 0)&0xFF);
  g_u8InitBuffer[PACK_COUNTER_HIGH_INDEX] =((masterCounter >> 8)&0xFF);
  
  // контрольная сумма пакета
  uint32_t hash = crc32_IEEE_802(g_u8InitBuffer,INIT_PACK_HASH_INDEX,PACK_SUM_START);
  
  g_u8InitBuffer[INIT_PACK_HASH_INDEX + 0] =((hash >> 0)&0xFF);
  g_u8InitBuffer[INIT_PACK_HASH_INDEX + 1] =((hash >> 8)&0xFF);
  g_u8InitBuffer[INIT_PACK_HASH_INDEX + 2] =((hash >> 16)&0xFF);
  g_u8InitBuffer[INIT_PACK_HASH_INDEX + 3] =((hash >> 24)&0xFF);
}
//******************************************************************************

//******************************************************************************
// Обновлние пакета инициализации
//******************************************************************************
void UpdateReportPack(){
   // код ответа
  g_u8ReportBuffer[PACK_CODE_INDEX] = POSITIVE;
  
  // длина данных
  uint16_t length = 0;
  GetScanCodes(&g_u8ReportBuffer[PACK_DATA_INDEX],&length,MAX_REPORT_SIZE);
  
  g_u8ReportBuffer[PACK_SIZE_LOW_INDEX] =((length >> 0)&0xFF);
  g_u8ReportBuffer[PACK_SIZE_HIGH_INDEX] =((length >> 8)&0xFF);
  
  // уникальное число от мастера
  g_u8ReportBuffer[PACK_COUNTER_LOW_INDEX] = ((masterCounter >> 0)&0xFF);
  g_u8ReportBuffer[PACK_COUNTER_HIGH_INDEX] =((masterCounter >> 8)&0xFF);
    
  uint32_t hashIndex = sizeof(HEADER) + length;
  // контрольная сумма пакета
  uint32_t hash = crc32_IEEE_802(g_u8ReportBuffer,hashIndex, PACK_SUM_START);
  
  // запись контрольной суммы
  g_u8ReportBuffer[hashIndex + 0] =((hash >> 0)&0xFF);
  g_u8ReportBuffer[hashIndex + 1] =((hash >> 8)&0xFF);
  g_u8ReportBuffer[hashIndex + 2] =((hash >> 16)&0xFF);
  g_u8ReportBuffer[hashIndex + 3] =((hash >> 24)&0xFF);
}
//******************************************************************************

//******************************************************************************
// Обработка запроса инициализации
//******************************************************************************
void Initialize(const uint8_t *payload, uint16_t length){

  if((payload == NULL))
   return;
  
  // Очистка очереди перед новой сессией
  while(GetKey() != 0){
  }
  
  UpdateInitPack();
#warning тут необходимо зашифровать пакет публичным ключом который передан в payload
  Transmit(g_u8InitBuffer,(uint16_t)INIT_BUFFER_SIZE);
}
//******************************************************************************

//******************************************************************************
// Обработка запроса получения отчета
//******************************************************************************
void GetReport(uint8_t *payload, uint16_t length){
    
  if((payload == NULL) || (length != VECTOR_LENGTH))
   return;
  
  UpdateReportPack();
#warning тут необходимо зашифровать пакет общим ключом и передать на стаффинг  
  Transmit(g_u8ReportBuffer, (uint16_t)REPORT_BUFFER_SIZE);
}
//******************************************************************************

//******************************************************************************
// Обработчик полученных пакетов
//******************************************************************************
void ProcessRequest(){
  
  // нет данных для обработки
  if(!isReceivePack)
    return;
  
  uint8_t *packet = g_u8ReceiveBuffer;
  uint16_t length  = 0;
  masterCounter = 0;
  
  length |=(packet[PACK_SIZE_LOW_INDEX] >> 0);
  length |=(packet[PACK_SIZE_HIGH_INDEX] >> 8);
  
  masterCounter |=(packet[PACK_COUNTER_LOW_INDEX] >> 0);
  masterCounter |=(packet[PACK_COUNTER_HIGH_INDEX] >> 8);
  
  uint8_t *payload = &packet[PACK_DATA_INDEX]; 
  switch(packet[PACK_CODE_INDEX]){
  case INIT: {
    Initialize(payload, length);
  }break;
  case REPORT: {
    GetReport(payload, length);
  }break;
  default: break;
  }
  isReceivePack = false;
}
//******************************************************************************

//******************************************************************************
// Обработка полученных по usb данных
//******************************************************************************
void ProcessUSBReceiveData(uint8_t *data, uint16_t len){
  
  if(isReceivePack)
    return;
  
  uint8_t byte = 0;
  for(int idx = 0; idx < len; idx++){
    byte =  data[idx];
    // если начало пакета					
    if(byte == FEND) 
    {
      RESET_STAFFING_RESTORER(receiver);
      receiver.isPackStartMarker = true;		
      continue;
    }		
    
    // игнор всех байтов если не получили стартовый 0xC0
    if(!receiver.isPackStartMarker)
    {	continue; 	}
    
    // получили признак стафинга
    if( byte == FESC) {
      receiver.isFrameMarker = true;
      continue;
    }
    
    // если просто байт добавляем в буфер
    if(!receiver.isFrameMarker) {	
      g_u8ReceiveBuffer[receiver.recvCount++] = byte;
    }
    else {   // определение следующего байт после начала frame
      switch(byte) {
      case TFEND: g_u8ReceiveBuffer[receiver.recvCount++] = FEND; break;  
      case TFESC: g_u8ReceiveBuffer[receiver.recvCount++] = FESC; break;
      default:  goto CLEAR_INCOMING; break;
      }
      // frame обработан
      receiver.isFrameMarker = false;
    }
    
    // получили заголовок
    if(receiver.recvCount == sizeof(HEADER)) {
      // определяем полный размер пакета
      receiver.pack_size = (g_u8ReceiveBuffer[PACK_SIZE_LOW_INDEX])|(g_u8ReceiveBuffer[PACK_SIZE_HIGH_INDEX]<<8);
      receiver.pack_size += PACK_MIN_SIZE;
      
      // для ретранслируемого пакета не нужно проверять буфер
      if (receiver.pack_size > RX_BUFFER_SIZE){
        goto CLEAR_INCOMING;
      }
      
      // проверка на минимальное значение
      if (receiver.pack_size < PACK_MIN_SIZE){
        goto CLEAR_INCOMING;
      }
    }
    
    // пока не получим всех данных, расчитываем контрольную сумму
    if(receiver.recvCount<=(receiver.pack_size -sizeof(CHECKSUM))) {
      receiver.in_hash = crc32_IEEE_802_step(g_u8ReceiveBuffer[receiver.recvCount-1], receiver.in_hash); 
      continue;
    }
    
    // проверка контрольной суммы
    if ((receiver.in_hash&0xff)!=(g_u8ReceiveBuffer[receiver.recvCount-1])){
      goto CLEAR_INCOMING;
    }
    
    // следующий байт контрольной суммы
    receiver.in_hash >>= 8;
    
    // пока не получим все данные
    if (receiver.recvCount < receiver.pack_size)
      continue;			
    
    isReceivePack = true;
    
    // завершение процедуры приема 
  CLEAR_INCOMING:
    RESET_STAFFING_RESTORER(receiver); 
  }
}
//******************************************************************************


void InitExchanger(){
  
  //  PubKey_st.mExponentSize = sizeof(PublicExponent);
  //  PubKey_st.mModulusSize = sizeof(Modulus);
  //  PubKey_st.pmExponent = (uint8_t *) PublicExponent;
  //  PubKey_st.pmModulus = (uint8_t *)Modulus;
}