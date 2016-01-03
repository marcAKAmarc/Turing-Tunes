using UnityEngine;
using System.Collections;
using System;

public class Runner : MonoBehaviour, iSyncable {
    private Guid id;
    [SerializeField]
    public TuringScene scene;
	private float speed;
    private Vector3 goalPosition;
    private Vector3 previousPosition;
    
    // Use this for initialization
	void updateSpeed () {
	   speed = 1.0f/scene.syncer.Interval;
	}
    
    void Start(){
        registerAsSyncObject();
        goalPosition = transform.position;
        previousPosition = transform.position; 
    }
    
    public void onSync(){
        updateSpeed();
        //rotate and shit
        transform.position = goalPosition;
        previousPosition = goalPosition;
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
       Debug.Log("Speed: " + speed.ToString() +" Goal Position: " + goalPosition.ToString() ); 
	   transform.position += (goalPosition-previousPosition)*speed*Time.deltaTime;
	}
}
