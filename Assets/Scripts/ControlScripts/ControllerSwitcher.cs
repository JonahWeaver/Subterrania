using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerSwitcher : MonoBehaviour
{
    public Controller[] controllers;

    //public WalkingController wc;
    //public ConversationController cc;
    //public BattleController bc;


    // Update is called once per frame
    void Start()
    {
        controllers = GetComponents<Controller>();
        //cc.enabled = false;
    }
    void Update()
    {
    }

    public void Switch(Controller controller)
    {
        foreach (Controller con in controllers)
        {
            if (con == controller)
            {
                con.Activate();
            }
        }

    }
}

