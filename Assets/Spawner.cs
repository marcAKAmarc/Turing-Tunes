using UnityEngine;
using System.Collections;
using System;
using System.Linq;
public class Spawner : Syncable {
    

    public Transform spawnObject;
    public int spawnCycleLength;
    public bool OneAtATime;
    private int tickCount;
    private Transform previouslyCreatedItem;

    
    protected override void Start(){
		base.Start ();
        registerToScene();
    }

    public override void onSyncRelease(){
        if (OneAtATime )
        {
            if(!(previouslyCreatedItem != null))
                previouslyCreatedItem = spawnSpawnObject();
        }
        else {
            tickCount += 1;
            tickCount = tickCount % spawnCycleLength;
            if (tickCount == 0 && checkPositionFree(transform.position))
                previouslyCreatedItem = spawnSpawnObject();
            base.onSyncRelease();
        }
    }
    
    public void registerToScene(){
        scene.Spawners.Add(this);
    }
    private Transform spawnSpawnObject(){
        //don't spawn if there is a runner in the way
        var syncObjects = base.getScene().syncObjects.Where(x=>x.getGameObject().transform.position.snap() == transform.position).ToList();
        foreach(var sync in syncObjects){
            var runner = sync.getGameObject().GetComponent<Runner>();
            if(runner != null)
                return null;
        }
        Transform t = Instantiate(spawnObject, transform.position.snap(), transform.rotation) as Transform;
        var syncableComponents = t.GetComponents<iSyncable>();
        foreach(var syncableComponent in syncableComponents)
        {
            syncableComponent.setOwner(OwnerId);
            syncableComponent.setSceneObject(scene); //how to do this genericly
        }

        return t;    
    }

	private bool checkPositionFree(Vector3 pos){
		bool free = true;
		foreach (var runner in base.getScene().Runners) {
			if (runner.getCurrentPosition ().snap () == pos.snap () || runner.getGoalPosition().snap ()==pos.snap ()) {
				free = false;
				break;
			}
		}
		
		return free;
	}
}
