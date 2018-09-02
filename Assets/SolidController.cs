using UnityEngine;
using System.Collections;

public class SolidController: Syncable {
    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start(){
		base.Start ();
		registerToScene();
	}

	private void registerToScene(){
		base.getScene ().Solids.Add (this);
	}
}
