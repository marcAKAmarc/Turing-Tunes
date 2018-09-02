using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class Carrier : Syncable {

    Transform carriedResource = null;
    public Transform carryTransform;
    public bool Deletable;
    public bool PreventPlacement;
    // Use this for initialization

    protected override void Awake()
    {
        base.Awake();
        base.deletable = Deletable;
        base.preventPlacement = PreventPlacement;
    }

    protected override void Start()
    {
        base.Start();
        //registerToScene();
    }

    public override void onPostSync()
    {
        tryPlaceResource();
        tryGetResource();
    }

    private void tryGetResource()
    {
        if (carriedResource != null)
            return;

        var resourcePool = (new Repositories()).getResourcePoolRepository().GetAll().Where(x => x.transform.position.x == transform.position.x && x.transform.position.z == transform.position.z).FirstOrDefault();

        if (resourcePool == null)
            return;

        var item = resourcePool.GetResource();

        carriedResource = item;
    }

    private void tryPlaceResource()
    {
        if (carriedResource == null)
            return;
        var resourceBank = (new Repositories()).getBankRepository().GetAll().Where(x => x.transform.position == transform.position).FirstOrDefault();

        if (resourceBank == null)
            return;

        var deposited = resourceBank.DepositResource(carriedResource);

        if (deposited)
            carriedResource = null;
    }

    void Update()
    {
        if (carriedResource != null)
        {
            var goal = carryTransform.position - carriedResource.transform.position;
            carriedResource.transform.position += (goal / 2.0f); 
        }
    }

   protected override void OnDestroy()
    {
        if (carriedResource == null)
        {
            base.OnDestroy();
            return;
        }

        Destroy(carriedResource.gameObject);
        carriedResource = null;
        base.OnDestroy();
    }
}
