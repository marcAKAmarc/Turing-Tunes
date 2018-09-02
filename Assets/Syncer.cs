using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
public class Syncer{
	public TuringScene scene;
    public static float Interval;
    private List<iSyncable> SyncObjects;
    private float Start;
   
    public Syncer(TuringScene myscene ,float interval, List<iSyncable> syncobjects) {
		scene = myscene;
		Interval = interval;
        SyncObjects = syncobjects;
        Start = Time.time;
    }
    
    public void addSyncObject( iSyncable obj){
        SyncObjects.Add(obj);
    }
   
    public void updateSyncObjects(){
        foreach (iSyncable sync in SyncObjects) {
            if(sync.getGameObject().activeInHierarchy)
			    sync.onSync ();
		}
		foreach (iSyncable sync in SyncObjects) {
            if (sync.getGameObject().activeInHierarchy)
                sync.onPostSync ();
		}
		foreach (iSyncable sync in SyncObjects) {
            if (sync.getGameObject().activeInHierarchy)
                sync.onSyncRelease ();
		}
		foreach (iSyncable sync in SyncObjects) {
            if (sync.getGameObject().activeInHierarchy)
                sync.onSyncDelete ();
		}
    }
    
    public void removeSyncObject( iSyncable obj){
        SyncObjects.Remove(obj);
    }
    
    public List<iSyncable> findSyncObjectsByPosition(Vector3 pos){
        List<iSyncable> found = SyncObjects.Where(x=>x.getGameObject().transform.position == pos).ToList();
        return found;
    }

    public List<iSyncable> findSyncObjectsBySnappedPosition(Vector3 pos)
    {
        List<iSyncable> found = SyncObjects.Where(x => x.getGameObject().transform.position.snap() == pos).ToList();
        return found;
    }
    
    public iSyncable findSyncObjectsById(Guid id){
        iSyncable found = SyncObjects.Where(x=>x.getSyncId() == id).FirstOrDefault();
        return found;
    }
    
    
}
