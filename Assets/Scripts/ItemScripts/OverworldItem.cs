using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldItem : MonoBehaviour
{
    public BaseItem item;
    CircleCollider2D captureRadius;
    public bool gettingPickedUp;
    void Start()
    {
        gettingPickedUp = false;
        SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = item.icon;
        transform.localScale = new Vector3(.25f,.25f,.25f);
        Rigidbody2D rb = gameObject.AddComponent<Rigidbody2D>();
        BoxCollider2D bc = gameObject.AddComponent<BoxCollider2D>();
        captureRadius = gameObject.AddComponent<CircleCollider2D>();
        captureRadius.isTrigger = true;
        captureRadius.radius *= 2;
        gameObject.tag = "OverworldItem";
    }

    void Update()
    {
        //if item is no longer in a player's view, disable it
        //enable if in a player's view
    }

    public string GetPickedUp()
    {
        gettingPickedUp = true;
        string name = item.name;
        item.amount = 1;
        Destroy(this.gameObject, 0.01f);
        
        return name;
    }
}
