using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;

public static class PlacementLogic
{
    public static Transform handlePlaceRequest(Transform placeObject, Vector3 position)
    {
        if (isMouseOverUI())
            return null;
        var syncobjs = TuringScene.CurrentController.syncer.findSyncObjectsByPosition(position.snap());

        //if there are objects in the way, don't place anything
        if (syncobjs.Where(x => x.preventPlacement || placeObject.GetComponents<Syncable>().Any(c=>c.GetType().FullName == x.GetType().FullName)).Count() > 0)
            return null;

        Transform t = Transform.Instantiate(placeObject, position, placeObject.rotation) as Transform;
        return t;
    }

    public static bool handleDeleteRequest(Vector3 deletePosition)
    {
        var syncobjs = TuringScene.CurrentController.syncer.findSyncObjectsByPosition(deletePosition.snap()).Where(x => x.deletable).ToList();
        bool deleted = syncobjs.Count > 0;
        foreach (var syncobj in syncobjs)
        {
            //scene.removeSyncObject(syncobj);
            //Destroy(syncobj.getGameObject());
            if (syncobj.deletable)
                syncobj.Delete();
        }
        return deleted;
    }

    public static iSyncable handlePickUpRequest(Vector3 pickPosition)
    {
        if (isMouseOverUI())
            return null;

        var syncobj = TuringScene.CurrentController.syncer.findSyncObjectsBySnappedPosition(pickPosition.snap()).Where(x => x.pickable).FirstOrDefault();
        
        if (syncobj != null)
        {
            syncobj.getGameObject().SetActive(false);
        }
               
        return syncobj;
    }

   /*public static bool handlePutDownRequest(iSyncable obj, Vector3 position)
    {
        if (isMouseOverUI())
            return false;
        var syncobjs = TuringScene.CurrentController.syncer.findSyncObjectsByPosition(position.snap());

        //if there are objects in the way, don't place anything
        if (syncobjs.Where(x => x.preventPlacement || x.GetType().FullName == obj.GetType().FullName ).Count() > 0)
            return false;

        obj.getGameObject().transform.position = position;
        obj.getGameObject().SetActive(true);

        return true;
    }*/



    public static bool isMouseOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    public static bool isMouseOffScreen()
    {
        if (Input.mousePosition.x <= 0 || Input.mousePosition.y <= 0 || Input.mousePosition.x >= Screen.width || Input.mousePosition.y >= Screen.height)
            return true;
        else
            return false;
    }


}

public class PickerController : MonoBehaviour {
    private enum PickerState { empty, full};
    private PickerState state = PickerState.empty;
    public Transform PickerModel;
    public Camera cam;
    private TuringScene scene;
    private Transform pickedModel;
    private Transform pickedModelDisplay;
    private Transform picked;
	// Use this for initialization
	void Start () {
        scene = TuringScene.CurrentController;
	}
    void OnEnable()
    {
        PickerModel.gameObject.SetActive(true);
    }
    private void OnDisable()
    {
        PickerModel.gameObject.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        updatePosition();
        if (Input.GetMouseButtonDown(0))
        {
            handlePickRequest();
        }
        if (Input.GetMouseButtonDown(1))
        {
            handlePickerRotateRequest();
        }
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

    private void handlePickRequest()
    {
        if (state == PickerState.empty)
        {
            var pickedup = PlacementLogic.handlePickUpRequest(transform.position);
            if (pickedup != null)
            {
                state = PickerState.full;
                picked = Transform.Instantiate(pickedup.getGameObject().transform, new Vector3(-1000, -1000, -1000), pickedup.getGameObject().transform.rotation, transform);
                if(pickedup.model != null)
                {
                    pickedModel = Transform.Instantiate<Transform>(pickedup.model, transform);
                    pickedModel.gameObject.SetActive(true);
                }
                Destroy(pickedup.getGameObject());
                
            }
        }
        else if (state == PickerState.full)
        {
            var placed = PlacementLogic.handlePlaceRequest(picked, transform.position);
            if (placed)
            {
                state = PickerState.empty;
                picked.GetComponent<Syncable>().Delete();
                placed.gameObject.SetActive(true);
                if (pickedModel != null)
                {
                    Destroy(pickedModel.gameObject);
                }
                transform.rotation = Quaternion.identity;
            }
        }
    }

    private void handlePickerRotateRequest()
    {
        if (state == PickerState.full) {
            var euler = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(euler.x, euler.y + 90f, euler.z);
        }
    }

}
