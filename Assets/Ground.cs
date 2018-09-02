using UnityEngine;
using System;

public enum GroundState {Tilled, Planted, Untouched}
public class Ground : Syncable{
	[SerializeField]
	private GroundState state;
	public virtual void setState(GroundState gs){
		foreach (Transform child in transform)
			child.gameObject.SetActive (false);

		state = gs;

		if (gs == GroundState.Planted) {
			var planted = transform.Find("Planted");
			planted.gameObject.SetActive(true);
		}
		if (gs == GroundState.Tilled) {
			var tilled = transform.Find("Tilled");
			tilled.gameObject.SetActive(true);
		}
	}
	public virtual GroundState getState(){
		return state;
	}
}
