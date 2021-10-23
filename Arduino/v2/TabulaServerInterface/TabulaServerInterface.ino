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
  Serial.begin(115200);

  radio.begin();

  radio.openWritingPipe(address);

  radio.setPALevel(RF24_PA_MAX);
  radio.setDataRate(RF24_250KBPS);

  radio.stopListening();

  Serial.println("Started!");
}

void send()
{
  /*
  int tries = 0;
  while (tries < 3 && !radio.writeFast(&packet, 32))
  {
    radio.reUseTX();
    tries++;
  }
  */
  radio.write(&packet, 32);
}

void loop()
{
  if (millis() - lastReceiveTime > 100) section = 0;
  
  if (readData >= 1025)
  { 
    readData = 0;
    Serial.println("Reading frames!");
    
    byte framePeriod = Serial.read();
    Serial.readBytes(packet, 30);
    packet[30] = framePeriod;
    packet[31] = 0;
    send();

    byte section = 1;
    while (section < 33)
    {
      Serial.readBytes(packet, 31);
      packet[31] = section;
      send();
      section++;
    }

    Serial.readBytes(packet, 2);
    packet[31] = section;
    send();
    while (Serial.available()) Serial.read();
  }
}
