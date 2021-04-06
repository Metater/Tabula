#include <SPI.h>
#include <nRF24L01.h>
#include <RF24.h>

RF24 radio(7, 8); // CE, CSN
const byte address[6] = "00001";

const int Columns = 64;
byte displayData[Columns];

const int LoopDelay = 1000;

void setup()
{
  initSerial();
  initRadio();
}

void loop()
{
  tryReadSerialData();
}
