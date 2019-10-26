#ifndef crc_algorithms_h
#define crc_algorithms_h
  
  #include <stdint.h>
  
  #ifdef CRC8_MAXIM
    uint8_t crc8_MAXIM(const uint8_t* data, unsigned int len, uint8_t sum);
    uint8_t crc8_MAXIM_step(uint8_t data, uint8_t sum);
  #endif

  #ifdef CRC16_ANSI_M
    uint16_t crc16_ANSI(const uint8_t* data, unsigned int len, uint16_t sum);
    uint16_t crc16_ANSI_step(uint8_t data, uint16_t sum);
  #endif

  #ifdef CRC16_CCITT
     uint16_t crc16_CCITT(const uint8_t* data, unsigned int len, uint16_t sum);
     uint16_t crc16_CCITT_step(uint8_t data, uint16_t sum);
  #endif

  #ifdef CRC16_CCITT_BITWISE	
    //***********************************************
    //* УВАГА!!! Початкове значення для побітового розрахунку CRC16 суми не є тим
    //* самим що й для побайтового розрахунку!
    //* Для перетворення значень придатних для цих двох алгоритмів слід 
    //* використовувати функції table_to_bitwise_init та bitwise_to_table_init.
    //***********************************************
    uint16_t crc16_CCITT_bitwise(const uint8_t *data, unsigned int len, uint16_t sum);
    
    //кінцева обробка контр. суми щоб отримати результат аналогічний 
    //результату функцій crc16_CCITT і crc16_CCITT_step
    uint16_t crc16_CCITT_bitwise_end(uint16_t sum);
  #endif

  #ifdef CRC32_IEEE_802_3
    uint32_t crc32_IEEE_802(const uint8_t *data, unsigned int len, uint32_t sum);
    uint32_t crc32_IEEE_802_step(uint8_t data, uint32_t sum);
  #endif

  #ifdef CRC16_INITIAL_FUNCTIONS
    //Перетворення початкового значення CRC16 що використовується у табличному 
    //алгоритмі на початкове значення що використовується у побітовому алгоритмі
    //обчислення CRC16.
    //bitwise - початкове значення для табличного алгоритма
    //poly - поліном що використовуєтья для розрахунку CRC16
    uint16_t table_to_bitwise_init(uint16_t table, uint16_t poly);

    //Перетворення початкового значення CRC16 що використовується у побітовому 
    //алгоритмі на початкове значення що використовується у табличному алгоритмі
    //обчислення CRC16
    //bitwise - початкове значення для побітового алгоритма
    //poly - поліном що використовуєтья для розрахунку CRC16
    uint16_t bitwise_to_table_init(uint16_t bitwise, uint16_t poly);
  #endif

#endif