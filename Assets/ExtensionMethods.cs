using UnityEngine;
using System.Collections;

public static class ExtensionMethods {

	public static Vector3 snap(this Vector3 v){
        return new Vector3(Mathf.Round(v.x), Mathf.Round(v.y), Mathf.Round(v.z));
    }
}
