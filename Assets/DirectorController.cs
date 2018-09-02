using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DirectorController : MonoBehaviour {
    public Transform Displayable;
    public Transform Placable;
    public Camera cam;
    void OnEnable()
    {
        Displayable.gameObject.SetActive(true);
    }
    private void OnDisable()
    {
        Displayable.gameObject.SetActive(false);
    }
    private bool handleDelete()
    {
        if (PlacementLogic.isMouseOverUI())
        {
            return false;
        }

        var syncobj = TuringScene.CurrentController.syncer.findSyncObjectsByPosition(transform.position.snap()).Where(x=>x is PagodaController).FirstOrDefault();
        if (syncobj != null)
        {
            syncobj.Delete();
            return true;
        }
        return false;
    }
    private void handlePlace()
    {
        if (PlacementLogic.isMouseOverUI())
            return;
        var syncobjs = TuringScene.CurrentController.syncer.findSyncObjectsByPosition(transform.position.snap());

        //if there are objects in the way, don't place anything
        if (syncobjs.Where(x => x.preventPlacement).Count() > 0)
            return;

        Transform t = Instantiate(Placable, transform.position, transform.rotation);
    }
    private void handleRotate()
    {
        var syncobj = TuringScene.CurrentController.syncer.findSyncObjectsByPosition(transform.position.snap()).Where(x => x is PagodaController).FirstOrDefault();
        if (syncobj != null)
        {
            var syncEuler = syncobj.getGameObject().transform.rotation.eulerAngles;
            syncobj.getGameObject().transform.rotation = Quaternion.Euler(syncEuler.x, syncEuler.y + 90f, syncEuler.z);
        }
        else
        {
            var euler = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(euler.x, euler.y + 90f, euler.z);
        }
    }
	// Use this for initialization
	void Start () {
		
	}
    private void updatePosition()
    {
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100000, 1 << 9))
        {
            transform.position = hit.point.snap();
            transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
        }
    }
    // Update is called once per frame
    void Update () {
        updatePosition();

        if (Input.GetMouseButtonDown(0))
        {
            var deleted = handleDelete();
            if(!deleted)
                handlePlace();
        }
        if (Input.GetMouseButtonDown(1))
        {
            handleRotate();
        }
    }
}
