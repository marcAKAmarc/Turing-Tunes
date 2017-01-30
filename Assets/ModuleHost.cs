using UnityEngine;
using System.Collections;

public interface iModuleHost{
	GameObject getGameObject();
	void registerModule (iModule module);
	void resetModule();
 	ModulationType? getModuleType ();
}
public class ModuleHost: Syncable, iModuleHost{
	iModule Module = null;
//	public virtual override GameObject getGameObject(){
//		return base.getGameObject ();
//	}
	public virtual void registerModule(iModule module){
		Module = module; 
	}
	protected override void OnDestroy(){
		resetModule ();
		base.OnDestroy ();
	}
	public void resetModule(){
		if(Module != null)
			Module.destroySelf ();
		Module = null;
	}
	public virtual ModulationType? getModuleType(){
		if (Module == null)
			return null;
		else
			return Module.getModType();
	}

	public void executeModule(){
		if (Module != null)
			Module.execute ();
	}

	public void postExecuteModule(){
		if (Module != null)
			Module.postExecute ();
	}

	public void executeReleaseModule(){
		if (Module != null)
			Module.executeRelease ();
	}
}
