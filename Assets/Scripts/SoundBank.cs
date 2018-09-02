using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public enum SoundName { Kick, Snare, Clap, HatClosed, HatOpen}
public enum PlayMode { Gameplay, Goal}
[Serializable]
public class SoundBankItem
{
    public SoundName Name;
    public AudioSource Source;
    public Sprite Icon;
}

public class SoundBank : Syncable
{
    private static SoundBank singleInstance = null;
    public TuringScene Scene;
    public PlayMode playMode = PlayMode.Gameplay;
    public List<SoundBankItem> soundBank;
    private List<SoundName> onDeckPlays;

    protected override void Awake()
    {
        //base.setSceneObject(Scene);
        base.Awake();
        onDeckPlays = new List<SoundName>();
        singleInstance = this;
    }
    /*protected override void Start()
    {
        base.setSceneObject(TuringScene.CurrentController);
    }*/
    public static SoundBank Get()
    {
        return singleInstance;
    }
    public SoundBankItem GetSoundBankItem(SoundName name)
    {
        return soundBank.First(x => x.Name == name);
    }
    public void Play(SoundName name)
    {
        if (playMode != PlayMode.Gameplay)
            return;
        var sourceItem = soundBank.First(x => x.Name == name);
        sourceItem.Source.Play();
        onDeckPlays.Add(name);
    }

    public void PlayGoalBeat(SoundName name)
    {
        if (playMode != PlayMode.Goal)
            return;
        var sourceItem = soundBank.First(x => x.Name == name);
        sourceItem.Source.Play();
    }
    
    public void SetPlayMode(PlayMode newPlayMode)
    {
        playMode = newPlayMode;
    }
    public override void onPostSync()
    {
        base.onPostSync();
        scene.GoalBeat.reportMostRecentLedger(onDeckPlays);
        onDeckPlays = new List<SoundName>();
        
    }
}




public static class AudioSourceExtensions
{
    public static AudioSource Copy(this AudioSource src)
    {
        var copy = new AudioSource();
        copy.loop = src.loop;
        copy.ignoreListenerPause = src.ignoreListenerPause;
        copy.ignoreListenerVolume = src.ignoreListenerVolume;
        copy.playOnAwake = src.playOnAwake;
        copy.velocityUpdateMode = src.velocityUpdateMode;
        copy.panStereo = src.panStereo;
        copy.spatialBlend = src.spatialBlend;
        copy.spatialize = src.spatialize;
        copy.clip = src.clip;
        copy.spatializePostEffects = src.spatializePostEffects;
        copy.bypassEffects = src.bypassEffects;
        copy.bypassListenerEffects = src.bypassListenerEffects;
        copy.bypassReverbZones = src.bypassReverbZones;
        copy.dopplerLevel = src.dopplerLevel;
        copy.priority = src.priority;
        copy.mute = src.mute;
        copy.minDistance = src.minDistance;
        copy.maxDistance = src.maxDistance;
        copy.rolloffMode = src.rolloffMode;
        copy.timeSamples = src.timeSamples;
        copy.time = src.time;
        copy.pitch = src.pitch;
        copy.volume = src.volume;

        return copy;
    }

    
}
