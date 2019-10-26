#ifndef CUSTOM_AES_196CBC_H
#define CUSTOM_AES_196CBC_H
#include "crypto.h"

//******************************************************************************
#define KEY_LENGTH              (CRL_AES192_KEY)    // длина ключа
#define VECTOR_LENGTH           (CRL_AES_BLOCK)     // длина блока вектора
//******************************************************************************

//******************************************************************************
// Ключ и вектор шифрования 
//******************************************************************************
extern uint8_t Key[CRL_AES192_KEY];
extern uint8_t Vector[CRL_AES_BLOCK];
//******************************************************************************

//******************************************************************************
// Шифрация данных
//******************************************************************************
int32_t Encrypt(uint8_t*  Input, uint32_t  InputLength,
                              uint8_t  *Key, uint8_t  *Vector, uint32_t  IvLength,
                              uint8_t  *Output, uint32_t *OutputLength);
//******************************************************************************

//******************************************************************************
// Дешифрация данных
//******************************************************************************
int32_t Decrypt(uint8_t*  Input, uint32_t  InputLength, 
                              uint8_t  *Key, uint8_t  *Vector, uint32_t  IvLength,
                              uint8_t  *Output, uint32_t *OutputLength);
//******************************************************************************

int32_t RSA_Encrypt(RSApubKey_stt *P_pPubKey,
                    const uint8_t *P_pInputMessage,
                    int32_t P_InputSize,
                    uint8_t *P_pOutput);

int32_t RSA_Decrypt(RSAprivKey_stt * P_pPrivKey,
                    const uint8_t * P_pInputMessage,
                    uint8_t *P_pOutput,
                    int32_t *P_OutputSize);

#endif //CUSTOM_AES_196CBC_H