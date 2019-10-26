#include "ps2_keyboard.h"
#include "cycle_queue.h"
#include "stm32f1xx_hal.h"


//******************************************************************************
// Таблица со строчными значениями кодов клавиатуры
//******************************************************************************
const uint8_t  PS2_KeyCodes[128] = {
   0,    F9,     0,      F5,     F3,     F1,     F2,     F12,    //0x00
   0,    F10,    F8,     F6,     F4,     TAB,    '`',    0,      //0x08
   0,    0,      L_SHFT, 0,      L_CTRL, 'q',    '1',    0,      //0x10
   0,    0,      'z',    's',    'a',    'w',    '2',    0,      //0x18
   0,    'c',    'x',    'd',    'e',    '4',    '3',    0,      //0x20
   0,    ' ',    'v',    'f',    't',    'r',    '5',    0,      //0x28
   0,    'n',    'b',    'h',    'g',    'y',    '6',    0,      //0x30
   0,    0,      'm',    'j',    'u',    '7',    '8',    0,      //0x38
   0,    ',',    'k',    'i',    'o',    '0',    '9',    0,      //0x40
   0,    '.',    '/',    'l',    ';',    'p',    '-',    0,      //0x48
   0,    0,      '\'',   0,      '[',    '=',    0,      0,      //0x50
   CAPS, R_SHFT  ,ENTER, ']',    0,      0x5c,   0,      0,      //0x58
   0,    0,      0,      0,      0,      0,      BKSP,   0,      //0x60
   0,    '1',    0,      '4',    '7',    0,      0,      0,      //0x68
   0,    '.',    '2',    '5',    '6',    '8',    ESC,    NUM,    //0x70
   F11,  '+',    '3',    '-',    '*',    '9',    0,      0       //0x78
};  /**< PS2 Keyboard ASCII Value LookUp Table. */
//******************************************************************************

//******************************************************************************
// Таблица с прописными значениями кодов клавиатуры при нажатии Shift
//******************************************************************************
const uint8_t  PS2_ShiftKeyCodes[128] = {
   0,    F9,     0,      F5,     F3,     F1,     F2,     F12,    //0x00
   0,    F10,    F8,     F6,     F4,     TAB,    '~',    0,      //0x08
   0,    0,      L_SHFT, 0,      L_CTRL, 'Q',    '!',    0,      //0x10
   0,    0,      'Z',    'S',    'A',    'W',    '@',    0,      //0x18
   0,    'C',    'X',    'D',    'E',    '$',    '#',    0,      //0x20
   0,    ' ',    'V',    'F',    'T',    'R',    '%',    0,      //0x28
   0,    'N',    'B',    'H',    'G',    'Y',    '^',    0,      //0x30
   0,    0,      'M',    'J',    'U',    '&',    '*',    0,      //0x38
   0,    '<',    'K',    'I',    'O',    ')',    '(',    0,      //0x40
   0,    '>',    '?',    'L',    ':',    'P',    '_',    0,      //0x48
   0,    0,      '\"',   0,      '{',    '+',    0,      0,      //0x50
   CAPS, R_SHFT, ENTER,  '}',    0,      '|',    0,      0,      //0x58
   0,    0,      0,      0,      0,      0,      BKSP,   0,      //0x60
   0,    '1',    0,      '4',    '7',    0,      0,      0,      //0x68
   0,    '.',    '2',    '5',    '6',    '8',    ESC,    NUM,    //0x70
   F11,  '+',    '3',    '-',    '*',    '9',    0,      0       //0x78
};  
//******************************************************************************

//******************************************************************************
// Глобальные переменные
//******************************************************************************
static PS2_State_e PS2_State; 
static PS2_Keyboard_s ps2;
//******************************************************************************

//******************************************************************************
// Инициализация состояния клавиатуры
//******************************************************************************
void PS2KeyboardInitState( void ){
   PS2_State = PS2_START;
   
   ps2.bit_pos   = 0;
   ps2.scan_code = 0;
   ps2.last_scan_code = 0;
   ps2.penultimate_scan_code = 0;
   ps2.penultimate_scan_code = 0;
   ps2.PS2_Busy = false;
   ps2.CapsLock = false;
   ps2.ShiftKey = false;
}
//******************************************************************************

//******************************************************************************
// Инициализация клавиатуры
//******************************************************************************
void PS2KeyboardInit() {
   GPIO_InitTypeDef clkStruct,dataStruct, extStruct;
   
   // Инициализация пина тактирования CLK
   clkStruct.Pin = PS2_CLK_PIN;
   clkStruct.Mode = GPIO_MODE_INPUT;
   clkStruct.Pull = GPIO_PULLUP;
   clkStruct.Speed = GPIO_SPEED_FREQ_HIGH;
   
   // Инициализация пина данных Data
   dataStruct.Pin  = PS2_DATA_PIN;
   dataStruct.Mode = GPIO_MODE_INPUT;
   dataStruct.Pull = GPIO_PULLUP;
   dataStruct.Speed = GPIO_SPEED_FREQ_HIGH;
   
   // Инициализация линии внешнего прерывания
   extStruct.Pin = EXT_INT_PIN;
   extStruct.Mode = GPIO_MODE_IT_FALLING;
   extStruct.Pull = GPIO_PULLUP;
   extStruct.Speed=GPIO_SPEED_FREQ_HIGH;
   
   HAL_GPIO_Init(PS2_CLK_PORT,  &clkStruct);
   HAL_GPIO_Init(PS2_DATA_PORT, &dataStruct);
   HAL_GPIO_Init(EXT_INT_PORT,  &extStruct);
   
   // Активация линии внешнего прерывания
   HAL_NVIC_SetPriority(EXT_INT_IRQn, 0, 0);
   HAL_NVIC_EnableIRQ(EXT_INT_IRQn);
   
   // Инициализация состояния клавиатуры
   PS2KeyboardInitState();
   InitQueue();
}
//******************************************************************************

//******************************************************************************
// Признак занятости клавиатуры
//******************************************************************************
bool IsQueueBusy() {
   return (ps2.PS2_Busy||IsQueueEmpty());
}
//******************************************************************************

int counter = 0;
//******************************************************************************
// Машина состояния обработки данных с пина данных
//******************************************************************************
void PS2StateMachine() {
   static GPIO_PinState  pinState; 
   
   // Определение значение на пине Data
   pinState = HAL_GPIO_ReadPin(PS2_DATA_PORT, PS2_DATA_PIN);
   
   switch (PS2_State)
   {
      default:
      //*********Начало скан кода***********************************************
      case PS2_START: {         
         // признак начала скан кода
         if( pinState == GPIO_PIN_RESET)
         {
            PS2_State++;
            ps2.PS2_Busy = true;
         }
         ps2.parity_value = 0;
         ps2.scan_code    = 0;
      } break;
      //*******Получение данных*************************************************
      case PS2_DATA: {
         ps2.parity_value = pinState ? (++ps2.parity_value):(ps2.parity_value);
         ps2.scan_code |= (pinState << ps2.bit_pos);
         ps2.bit_pos++;
         if( ps2.bit_pos >= 8 )
         {
            ps2.bit_pos = 0;
            PS2_State++;
         }
      } break;
      //******Проверка паритета*************************************************
      case PS2_PARITY: {
         if( pinState != (ps2.parity_value%2))
            PS2_State++;
         else
            PS2_State = PS2_START;
      } break;
      //*******Конец скан кода**************************************************
      case PS2_STOP: {    
   
         if( pinState == GPIO_PIN_SET )
         {     
            if (ps2.last_scan_code != ps2.scan_code || ps2.penultimate_scan_code != ps2.scan_code )
            {  
               ps2.penultimate_scan_code = ps2.last_scan_code;
               ps2.last_scan_code = ps2.scan_code;
               Enqueue(ps2.scan_code);
            }
         }
         PS2_State    = PS2_START;
         ps2.PS2_Busy = false;
      } break;
   }
}
//******************************************************************************


//******************************************************************************
// Получение нажатой клавиши 
//******************************************************************************
uint8_t GetKey( void ){
   uint8_t key_value = 0;
   uint8_t key_scan_code = 0u;
   key_scan_code = Dequeue ();
   
   switch(key_scan_code)
   {
      case 0xF0:
      {
         // игнорируем отжатие клавиш кроме shift 
         key_scan_code = Dequeue ();
         if( key_scan_code == L_SHFT || key_scan_code == R_SHFT )
         {
            if( ps2.ShiftKey )
               ps2.ShiftKey = false;
         }
      }break;
      
      default:
      {
         
         if( key_scan_code == L_SHFT || key_scan_code == R_SHFT )
         {
            ps2.ShiftKey = true;
            break;
         }
         
         if( key_scan_code == CAPS )
         {
            if( ps2.CapsLock )
               ps2.CapsLock = false;
            else
               ps2.CapsLock = true;
            break;
         }
         
         if( ps2.CapsLock )
            key_value = ps2.ShiftKey ? PS2_KeyCodes[key_scan_code]:PS2_ShiftKeyCodes[key_scan_code];
         else
            key_value = ps2.ShiftKey ? PS2_ShiftKeyCodes[key_scan_code]:PS2_KeyCodes[key_scan_code];
      }break;
   }
   return key_value;
}
//******************************************************************************

//******************************************************************************
// Получение нажатой клавиши 
//******************************************************************************
void GetScanCodes(uint8_t *buffer, uint16_t *length , uint16_t maxLength){
  
  uint8_t key = 0;
  uint8_t make_key = 0;
  uint16_t key_counter = 0;
  bool isMake = false;
  
  while(true){
    key = Dequeue();
        
    if(key == 0){  
      if( key_counter != 0){
        continue;
      } else { 
        *length = 0;
        return;
      }
    }
  

    buffer[key_counter] = key;
    key_counter++;
    
    if(key_counter >= maxLength){
      *length = key_counter;
      return;
    }
    
    if(key == 0xE0){
        continue;
    }
    
    // считали код нажатой клавиши
    if(key == 0xF0){
        isMake = true;
        continue;
    }
       
    if(!isMake){
        make_key = key;
        continue;
    }
    
    if(key == make_key){
       *length = key_counter;
    }else{
      *length = 0;
    }
           
    return;

  }
}
//******************************************************************************
