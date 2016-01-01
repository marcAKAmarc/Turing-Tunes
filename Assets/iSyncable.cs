using System;
public interface iSyncable {
    
    void onSync();
    
    Guid getSyncId();
    
    void registerAsSyncObject();
		
}
