using UnityEngine;
using System.Collections;
using System;
using System.Linq;
public class Spawner : MonoBehaviour, iSyncable {
    
    Guid? OwnerId = null;
    public TuringScene scene;
    public Transform spawnObject;
    public int spawnCycleLength;
    private int tickCount;
    
    private Guid id;
	// Use this for initialization
	
    
    
    void Awake(){
        id = Guid.NewGuid();
        tickCount = 0;
    }
    
    void Start(){
        registerAsSyncObject();
        registerToScene();
    }
    
	public void onSync(){
//        Debug.Log("synced...");
        tickCount+=1;
        tickCount = tickCount % spawnCycleLength;
        if(tickCount == 0)
            spawnSpawnObject();
    }
    public void onPostSync(){
        
    }
    public void onSyncRelease(){
        
    }
    public Guid getSyncId(){
       return id; 
    }
    public void registerAsSyncObject(){
        scene.syncer.addSyncObject(this);
    }
    
    public void registerToScene(){
        scene.Spawners.Add(this);
    }
    private Transform spawnSpawnObject(){
        //don't spawn if there is a runner in the way
        var syncObjects = scene.syncObjects.Where(x=>x.getGameObject().transform.position.snap() == transform.position).ToList();
        foreach(var sync in syncObjects){
            var runner = sync.getGameObject().GetComponent<Runner>();
            if(runner != null)
                return null;
        }
        Transform t = Instantiate(spawnObject, transform.position.snap(), transform.rotation) as Transform;
        t.GetComponent<iSyncable>().setOwner(OwnerId);
        t.GetComponent<iSyncable>().setSceneObject(scene); //how to do this genericly
        return t;    
    }
    
    public void setSceneObject(TuringScene t){
        scene = t;
    }
    
    public void alter(){
        transform.rotation = Quaternion.LookRotation(transform.right, transform.up);
    }
    
    public void alter2(){
        
    }
	// Update is called once per frame
	void Update () {
	
	}
    
    public GameObject getGameObject(){
        return gameObject;
    }
    
    public void setOwner(Guid? id){
        OwnerId = id;
    }
    public Guid? getOwner(){
        return OwnerId;
    }
}
