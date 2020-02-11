using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using MyEvent;

class HiscoreBoard : InterfaceObject<HiscoreBoard>
{
    static readonly string[] captions = new string[5] { "第一名", "第二名", "第三名", "第四名", "第五名" };
    public HiscoreBoardText text_prototype = null;
    private HiscoreBoardText[] lines;
    private List<GamePlayInfo> datalist = new List<GamePlayInfo>();

    private void Awake()
    {
        Initialize();
        lines = new HiscoreBoardText[5];
        for (int i = 0; i < 5; ++i)
        {
            lines[i]= Instantiate(text_prototype, transform);
            lines[i].transform.localPosition = new Vector2(0, 0 - 1.2f*i);
            lines[i].caption = captions[i];
        }

        EventManager.add_listener<EVENT_BUTTON_CLICK>(OnButtonClick);
        EventManager.add_listener<EVENT_NEW_HISCORE_RECORDED>(OnNewHighScoreRecorded);
        scale_2d = new Vector2(0, 0);
    }

    private void Start()
    {
        string path = Application.streamingAssetsPath + "/score.dat";
        try
        {
            using (var f = new BinaryReader(new FileStream(path, FileMode.Open)))
            {
                EVENT_HISCORE_DATA_LOADED e = new EVENT_HISCORE_DATA_LOADED();
                e.lastname = f.ReadString();
                EventManager.dispatch_event(e);
                
                for (int i = 0; i < 5; ++i)
                {
                    GamePlayInfo data;
                    data.name = f.ReadString();
                    data.score = f.ReadInt32();
                    data.iswin = f.ReadBoolean();
                    datalist.Add(data);
                }
            }
        }
        catch (IOException)
        {

        }
        Refresh();
    }

    private void Refresh()
    {
        datalist.Sort((x, y) => -x.CompareTo(y));
        while (datalist.Count > 5)
        {
            datalist.RemoveAt(5);
        }
        for (int i = 0; i < 5; ++i)
        {
            if (i < datalist.Count)
            {
                lines[i].data = datalist[i];
            }
            else
            {
                lines[i].data = null;
            }
        }
        
        var e = new EVENT_HISCORE_SCORELINE_CHANGED();
        if (datalist.Count > 0)
        {
            e.last_hiscore = datalist[datalist.Count - 1];
        }
        EventManager.dispatch_event(e);
    }

    private void OnNewHighScoreRecorded(EVENT_BASE e)
    {
        var evnt = e.ToDeliever<EVENT_NEW_HISCORE_RECORDED>();
        datalist.Add(evnt.data);
        Refresh();
        string path = Application.streamingAssetsPath + "/score.dat";
        try
        {
            using (var f = new BinaryWriter(new FileStream(path, FileMode.OpenOrCreate)))
            {
                f.Write(evnt.data.name);
                foreach (var i in datalist)
                {
                    f.Write(i.name);
                    f.Write(i.score);
                    f.Write(i.iswin);
                }
            }
            
        }
        catch (IOException)
        {
            
        }

        PlayAnimation("show");
    }

    private void OnMouseUp()
    {
        PlayAnimation("hide");
    }

    private void OnButtonClick(EVENT_BASE e)
    {
        var evnt = (EVENT_BUTTON_CLICK)e;
        switch (evnt.button_label)
        {
            case "hiscore":
                PlayAnimation("show");
                break;
        }
    }

    
}
