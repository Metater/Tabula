void tryReadRadioData()
{
  while (radio.available())
  {
    //unsigned long startTime = micros();
    
    byte radioData[32];
    radio.read(&radioData, 32);
    
    int displayRegion = radioData[0];
    
    parseRadioData(displayRegion, radioData);
    //printRadioData(radioData);

    //Serial.println(micros() - startTime);
  }
}

void printRadioData(byte radioData[])
{
  String data = "";
  data += "Radio Data Received: ";
  //data += "Time: " + String((float)millis()/(float)1000, DEC) + " > ";
  String hex;
  for (int i = 0; i < 32; i++)
  {
    hex = formatHex(String(radioData[i], HEX));
    data += hex + ":";
  }
  //Serial.println(data);
}

String formatHex(String str)
{
  if (str.length() == 1)
    str = "0" + str;
  return str;
}
