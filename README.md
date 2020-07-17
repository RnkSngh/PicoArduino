# Pico-Interactive-Arduino-Controller
This documentation provides a system overview, quickstart, and expansion guide for establishing 2-way Bluetooth communication between Arduino and the Pico-Interactive G2 headset. The included code is a Unity project that works with the example setup, and can be modified modularly to include other Arduino components. 

**Table of Contents**
* [About]
* [Quickstart guide for example setup]
	* [Sample System Components]
	* [System Diagram]
	* [Quickstart Steps]
* [Adding a Modular Component]
  * [Adding Arduino to Pico communication]
  * [Adding Pico to Arduino communication]


# About
This guide has been built with the following software versions:
* Arduino - v1.8.12
* Android - v10
* Pico Unity SDK - v2.8.3
* Bluetooth Classic Library for Unity (TechTweaking) - v3.9
* Unity - v2018.4.23f1

# Quickstart guide for example setup
This section provides a quickstart guide for establishing 2-way communication between an Arduino and a Pico Headset. The capacitive sensor sends data to the Pico Headset from the Arduino to display whether or not it is pressed, and the Pico Headset sends data to the Arduino to trigger the vibration motor.

## Example System Components
The following hardware components are used in this sample setup:
* Pico G2
	* Pico G2 Headset
	* Pico G2 Controller
		* 3Dof sensing
		* Controller Buttons
* Arduino Uno System
	* [capacitive sensor](https://www.amazon.com/HiLetgo-TTP223B-Capacitive-Digital-Raspberry/dp/B00HFQEFWQ/ref=sr_1_3?dchild=1&keywords=hiletgo+touch+switch&qid=1594935626&sr=8-3)
	* [haptic feedback driver](https://www.adafruit.com/product/2305)
	* [vibration motor](https://www.sparkfun.com/products/8449)
	* [Hc-05 Bluetooth Module](https://www.amazon.com/HiLetgo-Wireless-Bluetooth-Transceiver-Arduino/dp/B071YJG8DR)
* External Computer to upload code to Arduino
## Example Macro System Diagram
![](./Pico-Bluetooth-Unity/System-Diagram.svg)
## Example Arduino System Diagram
![](./Pico-Bluetooth-Unity/Arduino-System-Design.svg)
## Quickstart Steps
1. Set up the Arduino System as shown in the figure above, and upload the Arduino code from the Arduino-Bluetooth folder onto the board. [Make sure that the cables connected to the TX and RX ports on the Arduino are not connected when the code is being uploaded](https://www.quora.com/How-can-I-overcome-upload-error-in-Arduino-Uno), and re-connect them once the code is finished uploading to the Arduino. After this step is completed, the bluetooth module LED should be blinking to indicate there is power going to the bluetooth module. 
1. Pair the Pico Headset with the powered HC-05 Bluetooth module. To do this, navigate to **Settings> Advanced Settings > Bluetooth > Pair Bluetooth** from the Pico Headset home, and pair with the HC-05 module. The device only needs to be paired at this step; the app will establish connection between devices when run. Make sure that the Pico G2 controller and the HC-05 module are the only devices paired to the Pico G2 Headset for this Quickstart tutorial. The code can be modified to specify a specific device if multiple bluetooth devices are connected (see **Modifying Code** section). 
1. Launch the app by loading and running the Arduino-Bluetooth-Controller.apk onto the Pico Headset. The UI canvas will indicate when the capacitive button is pressed, and changing the slider will trigger the motor connected to the arduino. 

# Modifying Code 
The code can be modified to change what data is sent through the bluetooth from both the Arduino and the Pico side. 
## Modifying Pico To Arduino Communication
To modify code from the Pico Headset, import the code in the Pico-Bluetooth-Unity folder to a Unity project. The folder structure is similar to the structure from that created by the [Pico VR Quickstart of SDK access guide](https://sdk.picovr.com/docs/sdk/en/chapter_four.html), with some added files for the UI canvas and the bluetooth module. Most of the bluetooth communication code is added in the VisualController.cs file in the **Pico-Bluetooth-Unity > Assets > Projects** folder.
### Connecting More than One Bluetooth Device
The connection to the HC-05 module is established in the start function in the VisualController.cs file (shown below). If multiple devices are connected and a specific device needs to be specified, the index of the device can be found by iterating through the devices array and printing the name of each device by accessing the ```device.Name ``` variable for each device. Once the index of the specific device is found, it can be used to acess the device from the devices list. 
 
```
//Connect to bluetooth Module
BluetoothDevice[] devices = BluetoothAdapter.getPairedDevices(); //get list of paired devices
device = devices[0]; // assuming no other devices are connected. index might need to be changed if other devices are connected
device.setEndByte(10);
device.normal_connect(true, false);
```

### Changing Data Sent from Pico to Arduino
The ``` Adjust_HapFeedback ``` function in the VisualController.cs file is called whenever the value is changed for the slider on the UI canvas in the Headset - this was done by [linking the slider in unity with the ```OnValueChanged() ``` function](https://www.youtube.com/watch?v=HQ8Tttcksu4&t=4s). The function converts the interger character given by the slider to the ASCII byte representation, and sends the byte array to the bluetooth module (more bytes can be added to the array if multiple characters are sent each iteration). 
```
public void Adjust_HapFeedback(System.Single intensity)
{
//convert byte to ascii and send
byte intensity_ascii = (byte)intensity.ToString()[0];
device.send(new byte[]{ intensity_ascii}); //send byte array to bluetooth device
}
```

## Modifying Arduino to Pico Communication
### Changing Data Sent From Arduino to Pico
The data sent from arduino can be changed by changing the Arduino-bluetooth.ino file in the Arduino-bluetooth folder. The sent data can be changed by modifying the input to the ```Serial.print()``` function in the ```looop()``` function: 
```
  if (buttonState){ // if cap button is pressed
      Serial.print(2);
      Serial.print("\n"); //send \n because it was set as the token delineator
  }
  else{
    Serial.print(3); //if cap button is not pressed
    Serial.print("\n");
  }
```
### Changing How the Pico Headset Interprets Received Data
The Pico headset processes the data in the ```update()``` loop of the VisualController.cs file. The ```Serial.print()``` function from the Arduino in the above section sends a byte array to the Pico Headset. This byte array is converted to ASCII representation, and the ```Bluetooth.text variable``` is changed to match the state of the capacitive button, which is in turn updated in the UI canvas. 
```
// Read if data is available 
byte[] msg = Controller.UPvr_GetCapacative_button(device);
if (msg != null && msg.Length > 0)
{
    //Convert Byte array to string
    string content = System.Text.ASCIIEncoding.ASCII.GetString(msg);

    //change UI text
    if (content.Contains("3"))
    {
	Bluetooth.text = "not pressed";
    }
    else if(content.Contains("2"))
    {
	Bluetooth.text = "pressed";
    }
}
```
