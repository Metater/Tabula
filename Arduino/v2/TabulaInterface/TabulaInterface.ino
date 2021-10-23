#include <SPI.h>
#include <nRF24L01.h>
#include <RF24.h>
#include <avr/pgmspace.h>
#include <LEDMatrixDriver.hpp>

RF24 radio(7, 8); // CE, CSN
const byte address[6] = "00001";

const unsigned long newFramesAckDelay = 200; // ms

LEDMatrixDriver lmd(8, 10); // Num 8x8 Segments, Chip Select

byte packet[32];

unsigned long framePeriod; // Range: 1ms - 256ms
byte frames[1024]; // 16 frames worth

bool newFrames = false;
int nextFrame = 0;

unsigned long frameEndTime = 0;

unsigned long newFramesAckTime = 0;

void setup()
{
  Serial.begin(115200);

  radio.begin();

  radio.openReadingPipe(0, address);

  radio.setPALevel(RF24_PA_MAX);
  radio.setDataRate(RF24_250KBPS);

  radio.startListening();

  lmd.setEnabled(true);
  lmd.setIntensity(0);
}

int getOffset(byte section)
{
  return 30 + ((((int)section) - 1) * 31);
}

void loadFrame()
{
  int offset = nextFrame * 64;
  for (int x = 0; x < 64; x++)
    for (int y = 0; y < 8; y++)
      lmd.setPixel(x, y, bitRead(frames[x + offset], y));
  lmd.display();
  nextFrame++;
  frameEndTime = millis() + framePeriod;
}

void loop()
{
  if (radio.available())
  {
    radio.read(&packet, 32);
    if (packet[31] == 0)
    {
      framePeriod = ((unsigned long)packet[30]) + 1;
      for (int i = 0; i < 30; i++)
        frames[i] = packet[i];
      newFrames = true;
      newFramesAckTime = millis() + newFramesAckDelay;
    }
    else if (packet[31] <= 32)
    {
      int offset = getOffset(packet[31]);
      for (int i = 0; i < 31; i++)
        frames[i + offset] = packet[i];
    }
    else if (packet[31] == 33)
    {
      int offset = getOffset(packet[31]);
      for (int i = 0; i < 2; i++)
        frames[i + offset] = packet[i];
    }
  }

  if (newFrames && newFramesAckTime >= millis())
  {
    newFrames = false;
    nextFrame = 0;
    loadFrame();
  }
  else if (millis() >= frameEndTime)
  {
    if (nextFrame < 16) loadFrame();
  }
}
