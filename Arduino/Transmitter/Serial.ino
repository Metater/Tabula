int displayDataIndex = 0;
void tryReadSerialData()
{
  if (Serial.available() > 0)
  {
    while (Serial.available() > 0)
    {
      displayData[displayDataIndex] = Serial.read();
      displayDataIndex++;
      if (displayDataIndex == 64){
        displayDataIndex = 0;
        transmitServerData();
        //printSerialData();
      }
    }
    /*
    for (int i = 0; i < Columns; i++)
    {
      int incomingByte = Serial.read();
      displayData[i] = incomingByte;
    }
    */
    Serial.println(displayDataIndex);
    //Serial.readBytes(&displayData, 64);
    //Serial.println("DISAUASDBHSD");
    //Serial.println(displayData[63]);
  }
  else
  {
    return false;
  }
}

void printSerialData()
{
  String data = "";
  data += "Serial Data Received: ";
  String hex;
  for (int i = 0; i < Columns; i++)
  {
    hex = formatHex(String(displayData[i], HEX));
    data += hex + ":";
  }
  Serial.println(data);
}

void printDisplayRegionData(byte regionPacket[])
{
  String data = "";
  data += "Sending Display Region Data: ";
  String hex;
  for (int i = 0; i < 32; i++)
  {
    hex = formatHex(String(regionPacket[i], HEX));
    data += hex + ":";
  }
  Serial.println(data);
}

String formatHex(String str)
{
  if (str.length() == 1)
    str = "0" + str;
  return str;
}
