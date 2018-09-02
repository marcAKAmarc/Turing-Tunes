using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UI_GoalBeatController : MonoBehaviour {

    public Button playButton;
    public GoalBeat goalBeat;
    public GoalBeatDisplayAdapter displayAdapter;

    private void Awake()
    {
        playButton.GetComponentInChildren<Text>().text = "Play Goal Beat";
    }
    private void Start()
    {
        var playButtonEvent = new UnityEngine.Events.UnityAction(playButtonPlay);
        playButton.onClick.AddListener(playButtonEvent);
    }

    private void playButtonPlay()
    {
        var isPlay = goalBeat.TogglePlay();
        if(isPlay)
            playButton.GetComponentInChildren<Text>().text = "Stop";
        else
            playButton.GetComponentInChildren<Text>().text = "Play Goal Beat";
        displayAdapter.ToggleShow(isPlay);

    }
}
