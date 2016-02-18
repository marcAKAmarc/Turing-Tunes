﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
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
            sync.onPostSync();
            sync.onSyncRelease();
        }
    }
    
    public void removeSyncObject( iSyncable obj){
        SyncObjects.Remove(obj);
    }
    
    public List<iSyncable> findSyncObjectsByPosition(Vector3 pos){
        List<iSyncable> found = SyncObjects.Where(x=>x.getGameObject().transform.position == pos).ToList();
        return found;
    }
    
    public iSyncable findSyncObjectsById(Guid id){
        iSyncable found = SyncObjects.Where(x=>x.getSyncId() == id).FirstOrDefault();
        return found;
    }
    
    
}
