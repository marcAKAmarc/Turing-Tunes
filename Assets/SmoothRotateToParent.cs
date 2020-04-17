using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothRotateToParent : MonoBehaviour {
    public float turnCoefficient = 30f;
    private Quaternion prevRotation;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void LateUpdate () {
        var goalRotation = transform.parent.rotation;

        transform.rotation = Quaternion.Slerp(prevRotation, goalRotation, turnCoefficient * Time.deltaTime);

        prevRotation = transform.rotation;
    }
}
