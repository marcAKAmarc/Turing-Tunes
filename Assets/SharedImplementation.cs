using UnityEngine;
using System.Collections;
using System;
public class Syncable : MonoBehaviour, iSyncable {

    public virtual bool deletable { get; set; }
    public virtual bool preventPlacement { get; set; }
    public virtual bool pickable { get; set; }
    public virtual Transform model { get; set; }

	protected Guid id;
	protected TuringScene scene;
	protected Guid? OwnerId;
	protected DateTime TimeCreated;
	private bool toBeDeleted = false; 

    public Syncable()
    {
        deletable = true;
        preventPlacement = true;
        pickable = false;
    }

	protected virtual void Awake(){
		id = Guid.NewGuid ();
	}

	protected TuringScene getScene(){
		return scene;
	}

	protected virtual void Start(){
        scene = TuringScene.CurrentController;
		TimeCreated = DateTime.Now;
		registerAsSyncObject();
	}
	protected virtual void OnDestroy(){
		if (transform.GetComponent<MarkingController> () != null) {
			var something = "whatever";
		}
        try
        {
            scene.removeSyncObject((iSyncable)this);
        }
        catch(Exception ex)
        {
            throw;
        }
	}
	public void Delete(){
		toBeDeleted = true;
	}

	public virtual GameObject getGameObject(){ 

		return gameObject;
	}

	//these events happen in this order!
	public virtual void onSync(){}
	public virtual void onPostSync(){}
	public virtual void onSyncRelease(){
		if (scene.arena.Size.x <= Mathf.Abs (transform.position.x) 
		|| scene.arena.Size.z <= Mathf.Abs (transform.position.z) 
		|| scene.arena.Size.y <= Mathf.Abs (transform.position.y)) {
			//Destroy (gameObject);
			Delete ();
		}
	}
	public virtual void onSyncDelete(){
		if (toBeDeleted == true) {
			Destroy (gameObject);
		}
	}


	//these are not an event
	public virtual Guid getSyncId(){return id;}
	
	public virtual void setSceneObject(TuringScene t){
		//scene = t;
	}

	public virtual TuringScene getSceneObject(){
		return scene;
	}
	
	public virtual void setOwner(Guid? ownerId){
		OwnerId = ownerId;
	}
	public virtual Guid? getOwner(){
		return OwnerId;
	}
	
	public virtual void registerAsSyncObject(){
		try{
		scene.syncer.addSyncObject ((iSyncable)this);
		}catch(Exception ex){
			throw;
		}
	}
	
	public virtual void alter(){
		transform.rotation = Quaternion.LookRotation(transform.right, transform.up);
	}
	
	public virtual void alter2(){}
}
