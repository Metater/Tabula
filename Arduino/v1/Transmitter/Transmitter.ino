#include <SPI.h>
#include <nRF24L01.h>
#include <RF24.h>

RF24 radio(7, 8); // CE, CSN
const byte address[6] = "00001";

void setup()
{
  Serial.begin(115200);

  radio.begin();

  radio.openWritingPipe(address);

  radio.setPALevel(RF24_PA_MAX);
  radio.setDataRate(RF24_250KBPS);

  radio.stopListening();
}

// Add acking later

byte displayData[32];
bool outOfSync = false;
byte outOfSyncBuffer[71]; // For lower memory use could combine with displayData

union ArrayToUInt32 {
  byte array[4];
  uint32_t integer;
};


void loop()
{
  if (Serial.available() >= 36 && !outOfSync)
  {
    ArrayToInteger receivedHash;
    Serial.readBytes(receivedHash.array, 4);
    Serial.readBytes(displayData, 32);
    if (displayDataValid(receivedHash.integer))
    {
      transmitDisplayData();
    }
    else
    {
      outOfSync = true;
    }
  }
  else if (Serial.available() >= 71 && outOfSync)
  {
    Serial.readBytes(outOfSyncBuffer, 71);
    int constructionPos = 0;
    while (outOfSync != constructionPos <= 35)
    {
      ArrayToInteger receivedHash;
      copyArray(outOfSyncBuffer, constructionPos, receivedHash.array, 4);
      copyArray(outOfSyncBuffer, constructionPos + 4, displayData, 32);
      if (displayDataValid(receivedHash.integer))
      {
        outOfSync = false;
        break;
      }
      constructionPos++;
    }
  }
}

void copyArray(byte* src, int srcOffset, byte* dst, int len) {
    for (int i = 0; i < len; i++) {
        dst[i] = src[i + srcOffset];
    }
}

bool displayDataValid(uint32_t receivedHash)
{
  uint32_t hash = jenkins_one_at_a_time_hash(displayData, 32);
  return hash == receivedHash;
}

void transmitDisplayData()
{
  if (!radio.writeFast(&displayData, 32))
  {
    radio.reUseTX();
  }
}

uint32_t jenkins_one_at_a_time_hash(byte *key, size_t len)
{
    uint32_t hash, i;
    for(hash = i = 0; i < len; ++i)
    {
        hash += key[i];
        hash += (hash << 10);
        hash ^= (hash >> 6);
    }
    hash += (hash << 3);
    hash ^= (hash >> 11);
    hash += (hash << 15);
    return hash;
}
