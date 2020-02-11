using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyEvent;

class Button_Restart : Button_Base
{
    internal override List<EVENT_BASE> ExtraEvent()
    {
        var ret = new List<EVENT_BASE>();
        ret.Add(new EVENT_GAME_RESTART());
        return ret;
    }
}
