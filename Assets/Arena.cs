using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class Arena {

	public Vector3 Size;

	public Arena(Vector3 size){
		Size = size;
	}
	public Arena(){
		Size = new Vector3 (5, 5, 5);
	}
}
