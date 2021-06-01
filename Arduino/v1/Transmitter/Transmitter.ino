#include <SPI.h>
#include <nRF24L01.h>
#include <RF24.h>

void setup()
{

}

int displayDataIndex = 0;
byte displayData[32];

void loop()
{
  if (Serial.available() >= 32)
  {
    Serial.readBytes(displayData, displayDataIndex);
  }
}
