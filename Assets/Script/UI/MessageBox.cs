using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyEvent;

class MessageBox : InterfaceObject<MessageBox>
{
    public TextMesh my_caption;
    public GameObject hiscore_signer;
    public InputField hiscore_name;

    private GamePlayInfo last_hiscore = new GamePlayInfo();
    private Vector3 text1_pos;
    private GamePlayInfo lastgamedata;

    private int mystate=0;
    private void Awake()
    {
        Initialize();
        text1_pos = my_caption.transform.localPosition;
        EventManager.add_listener<EVENT_GAME_FINISHED>(OnGameFinished);
        EventManager.add_listener<EVENT_BUTTON_CLICK>(OnButtonClick);
        EventManager.add_listener<EVENT_HISCORE_SCORELINE_CHANGED>(OnHighScoreLineChanged);
        EventManager.add_listener<EVENT_HISCORE_DATA_LOADED>(OnHiscoreDataLoaded);
    }

    private void Start()
    {
        scale_2d = new Vector2(0.00f, 0.00f);
    }

    private IEnumerator OnGameFinished_Coroutine()
    {
        yield return new WaitForSeconds(0.5f);
        PlayAnimation("show");
    }
    private void OnGameFinished(EVENT_BASE e)
    {
        var evnt = e.ToDeliever<EVENT_GAME_FINISHED>();
        if (evnt.step == 1)
        {
            my_caption.text = evnt.data.iswin ? "你赢了!" : "你无路可走了";

            if (evnt.data.CompareTo(last_hiscore)>0)
            {
                mystate = 1;
                lastgamedata = evnt.data;
                my_caption.transform.localPosition = text1_pos;
                hiscore_name.text = lastgamedata.name;
                if (lastgamedata.name == "")
                {
                    hiscore_name.text = "无名";
                }
                else
                {
                    hiscore_name.text = lastgamedata.name;
                }
                hiscore_signer.SetActive(true);
            }
            else
            {
                mystate = 0;
                my_caption.transform.localPosition = text1_pos + new Vector3(0, -0.5f,0);
                hiscore_signer.SetActive(false);
            }
            StartCoroutine(OnGameFinished_Coroutine());
        }
    }


    private void OnHighScoreLineChanged(EVENT_BASE e)
    {
        last_hiscore = e.ToDeliever<EVENT_HISCORE_SCORELINE_CHANGED>().last_hiscore;
    }
    private void OnButtonClick(EVENT_BASE e)
    {
        if (mystate >= 0)
        {
            var evnt = e.ToDeliever<EVENT_BUTTON_CLICK>();
            switch (evnt.button_label)
            {
                case "continue":
                case "restart":
                    if (mystate == 1)
                    {
                        NewHisocreDispatch();
                    }
                    mystate = -1;
                    PlayAnimation("hide");
                    break;

            }
        }
    }

    private void OnHiscoreDataLoaded(EVENT_BASE e)
    {
        lastgamedata.name = e.ToDeliever<EVENT_HISCORE_DATA_LOADED>().lastname;
        
    }
    private void NewHisocreDispatch()
    {
        var e = new EVENT_NEW_HISCORE_RECORDED();
        e.data = lastgamedata;
        e.data.name = hiscore_name.text;
        EventManager.dispatch_event(e);
    }

}

