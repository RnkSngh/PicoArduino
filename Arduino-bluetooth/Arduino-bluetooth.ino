#include <Wire.h>
#include "Adafruit_DRV2605.h"

Adafruit_DRV2605 drv;


// constants won't change. They're used here to set pin numbers:
const int capacitivePin = 4;     // the number of the button pin
int buttonState = 0;
  
  
int waveindex=64;
char read_char; //char that is read from bluetooth connection
uint8_t read_int;
void setup () {

  Serial.begin(9600);
  pinMode(capacitivePin, INPUT);

  drv.begin();
  drv.selectLibrary(1);
  drv.setMode(DRV2605_MODE_INTTRIG); 

}

uint8_t effect = 64;


void send_haptic_feedback( int Waveform){
    // set the effect to play
    Serial.print(Waveform);
  drv.setWaveform(0, Waveform);  // play effect 
  drv.setWaveform(1, 0);       // end waveform

  // play the effect!
  drv.go();
    delay(100);
}

void loop() {

  buttonState = digitalRead(capacitivePin);
  if (buttonState){ // if cap button is pressed
      Serial.print(2);
      Serial.print("\n"); //send \n because it was set as the token delineator
      
  }
  else{
    Serial.print(3); //if cap button is not pressed
    Serial.print("\n");
  }
  

  
  if(Serial.available()){ // means hap feedback will be triggered
    read_char = (char) Serial.read();

    //map read char to proper haptic feedback range
    send_haptic_feedback(118- read_char);

  }
  delay(200);
}
