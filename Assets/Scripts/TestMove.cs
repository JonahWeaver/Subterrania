using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMove : MonoBehaviour
{
    public int speed = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.W))
        {
            this.transform.position += new Vector3(0f, speed * Time.deltaTime, 0f);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            this.transform.position -= new Vector3(0f, speed * Time.deltaTime, 0f);
        }
        if (Input.GetKey(KeyCode.D))
        {
            this.transform.position += new Vector3(speed * Time.deltaTime, 0f, 0f);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            this.transform.position -= new Vector3(speed * Time.deltaTime,0f, 0f);
        }
    }
}
