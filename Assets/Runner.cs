using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;







public class Runner : /*Syncable,*/ /*iModuleHost*/ ModuleHost {

//	iModule Module = null;
//	public override GameObject getGameObject(){
//		return base.getGameObject ();
//	}
//	public virtual void registerModule(iModule module){
//		Module = module; 
//	}
//	public void resetModule(){
//		Module.destroySelf ();
//		Module = null;
//	}
//	public virtual ModulationType? getModuleType(){
//		if (Module == null)
//			return null;
//		else
//			return Module.getModType();
//	}

	private float speed;
    private Vector3 goalPosition;
    private Vector3 previousPosition;
    private int FightThreshold = 2000;
    private Guid? BlockedBy;
    private int TicksBlocked=0;
    private int TicksBlockedByBlockedBy=0; 

    
    // Use this for initialization
	private void updateSpeed () {
        speed = 1.0f / Syncer.Interval;//base.getScene().syncer.Interval;
	}
    
    protected override void Start(){
		base.Start ();
		registerToScene();
        goalPosition = transform.position;
        previousPosition = transform.position; 
    }
    

    
    public override void onSync(){
        updateSpeed();
        transform.position = goalPosition.snap();
        handlePagodaRotation();
        goalPosition = (transform.position+transform.forward).snap();
        updateReactors();

		executeModule ();
    }
    
    public override void onPostSync(){
        //check positions
        if(!checkPositionFree(goalPosition)){
            goalPosition =transform.position.snap();
        }
        previousPosition = transform.position.snap();
    
		postExecuteModule ();
	}
    
    public override void onSyncRelease(){
		if(goalPosition.snap () == transform.position.snap ())
			TicksBlocked += 1;
		else
			TicksBlocked = 0;

        if(TicksBlockedByBlockedBy>FightThreshold){
            var other = base.getScene().syncer.findSyncObjectsById((Guid)BlockedBy);
            if (other != null)
				other.Delete ();
				//Destroy(other.getGameObject());
        }
        
        if(Mathf.Abs(transform.position.x) > 5.0f || Mathf.Abs(transform.position.z) > 5.0f){
			Delete ();
			//Destroy(gameObject);
        }

		base.onSyncRelease();

		executeReleaseModule ();
    }
    


    public void registerToScene(){
        base.getScene().Runners.Add(this);
    }
    


    public void handlePagodaRotation(){
        //check for state here to make sure we aren't blocked!
		PagodaController pc = base.getScene().Pagodas.Where(x=>x.transform.position == transform.position).FirstOrDefault();
        if(pc!=null){
            transform.rotation = pc.transform.rotation;
        }
    }

    public void updateReactors()
    {
        var q_fieldreacotors = base.getScene().syncObjects.Where(x => x is iFieldReactor);
        var q_on = q_fieldreacotors.Where(x => x.getGameObject().transform.position == transform.position);
        var q_stayingOn = transform.position == goalPosition ? q_on : new List<iSyncable>().Where(x=>false);
        var q_leavingOn = transform.position != goalPosition ? q_on : new List<iSyncable>().Where(x => false);
        var q_approachingOn = q_fieldreacotors.Where(x => x.getGameObject().transform.position == goalPosition && goalPosition != transform.position);
        var q_approachingSide = q_fieldreacotors
            .Where(x => x.getGameObject().transform.position != transform.position) //not in the position you are on
            .Where(x => x.getGameObject().transform.position != transform.position + (2.0f * (goalPosition - transform.position))) //not in the position infront of your goal position
            .Where(x => Mathf.Abs(x.getGameObject().transform.position.x - goalPosition.x) < 1.5f) //within 1.5f x of goalPos
            .Where(x => Mathf.Abs(x.getGameObject().transform.position.z - goalPosition.z) < 1.5f) //within 1.5f y of goalPos
            .Where(x => x.getGameObject().transform.position.x == goalPosition.x || x.getGameObject().transform.position.z == goalPosition.z); //disqualify diagnols
        var q_side = q_fieldreacotors
            .Where(x => x.getGameObject().transform.position != transform.position) //not in the position you are on
            .Where(x => x.getGameObject().transform.position != goalPosition) //not in the goal position
            .Where(x => Mathf.Abs(x.getGameObject().transform.position.x - transform.position.x) < 1.5f) //within 1.5f x of goalPos
            .Where(x => Mathf.Abs(x.getGameObject().transform.position.z - transform.position.z) < 1.5f) //within 1.5f y of goalPos
            .Where(x => x.getGameObject().transform.position.x == transform.position.x || x.getGameObject().transform.position.z == transform.position.z); //disqualify diagnols
        var q_stayingSide = transform.position == goalPosition ? q_side : new List<iSyncable>().Where(x => false);
        var q_leavingSide = transform.position != goalPosition ? q_side : new List<iSyncable>().Where(x => false);

        foreach (var r in q_stayingOn)
        {
            var fr = r.getGameObject().GetComponent<iFieldReactor>();
            fr.StayingOn();
        }
        foreach (var r in q_leavingOn)
        {
            var fr = r.getGameObject().GetComponent<iFieldReactor>();
            fr.LeavingOn();
        }
        foreach (var r in q_approachingOn)
        {
            var fr = r.getGameObject().GetComponent<iFieldReactor>();
            fr.ApproachingOn();
        }

        foreach (var r in q_approachingSide)
        {
            var fr = r.getGameObject().GetComponent<iFieldReactor>();
            fr.ApproachingSide();
        }
        foreach (var r in q_stayingSide)
        {
            var fr = r.getGameObject().GetComponent<iFieldReactor>();
            fr.StayingSide();
        }
        foreach (var r in q_leavingSide)
        {
            var fr = r.getGameObject().GetComponent<iFieldReactor>();
            fr.LeavingSide();
        }
    }
    


	void Update () { 
	   transform.position += (goalPosition-previousPosition)*speed*Time.deltaTime;
	}
    
    private bool checkPositionFree(Vector3 pos){
		return checkObstructionFree(pos) && checkRunnerTrafficFree(pos);   
	}
	private bool checkObstructionFree(Vector3 pos){
		var solid = getScene ().Solids.Where (x => x.transform.position.snap () == pos.snap ()).FirstOrDefault ();
		return (solid == null);
	}
	private bool checkRunnerTrafficFree(Vector3 pos){
		bool free = true;
        bool contested = false;
        bool contestingWinner = false;
        Runner blockingRunner = null;
        List<Runner> contestingRunners = new List<Runner>();
        foreach(var runner in base.getScene().Runners){
            if(runner.getSyncId() == id)
                continue;
            if(runner.getCurrentPosition() == pos){
                free= false;
                blockingRunner = runner;              
                break;
            }else if(runner.getGoalPosition() == pos){
                contested = true;
                contestingRunners.Add(runner);
            }
        }

		//determine if contesting winner
        if(contested && free){
            int? opposingTicksBlocked = contestingRunners.Select(x=>x.TicksBlocked).OrderByDescending(x=>x).FirstOrDefault();
			List<Runner> opposers = contestingRunners.Where (x=>x.TicksBlocked == opposingTicksBlocked).ToList ();
            if(TicksBlocked > opposingTicksBlocked)
			//if((Guid?)id == contestingRunners.Where(x=>x.TicksBlocked == MaxTicksBlocked).OrderBy(x=>x.TimeCreated).Select(x=>x.id).FirstOrDefault()){
                contestingWinner = true;
			else if (TicksBlocked == opposingTicksBlocked){ //go by birth date
				DateTime timecreated = opposers.Select (x=>x.TimeCreated).OrderBy(x=>x).FirstOrDefault();
				opposers = opposers.Where (x=>x.TimeCreated == timecreated).ToList ();
				if(timecreated > TimeCreated)
					contestingWinner = true;
				else if (timecreated == TimeCreated){ //idunno just go by alphabetical getSyncId???
					var winnerId =  opposers.OrderBy(x=>x.getSyncId().ToString()).FirstOrDefault().getSyncId().ToString();
					List<string> list = new List<string>();
					list.Add(getSyncId().ToString());
					list.Add(winnerId);
					list.Sort(StringComparer.CurrentCulture);
					if(id.ToString() == list[0] )
						contestingWinner = true;
				}
            }
        }

		//update ticksblocked by blocked by
		if (!free) {
			if (BlockedBy == blockingRunner.getSyncId ()) {
				TicksBlockedByBlockedBy += 1;
			} else {
				TicksBlockedByBlockedBy = 1;
			}
		} else
			TicksBlockedByBlockedBy = 0;

		//update blocked by
		if (!free)
			BlockedBy = blockingRunner.getSyncId();
		else
			BlockedBy = null;

        return free && (!contested || (contested && contestingWinner));            
    }
    
    public Vector3 getGoalPosition(){
        return goalPosition;
    }
    
    public Vector3 getPreviousPosition(){
        return previousPosition;
    }
    
    public Vector3 getCurrentPosition(){
        return transform.position.snap();
    }
    
    private void registerBlockedByRunner(Runner r){
        if (BlockedBy != r.getSyncId()){
            TicksBlocked = 1;
            BlockedBy = r.getSyncId();
        }
        else
            TicksBlocked +=1;
    }
}
