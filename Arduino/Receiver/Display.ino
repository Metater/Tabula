void parseRadioData(int region, byte radioData[])
{
  if (region == 0 || region == 1)
  {
    for (int i = 1; i < 32; i++)
    {
      displayData[(i+(region*31))-1] = radioData[i];
    }
  }
  if (region == 2)
  {
    displayData[62] = radioData[1];
    displayData[63] = radioData[2];
  }
  updateDisplayRegion(region);
  lmd.display();
}

void updateDisplayRegion(int region)
{
  switch(region)
  {
    case 0:
      //firstRegionReceived = micros();
      setDisplayColumns(0, 31);
      break;
    case 1:
      setDisplayColumns(31, 31);
      break;
    case 2:
      //Serial.println(micros() - firstRegionReceived);
      setDisplayColumns(62, 2);
      break;
  }
}

void setDisplayColumns(int start, int count)
{
  for(int x = start; x < start+count; x++)
  {
    for (int y = 0; y < 8; y++)
    {
      lmd.setPixel(x, y, bitRead(displayData[x], y));
    }
  }
}
