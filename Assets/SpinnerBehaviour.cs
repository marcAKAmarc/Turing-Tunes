using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinnerBehaviour : MonoBehaviour {

    private float xAdd = 20f;
    private float yAdd = .1f;
    private float zAdd = .1f;
    private float speed = 360f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        var euler = transform.rotation.eulerAngles;
        var newAngle = Quaternion.Euler(new Vector3(euler.x + (xAdd*Time.deltaTime), euler.y, euler.z));
        transform.RotateAround(transform.position, Vector3.one, speed * Time.deltaTime);
	}
}
