using UnityEngine;
using System.Collections;
using System.Linq;


public class ModPlanter:Module {
	public override void executeRelease(){
		var syncable = host.getGameObject().GetComponent<Syncable> ();
		if (syncable == null)
			return;
		
		TuringScene scene = syncable.getSceneObject ();
		if (scene == null)
			return;
		
		Ground groundtilled = (Ground)scene.syncObjects.Where (x => x.getGameObject().GetComponent<Ground> () != null
		                                                       && x.getGameObject().transform.position.snap () == host.getGameObject().transform.position.snap ()
		                                                       && ((Ground)x.getGameObject().GetComponent<Ground>()).getState () == GroundState.Tilled
		                                                       ).FirstOrDefault ();
		if (groundtilled == null)
			return;
		else {
			groundtilled.setState(GroundState.Planted);
		}
		
	}
}

