using UnityEngine;
using System.Collections;
using System.Linq;

public interface iSamplable
{
    
}

public interface iFieldReactor
{
    void ApproachingOn();
    void LeavingOn();
    void StayingOn();
    void ApproachingSide();
    void LeavingSide();
    void StayingSide();
}

public enum SampleState { Pressed, Held, Released, Ready, Inactive};

public class SampleController : Syncable, iFieldReactor
{
    private bool on;

    public SoundName sound;
    public bool Deletable;
    protected override void Awake()
    {
        base.Awake();
        base.deletable = Deletable;
    }
    public void playSound()
    {
        scene.GetComponent<SoundBank>().Play(sound);
    }
    public override void onPostSync()
    {
    }

    public void StayingOn()
    {
        if (on == false)
            playSound();
        on = true;
    }
    public void LeavingOn()
    {
        if (on == false)
            playSound();
        on = false;
    }
    public void ApproachingOn() { }
    public void ApproachingSide() { }
    public void LeavingSide() { }
    public void StayingSide() { }
}

//public class SampleController : Syncable, iFieldReactor
//{
//    //should really be called Button Controller

//    private SampleState State;
//    private bool hasBeenActedOn;

//    /*void onPostSync()
//    {
//        int count;
//        count = base.getScene().Runners.Where(r => r.transform.position.x == transform.position.x && r.transform.position.z == transform.position.z).Count();
//        if (count > 0){
//            //update state
//            updateState(true);
//        }
//        else
//        {
//            updateState(false);
//        }
//    }*/

//    private void updateState(bool activated)
//    {
//        var previousState = State;
//        if (activated && (State == SampleState.Pressed || State == SampleState.Held))
//            State = SampleState.Held;
//        else if (activated && (State == SampleState.Released || State == SampleState.Ready))
//            State = SampleState.Pressed;
//        else if (!activated && (State == SampleState.Pressed || State == SampleState.Held))
//            State = SampleState.Released;
//        else if (!activated && (State == SampleState.Ready || State == SampleState.Released))
//            State = SampleState.Ready;

//        if (previousState != State)
//        {
//            var buttonreactors = transform.GetComponents<iButtonReactor>();
//            foreach (var buttonreactor in buttonreactors)
//            {
//                buttonreactor.UpdateState(State);
//            }
//        }
//    }

//    public override void onPostSync() {
//        base.onPostSync();
//        if (!hasBeenActedOn)
//            updateState(false);
//        hasBeenActedOn = false;
//    }

//    public void ApproachingOn()
//    {
//        updateState(true);
//        hasBeenActedOn = true;
//    }
//    public void StayingOn()
//    {
//        updateState(true);
//        hasBeenActedOn = true;
//    }
//    public void LeavingOn()
//    {
//        updateState(true);
//        hasBeenActedOn = true;
//    }
//    public void ApproachingSide(){ }
//    public void LeavingSide() { }
//    public void StayingSide() { }
//}


