using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bluetooth_Controller_update : MonoBehaviour
{
    public Text ChangingText;
    public GameObject changingTextTwo;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void TextChange()
    {
         ChangingText.text = "2";
        changingTextTwo.GetComponent<Text>().text = "2";

    }
}
