#include "custom_aes196cbc.h"
#include "main.h"

//******************************************************************************
// Ключ и вектор шифрования 
//******************************************************************************
uint8_t Key[CRL_AES192_KEY];
uint8_t Vector[CRL_AES_BLOCK];
//******************************************************************************
uint8_t preallocated_buffer[4096]; /* buffer required for internal allocation of memory */
uint8_t entropy_data[32] =
  {
    0x91, 0x20, 0x1a, 0x18, 0x9b, 0x6d, 0x1a, 0xa7,
    0x0e, 0x69, 0x57, 0x6f, 0x36, 0xb6, 0xaa, 0x88,
    0x55, 0xfd, 0x4a, 0x7f, 0x97, 0xe9, 0x72, 0x69,
    0xb6, 0x60, 0x88, 0x78, 0xe1, 0x9c, 0x8c, 0xa5
  };
//******************************************************************************
// AES CBC Encryption
// error status: can be AES_SUCCESS if success or one of AES_ERR_BAD_INPUT_SIZE, 
// AES_ERR_BAD_OPERATION, AES_ERR_BAD_CONTEXT AES_ERR_BAD_PARAMETER if error occured.
//******************************************************************************
int32_t Encrypt(uint8_t* Input, uint32_t InputLength,
                              uint8_t  *Key, uint8_t  *Vector, uint32_t  IvLength,
                              uint8_t  *Output, uint32_t *OutputLength) 
{
  
  AESCBCctx_stt AESctx;
  AESctx.mFlags = E_SK_DEFAULT;
  AESctx.mKeySize = 24;
  AESctx.mIvSize = IvLength;
  
  uint32_t error_status = AES_SUCCESS;
  int32_t outputLength = 0;
  
  error_status = AES_CBC_Encrypt_Init(&AESctx, Key, Vector );
  if (error_status != AES_SUCCESS) {
    return error_status;
  }
  
  error_status = AES_CBC_Encrypt_Append(&AESctx, Input, InputLength, Output, &outputLength);
  if (error_status != AES_SUCCESS) {
    return error_status;
  }
  *OutputLength = outputLength;
  
  error_status = AES_CBC_Encrypt_Finish(&AESctx, Output+ *OutputLength, &outputLength);
  *OutputLength += outputLength;
  return error_status;
}
//******************************************************************************

//******************************************************************************
// AES CBC Decryption 
// error status: can be AES_SUCCESS if success or one of AES_ERR_BAD_INPUT_SIZE, 
// AES_ERR_BAD_OPERATION, AES_ERR_BAD_CONTEXT AES_ERR_BAD_PARAMETER if error occured.
//******************************************************************************
int32_t Decrypt(uint8_t* Input, uint32_t InputLength,
                              uint8_t  *Key, uint8_t  *Vector, uint32_t  IvLength,
                              uint8_t  *Output, uint32_t *OutputLength) 
{
  AESCBCctx_stt AESctx;
  AESctx.mFlags = E_SK_DEFAULT;
  AESctx.mKeySize = KEY_LENGTH;
  AESctx.mIvSize = IvLength;
  
  int32_t outputLength = 0;
  uint32_t error_status = AES_SUCCESS;
  
  error_status = AES_CBC_Decrypt_Init(&AESctx, Key, Vector );
  if (error_status != AES_SUCCESS) {
    return error_status;
  }
  
  error_status = AES_CBC_Decrypt_Append(&AESctx, Input, InputLength, Output, &outputLength);
  if (error_status == AES_SUCCESS) {
    return error_status;
  }
  *OutputLength = outputLength;
  
  error_status = AES_CBC_Decrypt_Finish(&AESctx, Output+ *OutputLength, &outputLength);
  *OutputLength += outputLength;
  return error_status;
}
//******************************************************************************

//void Testing(){
//  int32_t status = AES_SUCCESS;
// /* Encrypt DATA with AES in CBC mode */
//  status = STM32_AES_CBC_Encrypt( (uint8_t *) Plaintext, PLAINTEXT_LENGTH, Key, IV, sizeof(IV), OutputMessage, &OutputMessageLength);
// 
//  if (status == AES_SUCCESS) {
//  }
//  else {
//    Error_Handler();
//  }
//
//  /* Decrypt DATA with AES in CBC mode */
//  status = STM32_AES_CBC_Decrypt( (uint8_t *) Expected_Ciphertext, PLAINTEXT_LENGTH, Key, IV, sizeof(IV), OutputMessage, &OutputMessageLength);
// 
//  if (status == AES_SUCCESS){
//  }
//  else {
//    Error_Handler();
//  }
//}

//******************************************************************************
//  RSA Encryption with PKCS#1v1.5
//  error status: can be RSA_SUCCESS if success or one of
// RSA_ERR_BAD_PARAMETER, RSA_ERR_MESSAGE_TOO_LONG, RSA_ERR_BAD_OPERATION
//******************************************************************************
int32_t RSA_Encrypt(RSApubKey_stt *P_pPubKey,
                    const uint8_t *P_pInputMessage,
                    int32_t P_InputSize,
                    uint8_t *P_pOutput)
{
  int32_t status = RNG_SUCCESS ;
  RNGstate_stt RNGstate;
  RNGinitInput_stt RNGinit_st;
  RNGinit_st.pmEntropyData = entropy_data;
  RNGinit_st.mEntropyDataSize = sizeof(entropy_data);
  RNGinit_st.mPersDataSize = 0;
  RNGinit_st.mNonceSize = 0;

  status = RNGinit(&RNGinit_st, &RNGstate);
  if (status == RNG_SUCCESS)
  {
    RSAinOut_stt inOut_st;
    membuf_stt mb;

    mb.mSize = sizeof(preallocated_buffer);
    mb.mUsed = 0;
    mb.pmBuf = preallocated_buffer;

    /* Fill the RSAinOut_stt */
    inOut_st.pmInput = P_pInputMessage;
    inOut_st.mInputSize = P_InputSize;
    inOut_st.pmOutput = P_pOutput;

    /* Encrypt the message, this function will write sizeof(modulus) data */
    status = RSA_PKCS1v15_Encrypt(P_pPubKey, &inOut_st, &RNGstate, &mb);
  }
  return(status);
}
//******************************************************************************

//******************************************************************************
//  RSA Decryption with PKCS#1v1.5
//  error status: can be RSA_SUCCESS if success or RSA_ERR_GENERIC in case of fail
//******************************************************************************
int32_t RSA_Decrypt(RSAprivKey_stt * P_pPrivKey,
                    const uint8_t * P_pInputMessage,
                    uint8_t *P_pOutput,
                    int32_t *P_OutputSize)
{
  int32_t status = RSA_SUCCESS ;
  RSAinOut_stt inOut_st;
  membuf_stt mb;

  mb.mSize = sizeof(preallocated_buffer);
  mb.mUsed = 0;
  mb.pmBuf = preallocated_buffer;

  /* Fill the RSAinOut_stt */
  inOut_st.pmInput = P_pInputMessage;
  inOut_st.mInputSize = P_pPrivKey->mModulusSize;
  inOut_st.pmOutput = P_pOutput;

  /* Encrypt the message, this function will write sizeof(modulus) data */
  status = RSA_PKCS1v15_Decrypt(P_pPrivKey, &inOut_st, P_OutputSize, &mb);
  return(status);
}
//******************************************************************************