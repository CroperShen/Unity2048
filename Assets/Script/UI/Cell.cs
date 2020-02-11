using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using MyEvent;
using MyExtent;
using GlobalSettings;

class Cell : InterfaceObject<Cell>
{   static Cell()
    {
        SetAnmEndFunc("show", "Refresh");
        SetAnmEndFunc("hide", "Refresh");

        SetAnmActFunc("move", "AnmMoveAct");
        SetAnmEndFunc("move", "Refresh");
    }
    public Color[] txtcolors = new Color[12]{
        new Color(0.2f, 0.1f,0.1f),
        new Color(0.2f, 0.1f,0.1f),
        new Color(0.2f, 0.1f,0.1f),
        new Color(0.2f, 0.1f,0.1f),
        new Color(0.2f, 0.1f,0.1f),
        new Color(0.2f, 0.1f,0.1f),
        new Color(0.2f, 0.1f,0.1f),
        new Color(0.2f, 0.1f,0.1f),
        new Color(0.2f, 0.1f,0.1f),
        new Color(0.2f, 0.1f,0.1f),
        new Color(0.2f, 0.1f,0.1f),
        new Color(0.2f, 0.1f,0.1f),
    };
    public Color[] bgcolors = new Color[12] {
        new Color(1, 0.8f,0.6f),
        new Color(1, 0.8f,0.6f),
        new Color(1, 0.8f,0.6f),
        new Color(1, 0.8f,0.6f),
        new Color(1, 0.8f,0.6f),
        new Color(1, 0.8f,0.6f),
        new Color(1, 0.8f,0.6f),
        new Color(1, 0.8f,0.6f),
        new Color(1, 0.8f,0.6f),
        new Color(1, 0.8f,0.6f),
        new Color(1, 0.8f,0.6f),
        new Color(1, 0.8f,0.6f),
        };

    internal int logical_X;
    internal int logical_Y;
    internal int level;

    private TextMesh _my_txt;


   
    internal Vector2 pos_2d
    {
        get
        {
            var tmp = transform.localPosition;
            return new Vector2(tmp.x / 2.5f + 1.5f, tmp.y / 2.5f + 1.5f);
        }
        set
        {
            var x = (value.x - 1.5f) * 2.5f;
            var y = (value.y - 1.5f) * 2.5f;
            transform.localPosition = new Vector3(x, y, -1);
        }
    }
    private IEnumerator AnmMoveAct()
    {
        yield return null;
        while (true)
        {
            var logical_pos = new Vector2(logical_X, logical_Y);
            pos_2d += frame_length_factor * (logical_pos - pos_2d);
            yield return null;
        }
    }
    private Color txtcolor
    {
        get
        {
            return _my_txt.color;
        }
        set
        {
            _my_txt.color = value;
        }
    }

    private int _txt_num;
    private int txt_num
    {
        get
        {
            return _txt_num;
        }
        set
        {
            _txt_num = value;
            var sz = _txt_num.ToString();
            _my_txt.text = sz;
            _my_txt.fontSize = 240 / (2 + sz.Length);
        }
    }
    private MeshRenderer _my_background;
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
    internal new void Initialize()
    {
        base.Initialize();
        _my_txt = transform.Find("text").GetComponent<TextMesh>();
        _my_background = transform.Find("background").GetComponent<MeshRenderer>();
    }
    private IEnumerator Start()
    {
        Refresh();
        scale_2d = new Vector2(0.0f, 0.0f);
        yield return new WaitForSeconds(0.1f);
        PlayAnimation("show");
    }
    private void Refresh()
    {
        if (level < 0)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
            return;
        }
        scale_2d = new Vector2(1, 1);
        pos_2d = new Vector2(logical_X, logical_Y);
        txt_num =(int) Math.Pow(2, level);
        txtcolor = txtcolors[level];
        bgcolor = bgcolors[level];

    }
    private void Awake()
    {
        EventManager.add_listener<EVENT_MOUSE_DRAG>(OnMouseDrag2);
    }
    private void OnDestroy()
    {
        EventManager.remove_listener<EVENT_MOUSE_DRAG>(OnMouseDrag2);
    }
    private void OnMouseDrag2(EVENT_BASE e)
    {
        var tmp = new Vector2(logical_X, logical_Y);
        tmp += e.ToDeliever<EVENT_MOUSE_DRAG>().offset;
        tmp.x.SetInRange(0, 3);
        tmp.y.SetInRange(0, 3);
        pos_2d = tmp;
    }
}
