using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class SampleButtonBehavior : ButtonBehavior
{
    public List<AudioEnvelope> Envelopes;
    public AudioSource HeldAudio;
    public AudioSource PressedAudio;
    public AudioSource ReleasedAudio;
    public override void UpdateState(SampleState state)
    {
        //handleAudioTrigger(state);
        handleFiltersUpdate(state);
    }
    protected void handleAudioTrigger(SampleState state) { 
        switch (state)
        {
            case SampleState.Held:
                break;
            case SampleState.Pressed:
                if(HeldAudio!=null)
                    HeldAudio.Play();
                if(PressedAudio!=null)
                    PressedAudio.Play();
                break;
            case SampleState.Released:
                if(HeldAudio!=null)
                    HeldAudio.Stop();
                if(ReleasedAudio!=null)
                    ReleasedAudio.Play();
                break;
            default:
                break;
        }
    }

    protected void handleFiltersUpdate(SampleState state)
    {
        //Helpers.CreateComponentIfMissing(transform, Envelopes);
        foreach(AudioEnvelope envelope in Envelopes)
        {
            if (state == SampleState.Pressed) {
                StartCoroutine("adaptAttack", envelope);
                StopCoroutine("adaptRelease");
            }
            if (state == SampleState.Released)
            {
                StartCoroutine("adaptRelease", envelope);
                StopCoroutine("adaptAttack");
            }

        }
    }

    IEnumerator adaptAttack(AudioEnvelope envelope)
    {
        float t = 0f;
        while(t <= Syncer.Interval)
        {
            float val = envelope.Attack.Evaluate(t / Syncer.Interval);

            //get it
            switch (envelope.filter)
            {
                case AudioEnvelope.FilterType.Highpass:
                    var comp = transform.GetComponent<AudioHighPassFilter>();
                    if (envelope.control == AudioEnvelope.ControlType.Level)
                        comp.cutoffFrequency = (val * 21990f) + 10f;
                    else
                        comp.highpassResonanceQ = (val * 9f) + 1f;
                    break;
                case AudioEnvelope.FilterType.Lowpass:
                    //StartCoroutine("adaptLowPass", envelope);
                    break;
                case AudioEnvelope.FilterType.Pitch:
                    //StartCoroutine("adaptPitch", envelope);
                    break;
                case AudioEnvelope.FilterType.Volume:
                    var sources = transform.GetComponents<AudioSource>();
                    foreach (var source in sources)
                    {
                        source.volume = Mathf.Clamp01(val);
                    }
                    break;
            }
            t += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator adaptRelease(AudioEnvelope envelope)
    {
        float? startval = null;
        float t = 0f;
        while (t <= Syncer.Interval)
        {
            float val = envelope.Release.Evaluate(t / Syncer.Interval);

            //get it
            switch (envelope.filter)
            {
                case AudioEnvelope.FilterType.Highpass:
                    var comp = transform.GetComponent<AudioHighPassFilter>();
                    if (envelope.control == AudioEnvelope.ControlType.Level)
                        comp.cutoffFrequency = (val * 21990f) + 10f;
                    else
                        comp.highpassResonanceQ = (val * 9f) + 1f;
                    break;
                case AudioEnvelope.FilterType.Lowpass:
                    //StartCoroutine("adaptLowPass", envelope);
                    break;
                case AudioEnvelope.FilterType.Pitch:
                    //StartCoroutine("adaptPitch", envelope);
                    break;
                case AudioEnvelope.FilterType.Volume:
                    var sources = transform.GetComponents<AudioSource>();
                    foreach (var source in sources)
                    {
                        if (startval == null)
                            startval = source.volume;
                        source.volume = Mathf.Clamp01(val) * (float)startval;
                    }
                    break;
            }
            t += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }


    private static class Helpers
    {
        public static void CreateComponentIfMissing(Transform t, List<AudioEnvelope> Envelopes) {
            foreach (var env in Envelopes)
            {
                System.Type c = null;
                switch (env.filter)
                {
                    case AudioEnvelope.FilterType.Highpass:
                        c =  typeof(AudioHighPassFilter);
                        break;
                    case AudioEnvelope.FilterType.Lowpass:
                        c = typeof(AudioLowPassFilter);
                        break;
                    case AudioEnvelope.FilterType.Pitch:
                        c = typeof(AudioSource);
                        break;
                    case AudioEnvelope.FilterType.Volume:
                        c = typeof(AudioSource);
                        break;
                }
                if (c != null)
                {
                    var components = t.GetComponents(c.GetType());
                    if (components.Length == 0)
                        t.gameObject.AddComponent(c.GetType());
                }
            }
        }
    }
    
}

[System.Serializable]
public class AudioEnvelope {
    public AnimationCurve Attack;
    public AnimationCurve Release;
    public enum FilterType { Highpass, Lowpass, Bandpass, Volume, Pitch }
    public enum ControlType {Level, Resonance}
    public FilterType filter;
    public ControlType control;
}



