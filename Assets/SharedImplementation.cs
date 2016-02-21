using UnityEngine;
using System.Collections;
using System;
public class Syncable : MonoBehaviour, iSyncable {
	protected Guid id;
	protected TuringScene scene;
	protected Guid? OwnerId;
	protected DateTime TimeCreated;
	protected virtual void Awake(){
		id = Guid.NewGuid ();
	}

	protected TuringScene getScene(){
		return scene;
	}

	protected virtual void Start(){
		TimeCreated = DateTime.Now;
		registerAsSyncObject();
	}
	protected virtual void OnDestroy(){
		scene.removeSyncObject((iSyncable)this);
	}
	public virtual GameObject getGameObject(){ return gameObject;}

	//these events happen in this order!
	public virtual void onSync(){}
	public virtual void onPostSync(){}
	public virtual void onSyncRelease(){
		if (scene.arena.Size.x <= Mathf.Abs (transform.position.x) 
		|| scene.arena.Size.z <= Mathf.Abs (transform.position.z) 
		|| scene.arena.Size.y <= Mathf.Abs (transform.position.y)) {
			Destroy (gameObject);	
		}
	}


	//these are not an event
	public virtual Guid getSyncId(){return id;}
	
	public virtual void setSceneObject(TuringScene t){
		scene = t;
	}
	
	public virtual void setOwner(Guid? ownerId){
		OwnerId = ownerId;
	}
	public virtual Guid? getOwner(){
		return OwnerId;
	}
	
	public virtual void registerAsSyncObject(){
		scene.syncer.addSyncObject ((iSyncable)this);
	}
	
	public virtual void alter(){
		transform.rotation = Quaternion.LookRotation(transform.right, transform.up);
	}
	
	public virtual void alter2(){}
}
