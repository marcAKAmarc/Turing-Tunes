using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class GoalBeat: Syncable
{

    public List<BeatMoment> Beat;
    public List<GoalBeatDisplayAdapter> DisplayAdapters;
    public bool gameWon = false;
    public int gameBeatIndex = 0;
    public int playIndex = 0;
    public bool play;
    public TuringScene Scene;

    [Serializable]
    public class BeatMoment
    {
        public List<SoundName> sounds;
    }
    // Use this for initialization

    protected override void Awake()
    {
        setSceneObject(Scene);
        base.Awake();
        
    }
    public void reportMostRecentLedger(List<SoundName> sounds)
    {
        var goalSounds = Beat[gameBeatIndex].sounds;

        //if there are any in goalsounds that don't have a match in sounds
        if (goalSounds.Any(gs => !sounds.Any(s => s == gs)))
        {
            gameBeatIndex = 0;
        }
        else
        {
            gameBeatIndex += 1;
        }

        if (gameBeatIndex >= Beat.Count())
        {
            gameWon = true;
            gameBeatIndex = 0;
        }
    }
    public bool TogglePlay()
    {
        play = !play;
        if (play)
        {
            scene.GetComponent<SoundBank>().SetPlayMode(PlayMode.Goal);
            playIndex = 0;
        }
        else
        {
            scene.GetComponent<SoundBank>().SetPlayMode(PlayMode.Gameplay);
            foreach(var displayAdapter in DisplayAdapters)
            {
                displayAdapter.SetHighlight(null);
            }
        }
        return play;
    }
    public override void onSync()
    {
        base.onSync();

        //play maybe
        if (play)
        {
            var sounds = Beat[playIndex].sounds;
            foreach (var sound in sounds)
            {
                scene.GetComponent<SoundBank>().PlayGoalBeat(sound);
            }
            foreach(var displayAdapter in DisplayAdapters)
            {
                displayAdapter.SetHighlight(playIndex);
            }
            playIndex = (playIndex + 1) % Beat.Count();
        }
    }

    public List<SoundScore> GetBeatScore()
    {
        List<SoundScore> scores = new List<SoundScore>();
        List<SoundName> instruments = Beat.SelectMany(x => x.sounds).Distinct().ToList();
        for(var i = 0; i < instruments.Count(); i++)
        {
            var instrument = instruments[i];
            var soundScore = new SoundScore();
            soundScore.name = instrument;
            foreach(var beat in Beat)
            {
                if (beat.sounds.Any(x => x == instrument))
                    soundScore.score.Add(true);
                else
                    soundScore.score.Add(false);
            }

            scores.Add(soundScore);
        }
        return scores;
    }
}
public class SoundScore
{
    public SoundName name;
    public List<bool> score = new List<bool>();
}