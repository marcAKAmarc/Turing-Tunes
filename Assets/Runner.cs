using UnityEngine;
using System.Collections;
using System;
using System.Linq;

public class Runner : MonoBehaviour, iSyncable {
    private Guid id;
    [SerializeField]
    
    private Guid? OwnerId = null;
    public TuringScene scene;
	private float speed;
    private Vector3 goalPosition;
    private Vector3 previousPosition;
    private int FightThreshold = 3;
    private Guid? BlockedBy;
    private int TicksBlocked;
    
    // Use this for initialization
	void updateSpeed () {
	   speed = 1.0f/scene.syncer.Interval;
	}
    
    void Start(){
        registerAsSyncObject();
        registerToScene();
        goalPosition = transform.position;
        previousPosition = transform.position; 
    }
    
    void OnDestroy(){
        scene.removeSyncObject((iSyncable)this);
    }
    
    public void onSync(){
        updateSpeed();
        transform.position = goalPosition.snap();
        
    }
    
    public void onPostSync(){
        //check positions
        handlePagodaRotation();
    
        previousPosition = goalPosition;
        Vector3 requestPosition = (goalPosition+transform.forward).snap();
        if(checkPositionFree(requestPosition)){
            goalPosition =(goalPosition + transform.forward).snap();
        }
    }
    
    public void onSyncRelease(){
        if(TicksBlocked>FightThreshold){
            //scene.removeSyncObject((iSyncable)this);
            var other = scene.syncer.findSyncObjectsById((Guid)BlockedBy);
            if (other != null)
                Destroy(other.getGameObject());
        }
        
        if(Mathf.Abs(transform.position.x) > 200.0f || Mathf.Abs(transform.position.z) > 200.0f){
            Destroy(gameObject);
        }
    }
    
    public Guid getSyncId(){
        return id;
    }
	
    public void registerToScene(){
        scene.Runners.Add(this);
    }
    
    public void registerAsSyncObject(){
        if (id==Guid.Empty)
            id = Guid.NewGuid();
        scene.syncer.addSyncObject((iSyncable)this);
    }
    public void handlePagodaRotation(){
        PagodaController pc = scene.Pagodas.Where(x=>x.transform.position == transform.position).FirstOrDefault();
        if(pc!=null){
            transform.rotation = pc.transform.rotation;
        }
    }
    
    public void  setSceneObject(TuringScene t){
        scene = t;
    }
	// Update is called once per frame
	void Update () { 
	   transform.position += (goalPosition-previousPosition)*speed*Time.deltaTime;
	}
    
    private bool checkPositionFree(Vector3 pos){
        bool free = true;
        foreach(var runner in scene.Runners){
            if(runner.getGoalPosition() == pos || runner.getPreviousPosition() == pos){
                free= false;
                if(runner.getPreviousPosition() == pos)
                    registerBlockedByRunner(runner);               
                break;
            }
        }
        if (free)
        {
            TicksBlocked = 0;
            BlockedBy = null;
        }
        
        return free;            
    }
    
    public Vector3 getGoalPosition(){
        return goalPosition;
    }
    
    public Vector3 getPreviousPosition(){
        return previousPosition;
    }
    
    public GameObject getGameObject(){
        return gameObject;
    }
    
    public void alter(){
        transform.rotation = Quaternion.LookRotation(transform.right, transform.up);
    }
    public void alter2(){
    }
    
    private void registerBlockedByRunner(Runner r){
        if (BlockedBy != r.getSyncId()){
            TicksBlocked = 1;
            BlockedBy = r.getSyncId();
        }
        else
            TicksBlocked +=1;
    }
    
    public void setOwner(Guid? id){
        OwnerId = id;
    }
    public Guid? getOwner(){
        return OwnerId;
    }
}
