using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Syncer{
    public float Interval;
    private List<iSyncable> SyncObjects;
    private float Start;
    
    public Syncer(float interval, List<iSyncable> syncobjects) {
        Interval = interval;
        SyncObjects = syncobjects;
        Start = Time.time;
    }
    
    public void addSyncObject( iSyncable obj){
        SyncObjects.Add(obj);
    }
   
    public void updateSyncObjects(){
        foreach(iSyncable sync in SyncObjects){
            sync.onSync();
        }
    }
    
}
