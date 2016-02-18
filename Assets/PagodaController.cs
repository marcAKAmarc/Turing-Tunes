using UnityEngine;
using System.Collections;
using System;
using System.Linq;

public class PagodaController : MonoBehaviour, iSyncable {
    
    Guid? OwnerId = null;
    [SerializeField]
    private Material turnableMaterial;
    [SerializeField]
    private Material initialMaterial;
    private bool turnable = false;
    public TuringScene scene;
    private Guid id;    
    void Awake(){
        id = Guid.NewGuid();
    }
	void Start () {
       registerAsSyncObject();
	   registerToScene();
	}
    
    
    
    public void onSync(){
        
    }
    public void onPostSync(){
        //check positions;
    }
    
    public void onSyncRelease(){
        handlePagodaRotation();
    }
    
    public Guid getSyncId(){
        return id;
    }
    
    public void registerToScene(){
        scene.Pagodas.Add(this);
    }
    
    public void registerAsSyncObject(){
        if (id==Guid.Empty)
            id = Guid.NewGuid();
        scene.syncer.addSyncObject((iSyncable)this);
    }
 
    private void handlePagodaRotation(){
        if (!turnable)
            return;
        //Debug.Log("In rotation check");
        Vector3 left = transform.position - transform.right;
        Vector3 right = transform.position + transform.right;
        Vector3 forward = transform.position + transform.forward;
        Vector3 back = transform.position - transform.forward;
        //Debug.Log(left.ToString() + " " + right.ToString() + " " + forward.ToString() + " " + back.ToString() );
        foreach(Runner r in scene.Runners){
            //Debug.Log(r.transform.position.ToString());
        }
        var positionedRunners = scene.Runners.Where(x=>
            (x.transform.position.snap() == left.snap()  && (Quaternion.Angle(x.transform.rotation , Quaternion.LookRotation(-transform.forward,transform.up)) < 1f||Quaternion.Angle(x.transform.rotation, Quaternion.LookRotation(transform.forward, transform.up))<1f ))||  
            (x.transform.position.snap() == right.snap() && (Quaternion.Angle(x.transform.rotation , Quaternion.LookRotation(-transform.forward,transform.up)) < 1f||Quaternion.Angle(x.transform.rotation , Quaternion.LookRotation(transform.forward, transform.up))<1f))||
            (x.transform.position.snap() == forward.snap() && (Quaternion.Angle(x.transform.rotation , Quaternion.LookRotation(-transform.right,transform.up)) <1f|| Quaternion.Angle(x.transform.rotation , Quaternion.LookRotation(transform.right, transform.up))<1f))||
            (x.transform.position.snap() == back.snap() &&(Quaternion.Angle(x.transform.rotation , Quaternion.LookRotation(-transform.right,transform.up)) < 1f || Quaternion.Angle(x.transform.rotation , Quaternion.LookRotation(transform.right, transform.up))<1f))
        ).ToList();
        //Debug.Log(positionedRunners.Count);
        
        var positionedPagodas = scene.Pagodas.Where(x=>
            (x.transform.position.snap() == left.snap()  && (Quaternion.Angle(x.transform.rotation , Quaternion.LookRotation(-transform.forward,transform.up)) < 1f||Quaternion.Angle(x.transform.rotation, Quaternion.LookRotation(transform.forward, transform.up))<1f ))||  
            (x.transform.position.snap() == right.snap() && (Quaternion.Angle(x.transform.rotation , Quaternion.LookRotation(-transform.forward,transform.up)) < 1f||Quaternion.Angle(x.transform.rotation , Quaternion.LookRotation(transform.forward, transform.up))<1f))||
            (x.transform.position.snap() == forward.snap() && (Quaternion.Angle(x.transform.rotation , Quaternion.LookRotation(-transform.right,transform.up)) <1f|| Quaternion.Angle(x.transform.rotation , Quaternion.LookRotation(transform.right, transform.up))<1f))||
            (x.transform.position.snap() == back.snap() &&(Quaternion.Angle(x.transform.rotation , Quaternion.LookRotation(-transform.right,transform.up)) < 1f || Quaternion.Angle(x.transform.rotation , Quaternion.LookRotation(transform.right, transform.up))<1f))
        ).ToList();
        
        var rotateCounterClockwise = 0;
        foreach(Runner runner in positionedRunners){
            bool inPagoda = scene.Pagodas.Where(p=>p.transform.position.snap() == runner.transform.position.snap()).ToList().Count > 0;
            if(inPagoda)
                break;
            //Debug.Log("checking runner...");
            Vector3 cross = Vector3.Cross(runner.transform.position.snap() - transform.position.snap(), transform.up).snap();
            //Debug.Log("Cross: " + cross.ToString() + "  transform.forward: " + runner.transform.forward.ToString());
            if(cross==runner.transform.forward.snap() /*&& runner.goalposition != position*/)
                rotateCounterClockwise +=1;
            else
                rotateCounterClockwise -=1;
            //Debug.Log(rotateCounterClockwise.ToString());
        }
        
        if (rotateCounterClockwise > 0){
            transform.rotation = Quaternion.LookRotation(-transform.right.snap(), transform.up.snap());
        }
        if (rotateCounterClockwise < 0){
            transform.rotation = Quaternion.LookRotation(transform.right.snap(), transform.up.snap());
        }
    }
    
    public void setSceneObject(TuringScene s){
        scene = s;
    }
    
	// Update is called once per frame
	void Update () {
	   
	}
    
    public void alter(){
        transform.rotation = Quaternion.LookRotation(transform.right, transform.up);
    }
    
    public void alter2(){
        turnable = !turnable;
        setTurnableMaterial(turnable);
    }
    
    private void setTurnableMaterial(bool turnable){
        if(turnable){
            var mrs = transform.GetComponentsInChildren<MeshRenderer>();
            foreach(var mr in mrs)
                mr.material = turnableMaterial;
        }else{
            var mrs = transform.GetComponentsInChildren<MeshRenderer>();
            foreach(var mr in mrs)
                mr.material = initialMaterial;
        }
    }
    
    public GameObject getGameObject(){
        return gameObject;
    }
    
    public void setOwner(Guid? id){
        OwnerId = id;
    }
    public Guid? getOwner(){
        return OwnerId;
    }
}
