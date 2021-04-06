#include <SPI.h>
#include <nRF24L01.h>
#include <RF24.h>
#include <avr/pgmspace.h>
#include <LEDMatrixDriver.hpp>

RF24 radio(7, 8); // CE, CSN
const byte address[6] = "00001";

const int Columns = 64;
LEDMatrixDriver lmd(8, 10); // 8x8 Segments, Chip Select

byte displayData[Columns];

unsigned long firstRegionReceived;

void setup()
{
  initSerial();
  initRadio();
  initDisplay();
  
  updateDisplayRegion(0);
  updateDisplayRegion(1);
  updateDisplayRegion(2);
}

void loop()
{
  tryReadRadioData();
}
