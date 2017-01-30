using UnityEngine;
using System.Collections;


public interface iButtonReactor
{
    void UpdateState(SampleState state);

    //TODO:  create my first button reactor!
}
public class ButtonBehavior : MonoBehaviour, iButtonReactor {

    public virtual void UpdateState(SampleState state)
    {
        Debug.LogError(state.ToString());
    }
}


