using UnityEngine;
using System.Collections;
using System;

public class Runner : MonoBehaviour, iSyncable {
    private Guid id;
    [SerializeField]
    public TuringScene scene;
	private float speed;
    private Vector3 goalPosition;
    
    // Use this for initialization
	void updateSpeed () {
	   speed = 1.0f/scene.syncer.Interval;
	}
    
    void Start(){
        registerAsSyncObject();
        goalPosition = transform.position; 
    }
    
    public void onSync(){
        updateSpeed();
        //rotate and shit
        goalPosition += transform.forward;
    }
    
    public Guid getSyncId(){
        return id;
    }
	
    public void registerAsSyncObject(){
        if (id==Guid.Empty)
            id = Guid.NewGuid();
        scene.syncer.addSyncObject((iSyncable)this);
    }
	// Update is called once per frame
	void Update () {
	   transform.position += (transform.position - goalPosition)*speed*Time.deltaTime;
	}
}
