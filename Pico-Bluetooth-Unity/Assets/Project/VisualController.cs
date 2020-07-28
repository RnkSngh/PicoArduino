using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pvr_UnitySDKAPI;
using System;
using UnityEngine.UI;
using TechTweaking.Bluetooth;

public class VisualController : MonoBehaviour
{
    public enum UserHandNess
    {
        Right,
        Left
    }
    private static UserHandNess handness;

    private bool controller0is3dof = false;
    private bool controller1is3dof = false;


    public Text Left_keyName;
    public Text Left_keyState;
    public Text Left_controllerPos;
    public Text Left_controllerQuaternion;

    public Text Left_Dir;

    public Text Bluetooth; //holds the text received from the hc-05 bluetooth module

    public Text Right_keyName;
    public Text Right_keyState;
    public Text Right_controllerPos;
    public Text Right_controllerQuaternion;
    public Text Right_Dir;

    public GameObject ctr0;
    public GameObject ctr1;

    public BluetoothDevice device; 

    void Awake()
    {
        Pvr_ControllerManager.PvrServiceStartSuccessEvent += ServiceStartSuccess;
        Pvr_ControllerManager.SetControllerAbilityEvent += CheckControllerState;
        Pvr_ControllerManager.ChangeMainControllerCallBackEvent += MainControllerChanged;
        Pvr_ControllerManager.ChangeHandNessCallBackEvent += HandnessChanged;

    }
    void Start()
    {
        handness = (UserHandNess)Pvr_ControllerManager.controllerlink.getHandness();
        if ((int)handness == -1)
        {
            handness = UserHandNess.Right;
        }

        //connect to hc-05 bluetooth adapter
        device = BluetoothAdapter.getPairedDevices()[0]; // assuming no other devices are connected. index might need to be changed if other devices are connected
        device.setEndByte(10); //set newline as the token delineator
        device.normal_connect(true, false); //connect 
        
    }


    /// <summary> AdjustHapFeedback converts a single to an ASCII byte array and sends the byte array to the Arduino when the slider value is changed in the UI. 
    /// This data is used to trigger the Arduino haptic feedback module. 
    /// </summary>
    /// <param name="intensity">Indicates the haptic feedback intensity, which is set by the UI slider.</param>
    public void AdjustHapFeedback(System.Single intensity)
    {
        //convert byte to ascii and send
        byte intensity_ascii = (byte)intensity.ToString()[0];
        device.send(new byte[]{ intensity_ascii}); //send bytes to bluetooth device
        
    }

    /// <summary> ReadArduinoData checks if there is any data being sent from the Arduino. The data is received in the form of an ASCII byte array, and corresponds to the state 
    /// of the capacitive button. The function returns a string corresponding to the state of the capacitive button, which is used to change a text field in the UI. 
    /// </summary>
    public string ReadArduinoData()
    {
        // Read if data is available from Arduino
        byte[] msg = Controller.UPvr_GetCapacative_button(device);
        if ((msg != null) && (msg.Length > 0))
        {
            //Convert ASCII Byte array to literal string
            string content = System.Text.ASCIIEncoding.ASCII.GetString(msg);

            //Decode literal string
            if (content.Contains("3")) 
            {
                return "not pressed";
            }
            else if (content.Contains("2"))
            {
                return "pressed";
            }

        }

         return Bluetooth.text; //UI text is unchanged if no new information is received

    }

    /// <summary> CheckControllerState reads the button states from the Pico G2 buttons and updates the UI 
    /// </summary>
    void CheckControllerState()
    {
        
        if (Controller.UPvr_GetKey(1, Pvr_KeyCode.A))
        {
            Right_keyName.text = "Right A";
            Right_keyState.text = "Down";
        }
        else if (Controller.UPvr_GetKey(1, Pvr_KeyCode.B))
        {
            Right_keyName.text = "Right B";
            Right_keyState.text = "Down";
        }
        else if (Controller.UPvr_GetKey(1, Pvr_KeyCode.APP))
        {
            Right_keyName.text = "Right App";
            Right_keyState.text = "Down";
        }
        else if (Controller.UPvr_GetKey(1, Pvr_KeyCode.TOUCHPAD))
        {
            Right_keyName.text = "Right TouchPad";
            Right_keyState.text = "Down";
        }
        else if (Controller.UPvr_GetKey(1, Pvr_KeyCode.TRIGGER))
        {
            Right_keyName.text = "Right Trigger";
            Right_keyState.text = "Down";
        }
        else
        {
            Debug.Log(Right_keyName == null);
        }

        if (Controller.UPvr_GetKey(0, Pvr_KeyCode.X))
        {
            Left_keyName.text = "Left X";
            Left_keyState.text = "Down";
        }
        else if (Controller.UPvr_GetKey(0, Pvr_KeyCode.Y))
        {
            Left_keyName.text = "Left Y";
            Left_keyState.text = "Down";
        }
        else if (Controller.UPvr_GetKey(0, Pvr_KeyCode.APP))
        {
            Left_keyName.text = "Left App";
            Left_keyState.text = "Down";
        }
        else if (Controller.UPvr_GetKey(0, Pvr_KeyCode.TOUCHPAD))
        {
            Left_keyName.text = "Left TouchPad";
            Left_keyState.text = "Down";
        }
        else if (Controller.UPvr_GetKey(0, Pvr_KeyCode.TRIGGER))
        {
            Left_keyName.text = "Left Trigger";
            Left_keyState.text = "Down";
        }
        else
        {
            Left_keyName.text = "Left None";
            Left_keyState.text = "UP";
        }

        if (Controller.UPvr_GetKeyDown(0, Pvr_KeyCode.TOUCHPAD))
        {
            TouchPadClick pad = Controller.UPvr_GetTouchPadClick(0);
            Left_Dir.text = pad.ToString();
        }else
        {
            Left_Dir.text = "None";
        }

        Left_controllerPos.text = Controller.UPvr_GetControllerPOS(0).ToString();
        Left_controllerQuaternion.text = Controller.UPvr_GetControllerQUA(0).ToString();

        //change UI to reflect current state of capacitive button
        Bluetooth.text = ReadArduinoData();

    }

    private void Update()
    {
        CheckControllerState();
    }

    void OnDestroy()
    {
        Pvr_ControllerManager.PvrServiceStartSuccessEvent -= ServiceStartSuccess;
        Pvr_ControllerManager.SetControllerAbilityEvent -= CheckControllerState;
        Pvr_ControllerManager.ChangeMainControllerCallBackEvent -= MainControllerChanged;
        Pvr_ControllerManager.ChangeHandNessCallBackEvent -= HandnessChanged;
    }


    private void MainControllerChanged(string index)
    {
        RefreshHandness();
    }

    private void HandnessChanged(string index)
    {
        RefreshHandness();
    }


    private void ServiceStartSuccess()
    {
        RefreshHandness();
        if (Pvr_ControllerManager.controllerlink.neoserviceStarted)
        {
            if (Controller.UPvr_GetControllerState(0) == ControllerState.Connected)
            {
                controller0is3dof = ( Controller.UPvr_GetControllerAbility(0) == 1 );
            }
            if (Controller.UPvr_GetControllerState(1) == ControllerState.Connected)
            {
                controller1is3dof = ( Controller.UPvr_GetControllerAbility(1) == 1 );
            }
        }

    }
    private void RefreshHandness()
    {
        handness = (UserHandNess)Pvr_ControllerManager.controllerlink.getHandness();
        if (Controller.UPvr_GetMainHandNess() == 1)
        {
            ChangeHandNess();
        }
    }
    public static void ChangeHandNess()
    {
        handness =   handness ==   UserHandNess.Right  ? UserHandNess.Left : UserHandNess.Right  ;
    }
    private void CheckControllerState(string data)
    {
        var state = Convert.ToBoolean(Convert.ToInt16(data.Substring(4, 1)));
        var id = Convert.ToInt16(data.Substring(0, 1));
        var ability = Convert.ToInt16(data.Substring(2, 1));
        if (state)
        {
            if (id == 0)
            {
                controller0is3dof = ( ability == 1 );
            }
            if (id == 1)
            {
                controller1is3dof = ( ability == 1 );
                RefreshHandness();
            }
        }
    }

}
