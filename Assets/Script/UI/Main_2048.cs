using System.Collections;
using System;
using UnityEngine;
using MyEvent;
using MyExtent;



public class Main_2048 : MonoBehaviour
{
    public Camera main_cmr;
    private Vector2 mouse_pos;
    private void Awake()
    {
        EventManager.dispatch_event(new EVENT_GAME_RESTART());
    }
    private Vector2 GetMousePos()
    {
        return main_cmr.ScreenToWorldPoint(Input.mousePosition);
    }

    private void Start()
    {
        EventManager.dispatch_event(new EVENT_GAME_RESTART());
    }
    private void OnMouseDown()
    {
        mouse_pos = GetMousePos();
    }
    private void OnMouseDrag()
    {
        var mouse_pos_now = GetMousePos();
        var delta_x = mouse_pos_now.x - mouse_pos.x;
        var delta_y = mouse_pos_now.y - mouse_pos.y;
        var e = new EVENT_MOUSE_DRAG();
        e.offset = new Vector2(delta_x, delta_y);
        if (Math.Abs(e.offset.x) > Math.Abs(e.offset.y))
        {
            e.offset.y = 0;
        }
        else
        {
            e.offset.x = 0;
        }
        var l = e.offset.magnitude;
        var lt = 0f;
        float a = 0.6f;
        float b = 0.05f;
        if (l > 0)
        {
            lt = a - a/ (1 + l*b);
        }
        e.offset.SetMagnitude(lt);
        EventManager.dispatch_event(e);
    }
    private void OnMouseUp()
    {
        var mouse_pos_now = GetMousePos();
        var delta_x = mouse_pos_now.x - mouse_pos.x;
        var delta_y = mouse_pos_now.y - mouse_pos.y;
        var min_move_range = 0.5;
        if (delta_x * delta_x + delta_y * delta_y < min_move_range * min_move_range)
        {
            return;
        }
        else
        {
            var e = new EVENT_GAME_OPERATE();
            var  angle = Math.Atan2(delta_y, delta_x);
            e.direct = (int)(2 * (angle + Math.PI*9  / 4) / Math.PI)%4;
            EventManager.dispatch_event(e);
        }
    }
    
}
