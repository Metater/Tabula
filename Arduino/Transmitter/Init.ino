void initSerial()
{
  Serial.begin(115200);
}

void initRadio()
{
  radio.begin();
  
  radio.openWritingPipe(address);
  
  radio.setPALevel(RF24_PA_MAX);
  radio.setDataRate(RF24_2MBPS);
  
  radio.stopListening();
}
