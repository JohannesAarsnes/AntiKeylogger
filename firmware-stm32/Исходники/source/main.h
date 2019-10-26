#ifndef __MAIN_H__
#define __MAIN_H__
#include "stm32f1xx_hal.h"
#include "stdint.h"

//******************************************************************************
// This function is executed in case of error occurrence.
//******************************************************************************
void Error_Handler(void);
//******************************************************************************
void ProcessUSBReceiveData(uint8_t *data, uint16_t len);
void ProcessRequest();
void InitExchanger();
void GenerateCommonKey();
//******************************************************************************
// Error handler alias
//******************************************************************************

//******************************************************************************
#endif /* __MAIN_H__ */

