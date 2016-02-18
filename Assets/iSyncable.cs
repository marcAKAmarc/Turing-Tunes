using System;
using UnityEngine;
public interface iSyncable {
    
     GameObject getGameObject();
    //these events happen in this order!
    void onSync();
    void onPostSync();
    void onSyncRelease();
    
    //these are not an event
    Guid getSyncId();
    
    void setSceneObject(TuringScene t);
    
    void setOwner(Guid? ownerId);
    Guid? getOwner();
    
    void registerAsSyncObject();
    
    void alter();
    
    void alter2();
		
}
