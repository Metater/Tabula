byte region0Packet[32];
byte region1Packet[32];
byte region2Packet[32];

void transmitServerData()
{
  // Encode region ids
  region0Packet[0] = 0;
  region1Packet[0] = 1;
  region2Packet[0] = 2;

  // Encode display data
  for (int i = 1; i < 32; i++)
  {
    region0Packet[i] = displayData[i-1];
    region1Packet[i] = displayData[i+30];
  }
  for (int i = 1; i < 3; i++)
  {
    region2Packet[i] = displayData[i+61];
    //Serial.println(displayData[i+61]);
  }

  // Transmit display data
  /*
  radio.writeFast(&region0Packet, 32);
  radio.txStandBy();
  radio.writeFast(&region1Packet, 32);
  radio.txStandBy();
  radio.writeFast(&region2Packet, 32);
  radio.txStandBy();
  */
  //unsigned long startTime = micros();
  //printDisplayRegionData(region0Packet);
  if (!radio.writeFast(&region0Packet, 32))
  {
    radio.reUseTX();
  }
  //printDisplayRegionData(region1Packet);
  if (!radio.writeFast(&region1Packet, 32))
  {
    radio.reUseTX();
  }
  //printDisplayRegionData(region2Packet);
  if (!radio.writeFast(&region2Packet, 32))
  {
    radio.reUseTX();
  }
  //Serial.print("Time: ");
  //Serial.println(micros() - startTime);
}
