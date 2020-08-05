#include <Wire.h>
#include "Adafruit_DRV2605.h"

Adafruit_DRV2605 g_Drv; //the connected driver


//constants won't change, they're used here to set pin numbers:
const int CAPACITIVE_PIN = 4;     // the input port pin on Arduino used to send the capacitive sensor signal
int g_ButtonState = 0; //button state of the capacitive sensor
  
char g_ReadChar; //char that is read from bluetooth connection


//set up is executed once at the start of the script, similar to Unity's Start() function
void setup () {

  Serial.begin(9600); //begin serial communication at 9600 baud rate
  pinMode(CAPACITIVE_PIN, INPUT); //set pin 4 to input mode

//begin drv driver and set library
  g_Drv.begin();
  g_Drv.selectLibrary(1);
  g_Drv.setMode(DRV2605_MODE_INTTRIG); 

}

// SendHapticFeedback triggers a vibration sequence in the motor using the Adafruit DRV driver.
// Waveform is an interger between 1-123 corresponding to a waveform ID for a vibration sequence (see Adafruit Haptic Feedback Driver documentation)
void SendHapticFeedback( int Waveform){
    // set the effect to play
    Serial.print(Waveform);
  g_Drv.setWaveform(0, Waveform);  // play effect 
  g_Drv.setWaveform(1, 0);       // end waveform

  // play the effect!
  g_Drv.go();
    delay(100);
}

// loop function continues endlessly, similar to Unity's Update() function
void loop() { 

  g_ButtonState = digitalRead(CAPACITIVE_PIN);
  if (g_ButtonState){ // if cap button is pressed
      Serial.print(2);
      Serial.print("\n"); //send \n because it was set as the token delineator for the Pico headset
      
  }
  else{
    Serial.print(3); //if cap button is not pressed
    Serial.print("\n");
  }
  

  //check if the bluetooth module is sending any data to Arduino, and convert the ASCII byte to a WaveForm ID to be passed to the SendHapticFeedback function
  if(Serial.available()){ 
    g_ReadChar = (char) Serial.read(); //read the sent ASCII byte representation. The Pico headset only sends intergers between 1-6, which correspond to ASCII bytes 49-54
    SendHapticFeedback(118 - g_ReadChar); //map bytes 49-54 to the waveform ID between 64-69, which correspond to hums of varying intensity
  }
  
  delay(200); //add 0.2 second delay for stability
}
