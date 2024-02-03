using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool mouseInside;
    //keep track if controller exits one UI and enters another
    //maybe have boolean for each UI
    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseInside = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseInside = false;
    }
}
