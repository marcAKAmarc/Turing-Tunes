using UnityEngine;
using System.Collections;
using System.Linq;
using System;

public interface iModulator{
	void setModulatorType(ModulationType type);
	ModulationType getModulatorType();
}
public class ModulatorController:Syncable,iModulator{
	[SerializeField]
	private ModulationType modType = ModulationType.ModTiller;
	public Transform TillerPrefab;
	public Transform PlanterPrefab;
	public void setModulatortype(ModulationType type){
		modType = type;
	}
	public ModulationType getModulatorType(){
		return modType;
	}

	public void setModulatorType(ModulationType type){
		modType = type;
	}

	public void cycleModulatorType(){
		var type = getModulatorType ();
		int count = Enum.GetNames(typeof(ModulationType)).Length;
		var newtype = ((int)type + 1) % count;
		setModulatortype ((ModulationType)newtype);
	}
	
	public ModulationType? getHostModulatorType(iModuleHost modhost){
		return modhost.getModuleType ();
	}
	
	public override void  onPostSync(){
		iModuleHost host = (iModuleHost)scene.syncObjects.Where (x => x.getGameObject ().GetComponent<iModuleHost> () != null 
		                                             && x.getGameObject ().transform.position.snap () == transform.position.snap ()).FirstOrDefault();
		if (host == null)
			return;
		iModuleHost nonNullHost = (iModuleHost)host;
		ModulationType? hostmodtype = nonNullHost.getModuleType (); 
		if (hostmodtype != modType) {
			removeCurrentMod(nonNullHost);
			giveMod(nonNullHost);
		}
		
	}
	private void removeCurrentMod(iModuleHost host){
		host.resetModule ();
	}
	private void giveMod(iModuleHost host){
		var mod = createMod ();
		host.registerModule (mod);
		mod.registerHost (host);
	}
	
	private iModule createMod(){
		iModule newmod = null;
		switch (modType) { 
		case ModulationType.ModTiller:
			newmod = (Instantiate (TillerPrefab, transform.position.snap (), Quaternion.identity) as Transform).GetComponent<iModule>();
			break;
		case ModulationType.ModPlanter:
			newmod = (Instantiate (PlanterPrefab, transform.position.snap (), Quaternion.identity) as Transform).GetComponent<iModule>();
			break;
		default:
			newmod = null;
			break;
		}
		return newmod;
	}

	public override void alter2 ()
	{
		cycleModulatorType ();
	}
	
}