using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class TuringScene:MonoBehaviour {
    [HideInInspector]
    public Syncer syncer;
	public Arena arena;
    public float syncerInterval;
    public List<PagodaController> Pagodas;
    public List<Runner> Runners;
    public List<Spawner> Spawners;
	public List<SolidController> Solids;
    public List<iSyncable> syncObjects;
    
    void Awake(){
		arena = new Arena(new Vector3(6,6,6));
        Runners = new List<Runner>();
        Pagodas = new List<PagodaController>(); 
        syncObjects = new List<iSyncable>();
		Solids = new List<SolidController> ();
        syncer = new Syncer(this, syncerInterval, syncObjects);    
    }
    
    void Start(){
        startTimer();
    }
    void startTimer(){
        StartCoroutine(Timer());  
    }
    IEnumerator Timer() {
        while (true){
            yield return new WaitForSeconds(Syncer.Interval);
            syncer.updateSyncObjects();
            //GetComponent<AudioSource>().Play();
        }
    }
    
    public void removeSyncObject(iSyncable sync){
        //this is shitty nested so that we can check the least expensive first...
        var deleteSpawner = Spawners.Where(x=>x.getSyncId() == sync.getSyncId()).FirstOrDefault();
        if(deleteSpawner != null){
            Spawners.Remove(deleteSpawner);
            
        }else{
            var deleteRunner = Runners.Where(x=>x.getSyncId() == sync.getSyncId()).FirstOrDefault();
            if(deleteRunner!= null){
                Runners.Remove(deleteRunner);
                
            }else{
                var deletePagoda = Pagodas.Where(x=>x.getSyncId() == sync.getSyncId()).FirstOrDefault();
                if(deletePagoda!= null){
                    Pagodas.Remove(deletePagoda);
                   
                }else{
					var deleteSolid = Solids.Where(x=>x.getSyncId() == sync.getSyncId()).FirstOrDefault();
					if(deleteSolid!= null){
						Solids.Remove(deleteSolid);

					}
				}
			}
		}

		syncer.removeSyncObject(sync);
	}
}
