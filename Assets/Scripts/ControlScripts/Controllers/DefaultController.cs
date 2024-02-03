using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultController : Controller
{
    public UIManager uiManager;
    public float speed = 5f;
    public float scale = .75f;
    public float jumpForce = 5f;

    bool isGrounded = false;
    float mVert;
    float mHor;
    bool jump;
    bool inventoryOpen;
    bool settings;

    bool leftClick;
    bool rightClick;
    bool middleClick;

    Vector2 clickLocation;
    public LayerMask groundLayer;

    protected override void Start()
    {
        base.Start();
    }
    public override void ReadInput(InputData data)
    {
        RegisterInput(data);
        rb.velocity = new Vector3(mHor * speed, rb.velocity.y, 0f);

        // Flip character sprite based on direction of movement
        if (mHor > 0)
        {
            transform.localScale = new Vector3(scale, scale, scale);
        }
        else if (mHor < 0)
        {
            transform.localScale = new Vector3(-scale, scale, scale);
        }

        if(inventoryOpen)
        {
            uiManager.ProcessCommand(UIManager.Command.OpenClose, UIManager.ManagerType.Inventory);
        }
        if(leftClick)
        {
            //check if click hits UI, then handle accordingly
            if(uiManager.mouseClickOverUI())
            {
                //
            }
            else
            {
                tileWorld.BreakBlockInWorld(clickLocation);
            }
            
        }
        else if(rightClick)
        {
            //check if click hits UI, then handle accordingly
            if (uiManager.mouseClickOverUI())
            {
                //
            }
            else
            {
                tileWorld.PlaceBlockInWorld(clickLocation);
            }
        }
        else if(middleClick)
        {
            //What is middle click actually going to be used for?
            //check if click hits UI, then handle accordingly
            if (uiManager.mouseClickOverUI())
            {
                //
            }
            else
            {

            }
        }
        
        data.Reset();
        newInput = true;
    }
    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(transform.position, .39f, groundLayer);
        rb.velocity = new Vector3(mHor * speed, rb.velocity.y, 0f);
        mHor = 0;
        if (newInput)
        {
            if (jump && isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }

            newInput = false;
        }
    }
    void RegisterInput(InputData data)
    {
        if (data.axes != null)
        {
            inputData = data;

            mVert = data.axes[0];
            mHor = data.axes[1];

            jump = data.buttons[0];
            inventoryOpen = data.buttons[1];
            settings = data.buttons[2];

            leftClick = data.clicks[0];
            rightClick = data.clicks[1];
            middleClick = data.clicks[2];

            clickLocation = data.clickLocation;
        }
    }
}
