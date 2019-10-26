#include "cycle_queue.h"
#include "stm32f1xx_hal.h"

Queue queue;
uint8_t buffer[RING_BUFFER_SIZE];

void InitQueue()
{
  queue.header = buffer;
  queue.tail   = &buffer[RING_BUFFER_SIZE];
  queue.input  = queue.header;
  queue.output = queue.header;
  queue.isBusy = false;
  queue.cycle  = false;
}

void SetQueueOwner()
{
  while (queue.isBusy);
  queue.isBusy = true;
}

void ResetQueueOwner()
{
  queue.isBusy = false;
}

void Enqueue(uint8_t byte)
{
  SetQueueOwner();
  
  // достигли конца буфера
  if (queue.input >= queue.tail)
  {
    queue.input = queue.header;
    queue.cycle = true;
  }
  
  // достигли выходного указателя, затирание данных
  if (queue.input == queue.output && queue.cycle)
  {
    if ((queue.output + 1) >= queue.tail)
      queue.output = queue.header;
    else
      queue.output++;
  }
  
  *(queue.input) = byte;
  queue.input++;
  ResetQueueOwner();
}

uint8_t Dequeue()
{
  uint8_t byte = 0x00;
  __disable_irq(); 
  // занимаем очередь для извлечения
  SetQueueOwner();
  
  // нет данных в очереди
  if (IsQueueEmpty())
    goto RESET;
  
  // достигли конца, перевод на начало
  if (queue.output >= queue.tail)
    queue.output = queue.header;
  
  // последний байт в очереди
  if ((queue.output+1) == queue.input)
    queue.cycle = false;
  
  
  byte = *(queue.output);
  queue.output++;
  
RESET:
  ResetQueueOwner();
  __enable_irq();
  
  return byte;
}

uint8_t Peek()
{
  // нет данных
  if (IsQueueEmpty())
    return 0x00;
  
  return *(queue.output);
}

bool IsQueueEmpty()
{
  if(queue.input == queue.output && !queue.cycle)
    return true;
  
  return false;
}
