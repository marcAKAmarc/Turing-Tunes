using UnityEngine;
using System.Collections;
using System;
using System.Linq;
public class Spawner : Syncable {
    

    public Transform spawnObject;
    public int spawnCycleLength;
    private int tickCount;
    

    
    protected override void Start(){
		base.Start ();
        registerToScene();
    }

    public override void onSyncRelease(){
		tickCount+=1;
		tickCount = tickCount % spawnCycleLength;
		if(tickCount == 0 && checkPositionFree(transform.position) )
			spawnSpawnObject();
		base.onSyncRelease ();
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
        t.GetComponent<iSyncable>().setOwner(OwnerId);
        t.GetComponent<iSyncable>().setSceneObject(scene); //how to do this genericly
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
