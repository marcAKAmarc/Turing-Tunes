using UnityEngine;
using System.Collections;
using System.Linq;


public class ModTiller:Module {
	public Transform groundTilledPrefab;
	public override void executeRelease(){
		var syncable = host.getGameObject().GetComponent<Syncable> ();
		if (syncable == null)
			return;
		
		TuringScene scene = syncable.getSceneObject ();
		if (scene == null)
			return;
		
		MarkingController marker = (MarkingController)scene.syncObjects.Where (x => x.getGameObject().GetComponent<MarkingController> () != null
		                                                    && x.getGameObject().transform.position.snap () == host.getGameObject().transform.position.snap ()
		                                                    ).FirstOrDefault ();
		if (marker == null)
			return;
		else {
			marker.Delete ();
			Transform t = Instantiate(groundTilledPrefab, host.getGameObject().transform.position.snap (), Quaternion.identity) as Transform;
			//set owner?
			t.GetComponent<iSyncable>().setSceneObject(scene); //how to do this genericly
			t.GetComponent<Ground>().setState(GroundState.Tilled);
		}
		
	}
}



