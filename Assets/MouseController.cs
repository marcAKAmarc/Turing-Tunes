using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour {
    public TuringScene scene;
    public List<Transform> placables;
    public List<Transform> displayables;
    private List<Transform> displayableInstances;
    public Camera cam;    
    private int selectionPosition;
	// Use this for initialization
    private Guid id;
    
    private float zoomCoefficient = 1;
    private int LayerField;

    private void OnEnable()
    {
        updateDisplayables();
    }
    private void OnDisable()
    {
        if (displayableInstances.Count > selectionPosition)
            displayableInstances[selectionPosition].gameObject.SetActive(false);
    }
    void Start(){
        LayerField = LayerMask.NameToLayer("Field");
        id = Guid.NewGuid();
        selectionPosition = 0;
        displayableInstances = createDisplayables();
        //Debug.Log("displayableInstances.count " + displayableInstances.Count.ToString() );
        updateDisplayables();
    }
    
    
	// Update is called once per frame
	void Update () {
	    
        updatePosition();
        
        if (Input.GetKeyDown("insert")){
            handleZoomInRequest();
        }
        if(Input.GetKeyDown("delete")){
            handleZoomOutRequest();
        }
        
        var scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f){
            handlePlacableScrollRequest(scroll);
        }
        
        updateDisplayables();
        
         if (Input.GetMouseButtonDown(0)){
                handleDeleteRequest();   
                handlePlaceRequest();
         }
        
         if (Input.GetMouseButtonDown(1))
            handleAlterRequest();
         
         if(Input.GetMouseButtonDown(2))
            handleAlter2Request();
         
         //handleCameraScroll();  

    }
    
    private List<Transform> createDisplayables(){
        List<Transform> returnList = new List<Transform>();
        foreach(Transform t in displayables){
           if(t!=null){     
                Transform inst = Instantiate(t,transform.position,Quaternion.identity) as Transform;
                inst.gameObject.SetActive(false);
                returnList.Add(inst);
           }
        }
        return returnList;
    }
    
    private void updateDisplayables(){
        for(var i = 0; i<displayableInstances.Count; i++){
            displayableInstances[i].position = transform.position;

            if (i==selectionPosition)
                displayableInstances[i].gameObject.SetActive(true);
            else
                displayableInstances[i].gameObject.SetActive(false);
        }
    }
    
    void updatePosition(){
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit,100000, 1<<9)) {
            transform.position = hit.point.snap();
            transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
        }
    }        
            
    void handlePlaceRequest(){
        if (isMouseOverUI())
            return;
        var syncobjs = scene.syncer.findSyncObjectsByPosition(transform.position.snap());

        //if there are objects in the way, don't place anything
        if (syncobjs.Where(x => x.preventPlacement).Count() > 0)
            return;

        Transform t = Instantiate(placables[selectionPosition], transform.position,Quaternion.identity) as Transform;
        //var syncables = t.GetComponents<iSyncable>();
        /*foreach(var syncable in syncables)
            syncable.setSceneObject(scene);*/
    }
    
    void handleAlterRequest(){
        var syncobjs = scene.syncer.findSyncObjectsByPosition(transform.position.snap());
        foreach(var syncobj in syncobjs){
            syncobj.alter();
        }
        
        // //make this genereic ala delete
        // var potentialPagoda = scene.Pagodas.Where(x=>x.transform.position.snap() == transform.position.snap()).FirstOrDefault();
        // 
        // if(potentialPagoda!=null){
        //     potentialPagoda.transform.rotation = Quaternion.LookRotation(potentialPagoda.transform.right, potentialPagoda.transform.up);
        // }
        // 
        // var potentialSpawner = scene.Spawners.Where(x=>x.transform.position.snap() == transform.position.snap()).FirstOrDefault();
        // if(potentialSpawner!=null){
        //     potentialSpawner.transform.rotation = Quaternion.LookRotation(potentialSpawner.transform.right, potentialSpawner.transform.up);
        // }
       
    }
    
    void handleAlter2Request(){
         var syncobjs = scene.syncer.findSyncObjectsByPosition(transform.position.snap());
        foreach(var syncobj in syncobjs){
            syncobj.alter2();
        }
    }
    
    bool handleDeleteRequest(){
        var syncobjs = scene.syncer.findSyncObjectsByPosition(transform.position.snap()).Where(x=>x.deletable).ToList();
        bool deleted = syncobjs.Count > 0;
        foreach(var syncobj in syncobjs){
            //scene.removeSyncObject(syncobj);
            //Destroy(syncobj.getGameObject());
            if(syncobj.deletable)
			    syncobj.Delete();
        }
        return deleted;   
    }
    
    void handlePlacableScrollRequest(float scroll){
        if(scroll>0f){
            selectionPosition+=1;
        }
        if(scroll<0f){
            selectionPosition-=1;
        }
        if (selectionPosition < 0)
            selectionPosition += placables.Count;
        selectionPosition = selectionPosition % placables.Count;
    }
    
    void handleCameraScroll(){
        if (isMouseOffScreen() || isMouseOverUI())
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
        if (y<.1f)
            vtravel = cambackzerod;
        if (y>.75f)
            vtravel = camforwardzerod;
            
        cam.transform.position += (htravel+vtravel).normalized * Time.deltaTime * 8.0f * zoomCoefficient;
            
    }
    
    void handleZoomOutRequest(){
        zoomCoefficient = zoomCoefficient*2;
        // cam.transform.position += -(cam.transform.forward)*zoomCoefficient;
        cam.orthographicSize = cam.orthographicSize + zoomCoefficient;
    }
    
    void handleZoomInRequest(){
        cam.orthographicSize = cam.orthographicSize - zoomCoefficient;
        //cam.transform.position += cam.transform.forward*zoomCoefficient;
        zoomCoefficient = zoomCoefficient/2.0f;
    }
        
    private iSyncable getISyncableAtPosition(){
        iSyncable foundObj = null;
        foundObj = scene.Pagodas.Where(x=>x.transform.position.snap() == transform.position.snap()).FirstOrDefault();
        return foundObj;
    }   


    private bool isMouseOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    private bool isMouseOffScreen()
    {
        //Debug.Log(Input.mousePosition);
#if UNITY_EDITOR
        if (Input.mousePosition.x <= 0 || Input.mousePosition.y <= 0 || Input.mousePosition.x >= Screen.width || Input.mousePosition.y >= Screen.height)
            return true;
        else
            return false;
#else
        if (Input.mousePosition.x == 0 || Input.mousePosition.y == 0 || Input.mousePosition.x >= Screen.width - 1 || Input.mousePosition.y >= Screen.height - 1)
            return true;
        else
            return false;
#endif
    }

}
