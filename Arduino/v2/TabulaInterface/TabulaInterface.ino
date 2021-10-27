#include <SPI.h>
#include <nRF24L01.h>
#include <RF24.h>
#include <avr/pgmspace.h>
#include <LEDMatrixDriver.hpp>

RF24 radio(7, 8); // CE, CSN
const byte address[6] = "00001";

LEDMatrixDriver lmd(8, 10); // Num 8x8 Segments, Chip Select

byte packet[32];

unsigned long framePeriod; // Range: 1ms - 256ms
byte frames[1024]; // 16 frames worth

int nextFrame = 0;

unsigned long frameEndTime = 0;

byte section = 0;
unsigned long lastReceiveTime = 0;

void setup()
{
  Serial.begin(57600);

  radio.begin();

  radio.openReadingPipe(0, address);

  radio.setPALevel(RF24_PA_MAX);
  radio.setDataRate(RF24_250KBPS);

  radio.startListening();

  lmd.setEnabled(true);
  lmd.setIntensity(0);
}

void loadFrame()
{
  int offset = nextFrame * 64;
  for (int x = 0; x < 64; x++)
    lmd.setColumn(x, frames[x + offset]);
  lmd.display();
  nextFrame++;
    frameEndTime = millis() + framePeriod;
}

void loop()
{
  if (millis() - lastReceiveTime > 128UL) section = 0;
  
  if (radio.available())
  {
    lastReceiveTime = millis();
    radio.read(&packet, 32);
    if (section == 0)
    {
      framePeriod = ((unsigned long)packet[0]) + 1UL;
    }
    else if (section <= 32)
    {
      for (int i = 0; i < 32; i++)
        frames[i + ((section - 1) * 32)] = packet[i];
    }
    else return;
    if (section == 32)
    {
      nextFrame = 0;
      frameEndTime = 0;
    }
    section++;
  }
  
  if (millis() >= frameEndTime && nextFrame < 16)
    loadFrame();
}
