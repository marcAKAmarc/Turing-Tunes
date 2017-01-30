using UnityEngine;
using System.Collections;
using System;
using System.Linq;

public class PagodaController : Syncable {
    
    //Guid? OwnerId = null;
    [SerializeField]
    private Material turnableMaterial;
    [SerializeField]
    private Material initialMaterial;
    private bool turnable = false;

	protected override void Start () {
		base.Start ();
	   registerToScene();
	}
    
    public override  void onSyncRelease(){
        handlePagodaRotation();
    }
    
    public void registerToScene(){
        scene.Pagodas.Add(this);
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
        
        var positionedRunners = base.getScene().Runners.Where(x=>
            (x.transform.position.snap() == left.snap()  && (Quaternion.Angle(x.transform.rotation , Quaternion.LookRotation(-transform.forward,transform.up)) < 1f||Quaternion.Angle(x.transform.rotation, Quaternion.LookRotation(transform.forward, transform.up))<1f ))||  
            (x.transform.position.snap() == right.snap() && (Quaternion.Angle(x.transform.rotation , Quaternion.LookRotation(-transform.forward,transform.up)) < 1f||Quaternion.Angle(x.transform.rotation , Quaternion.LookRotation(transform.forward, transform.up))<1f))||
            (x.transform.position.snap() == forward.snap() && (Quaternion.Angle(x.transform.rotation , Quaternion.LookRotation(-transform.right,transform.up)) <1f|| Quaternion.Angle(x.transform.rotation , Quaternion.LookRotation(transform.right, transform.up))<1f))||
            (x.transform.position.snap() == back.snap() &&(Quaternion.Angle(x.transform.rotation , Quaternion.LookRotation(-transform.right,transform.up)) < 1f || Quaternion.Angle(x.transform.rotation , Quaternion.LookRotation(transform.right, transform.up))<1f))
        ).ToList();
        //Debug.Log(positionedRunners.Count);
        
//        var positionedPagodas = base.getScene().Pagodas.Where(x=>
//            (x.transform.position.snap() == left.snap()  && (Quaternion.Angle(x.transform.rotation , Quaternion.LookRotation(-transform.forward,transform.up)) < 1f||Quaternion.Angle(x.transform.rotation, Quaternion.LookRotation(transform.forward, transform.up))<1f ))||  
//            (x.transform.position.snap() == right.snap() && (Quaternion.Angle(x.transform.rotation , Quaternion.LookRotation(-transform.forward,transform.up)) < 1f||Quaternion.Angle(x.transform.rotation , Quaternion.LookRotation(transform.forward, transform.up))<1f))||
//            (x.transform.position.snap() == forward.snap() && (Quaternion.Angle(x.transform.rotation , Quaternion.LookRotation(-transform.right,transform.up)) <1f|| Quaternion.Angle(x.transform.rotation , Quaternion.LookRotation(transform.right, transform.up))<1f))||
//            (x.transform.position.snap() == back.snap() &&(Quaternion.Angle(x.transform.rotation , Quaternion.LookRotation(-transform.right,transform.up)) < 1f || Quaternion.Angle(x.transform.rotation , Quaternion.LookRotation(transform.right, transform.up))<1f))
//        ).ToList();
        
        var rotateCounterClockwise = 0;
        foreach(Runner runner in positionedRunners.Where (x=>x.getGoalPosition().snap() != x.transform.position.snap ())){
            bool inPagoda = base.getScene().Pagodas.Where(p=>p.transform.position.snap() == runner.transform.position.snap()).ToList().Count > 0;
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
    
    
    public override void alter2(){
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
}
