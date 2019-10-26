#include "main.h"
#include "usb_device.h"
#include "ps2_keyboard.h"

//******************************************************************************
//
//******************************************************************************
#define LED_Pin GPIO_PIN_13
#define LED_GPIO_Port GPIOC
//******************************************************************************

//******************************************************************************
// 
//******************************************************************************
CRC_HandleTypeDef hcrc;
extern USBD_HandleTypeDef hUsbDeviceFS;
bool isUSBConnected =  false;
bool updateCommonKey =  true;
//******************************************************************************

//******************************************************************************
// Прерывание на линии CLK клавиатуры
//******************************************************************************
void HAL_GPIO_EXTI_Callback(uint16_t GPIO_Pin) {
  if(!isUSBConnected)
    return;
  
  if(GPIO_Pin == PS2_CLK_PIN) {
    PS2StateMachine();
    HAL_GPIO_TogglePin(LED_GPIO_Port, LED_Pin);
  }
}
//******************************************************************************

//******************************************************************************
// Инициализация модуля crc
//******************************************************************************
static void MX_CRC_Init(void) {
  hcrc.Instance = CRC;
  if (HAL_CRC_Init(&hcrc) != HAL_OK) {
    Error_Handler();
  }
}
//******************************************************************************

//******************************************************************************
//Configure pins
//******************************************************************************
static void MX_GPIO_Init(void) {
  GPIO_InitTypeDef GPIO_InitStruct;
  
  __HAL_RCC_GPIOA_CLK_ENABLE();
  __HAL_RCC_GPIOB_CLK_ENABLE();
  __HAL_RCC_GPIOC_CLK_ENABLE();
  __HAL_RCC_GPIOD_CLK_ENABLE();

  HAL_GPIO_WritePin(LED_GPIO_Port, LED_Pin, GPIO_PIN_SET);
  
  GPIO_InitStruct.Pin = LED_Pin;
  GPIO_InitStruct.Mode = GPIO_MODE_OUTPUT_PP;
  GPIO_InitStruct.Pull = GPIO_NOPULL;
  GPIO_InitStruct.Speed = GPIO_SPEED_FREQ_LOW;
  HAL_GPIO_Init(LED_GPIO_Port, &GPIO_InitStruct);
  
}
//******************************************************************************

//******************************************************************************
// System Clock Configuration
//******************************************************************************
void SystemClock_Config(void) {
  RCC_OscInitTypeDef RCC_OscInitStruct;
  RCC_ClkInitTypeDef RCC_ClkInitStruct;
  RCC_PeriphCLKInitTypeDef PeriphClkInit;
  
  /**Initializes the CPU, AHB and APB busses clocks 
  */
  RCC_OscInitStruct.OscillatorType = RCC_OSCILLATORTYPE_HSE;
  RCC_OscInitStruct.HSEState = RCC_HSE_ON;
  RCC_OscInitStruct.HSEPredivValue = RCC_HSE_PREDIV_DIV1;
  RCC_OscInitStruct.HSIState = RCC_HSI_ON;
  RCC_OscInitStruct.PLL.PLLState = RCC_PLL_ON;
  RCC_OscInitStruct.PLL.PLLSource = RCC_PLLSOURCE_HSE;
  RCC_OscInitStruct.PLL.PLLMUL = RCC_PLL_MUL9;
  if (HAL_RCC_OscConfig(&RCC_OscInitStruct) != HAL_OK)
  {
    Error_Handler();
  }
  
  /**Initializes the CPU, AHB and APB busses clocks 
  */
  RCC_ClkInitStruct.ClockType = RCC_CLOCKTYPE_HCLK|RCC_CLOCKTYPE_SYSCLK
    |RCC_CLOCKTYPE_PCLK1|RCC_CLOCKTYPE_PCLK2;
  RCC_ClkInitStruct.SYSCLKSource = RCC_SYSCLKSOURCE_PLLCLK;
  RCC_ClkInitStruct.AHBCLKDivider = RCC_SYSCLK_DIV1;
  RCC_ClkInitStruct.APB1CLKDivider = RCC_HCLK_DIV2;
  RCC_ClkInitStruct.APB2CLKDivider = RCC_HCLK_DIV1;
  
  if (HAL_RCC_ClockConfig(&RCC_ClkInitStruct, FLASH_LATENCY_1) != HAL_OK)
  {
    Error_Handler();
  }
  
  PeriphClkInit.PeriphClockSelection = RCC_PERIPHCLK_USB;
  PeriphClkInit.UsbClockSelection = RCC_USBCLKSOURCE_PLL_DIV1_5;
  if (HAL_RCCEx_PeriphCLKConfig(&PeriphClkInit) != HAL_OK)
  {
    Error_Handler();
  }
  
  HAL_SYSTICK_Config(HAL_RCC_GetHCLKFreq()/1000);
  HAL_SYSTICK_CLKSourceConfig(SYSTICK_CLKSOURCE_HCLK);
  HAL_NVIC_SetPriority(SysTick_IRQn, 0, 0);
  
  HAL_NVIC_SetPriority(USB_LP_CAN1_RX0_IRQn, 0, 0);
  HAL_NVIC_EnableIRQ(USB_LP_CAN1_RX0_IRQn);
 
  HAL_NVIC_SetPriority(USB_HP_CAN1_TX_IRQn, 0, 0);
  HAL_NVIC_EnableIRQ(USB_HP_CAN1_TX_IRQn);

  HAL_NVIC_SetPriority(RCC_IRQn, 0, 0);
  HAL_NVIC_EnableIRQ(RCC_IRQn);

}
//******************************************************************************

//******************************************************************************
// This function is executed in case of error occurrence.
//******************************************************************************
void Error_Handler() { while(1){}}
//******************************************************************************


//******************************************************************************
// Entry point to the program 
//******************************************************************************
int main(void)
{
  HAL_Init();
  SystemClock_Config();
  MX_GPIO_Init();
  MX_USB_DEVICE_Init();
  MX_CRC_Init();
  PS2KeyboardInit();
  InitExchanger();
  while (1){ 
    if(hUsbDeviceFS.dev_state == USBD_STATE_CONFIGURED){
      isUSBConnected =  true;
    }else{
        if(isUSBConnected)
          updateCommonKey = true;
        
        isUSBConnected = false;
    }

    ProcessRequest();
    
    if(updateCommonKey){
      GenerateCommonKey();
      updateCommonKey = false;
    }
  }
}
//******************************************************************************

