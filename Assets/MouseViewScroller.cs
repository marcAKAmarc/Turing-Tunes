using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseViewScroller : MonoBehaviour {

    public Camera cam;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        handleCameraScroll();
	}

    void handleCameraScroll()
    {
        if (PlacementLogic.isMouseOffScreen() || PlacementLogic.isMouseOverUI())
            return;
        var x = Input.mousePosition.x / Screen.width;
        var y = Input.mousePosition.y / Screen.height;

        var camforward = cam.transform.forward;
        var camforwardzerod = new Vector3(camforward.x, 0, camforward.z).normalized;
        var camleft = -cam.transform.right;
        var camleftzerod = new Vector3(camleft.x, 0, camleft.z).normalized;
        var camright = cam.transform.right;
        var camrightzerod = new Vector3(camright.x, 0, camright.z).normalized;
        var camback = -cam.transform.forward;
        var cambackzerod = new Vector3(camback.x, 0, camback.z).normalized;

        Vector3 htravel = Vector3.zero;
        Vector3 vtravel = Vector3.zero;
        if (x < .1f)
            htravel = camleftzerod;
        if (x > .9f)
            htravel = camrightzerod;
        if (y < .1f)
            vtravel = cambackzerod;
        if (y > .75f)
            vtravel = camforwardzerod;

        cam.transform.position += (htravel + vtravel).normalized * Time.deltaTime * 8.0f * /*zoomCoefficient*/1f;

    }
}
