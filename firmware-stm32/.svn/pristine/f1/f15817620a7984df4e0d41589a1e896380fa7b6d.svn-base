#ifndef CYCLE_QUEUE_H
#define	CYCLE_QUEUE_H
#include "stdint.h"
#include "stdbool.h"

#define RING_BUFFER_SIZE	50


typedef struct Queue
{
	uint8_t *header;
	uint8_t *tail;
	uint8_t *input;
	uint8_t *output;
	bool isBusy;
	bool cycle;
}Queue;

void InitQueue();
void SetQueueOwner();
void ResetQueueOwner();
void Enqueue(uint8_t byte);
uint8_t Dequeue();
uint8_t Peek();
bool IsQueueEmpty();


#endif	/* CYCLE_QUEUE_H */