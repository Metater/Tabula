#include <SPI.h>
#include <nRF24L01.h>
#include <RF24.h>

RF24 radio(7, 8); // CE, CSN
const byte address[6] = "00001";

byte packet[32];

byte section = 0;

unsigned long lastReceiveTime = 0;

void setup()
{
  Serial.begin(57600);

  radio.begin();

  radio.openWritingPipe(address);

  radio.setPALevel(RF24_PA_MAX);
  radio.setDataRate(RF24_250KBPS);

  radio.stopListening();
}

void send()
{
  radio.write(&packet, 32);
}

void loop()
{
  if (millis() - lastReceiveTime > 128UL) section = 0;

  if (Serial.available() < 32) return;
  
  lastReceiveTime = millis();

  if (section == 0)
  {
    Serial.readBytes(packet, 1);
  }
  else if (section <= 32)
  {
    Serial.readBytes(packet, 32);
  }
  else return;
  send();
  section++;
}
