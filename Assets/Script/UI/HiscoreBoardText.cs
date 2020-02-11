using System.Collections;
using System.Collections.Generic;
using UnityEngine;


class HiscoreBoardText : MonoBehaviour
{
    // Start is called before the first frame update
    public TextMesh caption_text;
    public TextMesh name_text;
    public TextMesh score_text;
    public TextMesh iswin_text;

    internal string caption
    {
        get
        {
            return caption_text.text;
        }
        set
        {
            caption_text.text = value;
        }
    }
    private GamePlayInfo? _data;
    internal GamePlayInfo? data{
        get
        {
            return _data;
        }
        set
        {
            _data = value;

            name_text.text = _data?.name??"----";
            score_text.text = _data?.score.ToString()??"----";
            iswin_text.text = (_data!=null)?((bool)_data?.iswin ? "胜利" : "失败"):"----";
        }
    }
}
