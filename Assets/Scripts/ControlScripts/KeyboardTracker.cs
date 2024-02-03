using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardTracker : DeviceTracker
{

    public AxisKeys[] axisKeys;
    public KeyCode[] buttonKeys;
    public bool[] clicks;
    public Vector2 clickLocation;

    void Reset()
    {
        im = GetComponent<InputManager>();
        //data = new InputData();
        axisKeys = new AxisKeys[im.axisCount];
        buttonKeys = new KeyCode[im.buttonCount];
        clicks = new bool[3];
    }
    public override void Refresh()
    {
        im = GetComponent<InputManager>();

        //create 2 temp arrays for buttons and axes
        KeyCode[] newButtons = new KeyCode[im.buttonCount];
        AxisKeys[] newAxes = new AxisKeys[im.axisCount];

        if (buttonKeys != null)
        {
            for (int i = 0; i < Mathf.Min(newButtons.Length, buttonKeys.Length); i++)
            {
                newButtons[i] = buttonKeys[i];
            }
        }
        buttonKeys = newButtons;

        if (axisKeys != null)
        {
            for (int i = 0; i < Mathf.Min(newAxes.Length, axisKeys.Length); i++)
            {
                newAxes[i] = axisKeys[i];
            }
        }
        axisKeys = newAxes;
    }
    void Update()
    {
        //check for inputs, if inputs detected, set newData to true
        //populate InputData to pass to the InputManager
        for (int i = 0; i < axisKeys.Length; i++)
        {
            float val = 0f;
            if (Input.GetKey(axisKeys[i].positive))
            {
                val += 1f;
                newData = true;
            }
            if (Input.GetKey(axisKeys[i].negative))
            {
                val -= 1f;
                newData = true;
            }
            //Debug.Log(i.ToString() + " " + data.axes.Length.ToString());
            data.axes[i] = val;
        }
        for (int i = 0; i < buttonKeys.Length; i++)
        {
            if (Input.GetKeyDown(buttonKeys[i]))
            {
                data.buttons[i] = true;
                newData = true;
            }
        }
        for(int i=0; i<3;i++)
        {
            if(Input.GetMouseButtonDown(i))
            {
                data.clicks[i] = true;
                data.clickLocation = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                newData = true;
            }
        }
        if (newData)
        {
            im.PassInput(data);
            newData = false;
            data.Reset();
        }
    }
}

[System.Serializable]
public struct AxisKeys
{
    public KeyCode positive;
    public KeyCode negative;
}
