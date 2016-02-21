using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class Runner : Syncable {

	private float speed;
    private Vector3 goalPosition;
    private Vector3 previousPosition;
    private int FightThreshold = 2000;
    private Guid? BlockedBy;
    private int TicksBlocked=0;
    private int TicksBlockedByBlockedBy=0; 

    
    // Use this for initialization
	private void updateSpeed () {
	   speed = 1.0f/base.getScene().syncer.Interval;
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
    }
    
    public override void onPostSync(){
        //check positions
        if(!checkPositionFree(goalPosition)){
            goalPosition =transform.position.snap();
        }
        previousPosition = transform.position.snap();
    }
    
    public override void onSyncRelease(){
		if(goalPosition.snap () == transform.position.snap ())
			TicksBlocked += 1;
		else
			TicksBlocked = 0;

        if(TicksBlockedByBlockedBy>FightThreshold){
            var other = base.getScene().syncer.findSyncObjectsById((Guid)BlockedBy);
            if (other != null)
                Destroy(other.getGameObject());
        }
        
        if(Mathf.Abs(transform.position.x) > 5.0f || Mathf.Abs(transform.position.z) > 5.0f){
            Destroy(gameObject);
        }

		base.onSyncRelease();
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
    


	void Update () { 
	   transform.position += (goalPosition-previousPosition)*speed*Time.deltaTime;
	}
    
    private bool checkPositionFree(Vector3 pos){
        bool free = true;
        bool contested = false;
        bool contestingWinner = false;
        bool blockedLongest = false;
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
    
    public override GameObject getGameObject(){
        return gameObject;
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
