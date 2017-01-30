using System;
using UnityEngine;
public interface iSyncable {
    
     GameObject getGameObject();
    //these events happen in this order!
    void onSync();
    void onPostSync();
    void onSyncRelease();
	void onSyncDelete();
    
	void Delete();
    //these are not an event
    Guid getSyncId();
    
    void setSceneObject(TuringScene t);
	TuringScene getSceneObject();
    
    void setOwner(Guid? ownerId);
    Guid? getOwner();
    
    void registerAsSyncObject();
    
    void alter();
    
    void alter2();
		
}
