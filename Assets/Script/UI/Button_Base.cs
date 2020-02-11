using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyEvent;

public class Button_Base : MonoBehaviour
{

    public Color color_mouseover = new Color(0.4f, 0.4f, 0.4f);
    public Color color_mousedown = new Color(0.2f, 0.2f, 0.2f);
    public string my_event_label;
    public MeshRenderer _my_background;


    private bool mouse_in_region = false;
    private Color color_normal;

    private Color bgcolor
    {
        get
        {
            return _my_background.material.color;
        }

        set
        {
            _my_background.material.color = value;
        }
    }
    private void Awake()
    {
        GetComponent<BoxCollider>().size = new Vector3(_my_background.transform.localScale.x * 10, _my_background.transform.localScale.z * 10, 0.1f);
        color_normal = bgcolor;
    }
    internal virtual List<EVENT_BASE> ExtraEvent()
    {
        return null;
    }
    private void OnMouseEnter()
    {
        bgcolor = color_mouseover;
        mouse_in_region = true;
    }
    private void OnMouseDown()
    {
        bgcolor = color_mousedown;
    }
    private void OnMouseUp()
    {
        if (mouse_in_region)
        {
            bgcolor = color_mouseover;
            var e = new EVENT_BUTTON_CLICK();
            e.button_label = my_event_label;
            EventManager.dispatch_event(e);
            var ret= ExtraEvent();
            if (ret != null)
            {
                foreach (var extraevent in ret)
                {
                    EventManager.dispatch_event(extraevent);
                }
            }

        }
    }
    private void OnMouseExit()
    {
        bgcolor = color_normal;
        mouse_in_region = false;
    }
}
