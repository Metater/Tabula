void initSerial()
{
  Serial.begin(115200);
}

void initRadio()
{
  radio.begin();
  
  radio.openReadingPipe(0, address);
  
  radio.setPALevel(RF24_PA_MAX);
  radio.setDataRate(RF24_2MBPS);
  
  radio.startListening();
}

void initDisplay()
{
  lmd.setEnabled(true);
  lmd.setIntensity(0);
}
