using UnityEngine;
using System.Collections;
using System.Linq;

public enum ModulationType{ModTiller, ModPlanter}



public interface iModule{
	void registerHost( iModuleHost gameObject);
	void execute();
	void postExecute();
	void executeRelease();
	void destroySelf();
	ModulationType getModType();
}

public class Module : MonoBehaviour, iModule{
	public iModuleHost host = null;
	public ModulationType modType = ModulationType.ModTiller;
	public virtual void execute(){}
	public virtual void postExecute (){}
	public virtual void executeRelease(){}
	public virtual void registerHost (iModuleHost hostobj){
		host = hostobj;
	}
	public void destroySelf(){
		Destroy (gameObject);
	}
	public ModulationType getModType(){
		return modType;
	}
	
}




public class Plant : Syncable {
}
public class Fungus: Syncable {
}
