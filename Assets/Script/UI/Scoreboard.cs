using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyEvent;

class Scoreboard : MonoBehaviour
{
    public TextMesh score_text;
    public TextMesh step_text;

    private int _score;
    private int score
    {
        get
        {
            return _score;
        }
        set
        {
            _score = value;
            score_text.text = _score.ToString();
        }
    }

    private int _step;
    private int step
    {
        get
        {
            return _step;
        }
        set
        {
            _step = value;
            step_text.text = _step.ToString();
        }
    }
    private void Awake()
    {
        EventManager.add_listener<EVENT_GAME_OPERATOR_DONE>(OnGameOperatorDone);
        EventManager.add_listener<EVENT_BUTTON_CLICK>(OnButtonClick);
        EventManager.add_listener<EVENT_GAME_RESTART>(OnGameRestart);
        EventManager.add_listener<EVENT_GAME_FINISHED>(OnGameFinished);
    }

    private void OnButtonClick(EVENT_BASE e)
    {
        var evnt = e.ToDeliever<EVENT_BUTTON_CLICK>();
    }
    private void OnGameFinished(EVENT_BASE e)
    {
        var evnt = e.ToDeliever<EVENT_GAME_FINISHED>();
        if (evnt.step == 0)
        {
            evnt.step = 1;
            evnt.data.score = score;
            EventManager.dispatch_event(e);
        }
    }
    private void OnGameRestart()
    {
        ResetScore();
    }
    private void OnGameOperatorDone(EVENT_BASE e)
    {
        var lst = e.ToDeliever<EVENT_GAME_OPERATOR_DONE>().lst;
        step++;

        int delta_score = 0;
        foreach (int i in lst)
        {
            delta_score += i;
        }
        score += delta_score;
    }
    private void ResetScore()
    {
        score = 0;
        step = 0;
    }
}
