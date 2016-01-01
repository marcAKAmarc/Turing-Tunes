using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TuringScene:MonoBehaviour {
    [HideInInspector]
    public Syncer syncer;
    public List<iSyncable> syncObjects;
    
    void Awake(){
        syncObjects = new List<iSyncable>();
        syncer = new Syncer(1, syncObjects);    
    }
    
    void Start(){
        startTimer();
    }
    void startTimer(){
        StartCoroutine(Timer());  
    }
    IEnumerator Timer() {
        while (true){
            yield return new WaitForSeconds(syncer.Interval);
            syncer.updateSyncObjects();
        }
    }
}
