using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bank : Syncable
{
    protected static int newId { get { return _id++; } }
    private static int _id;

    private Repositories repositories = new Repositories();

    [HideInInspector]
    public int id;
    public MonoBehaviour accepts;
    public int Amount = -1;
    public float DeplositFrequency = 0;
    private int StoredAmount = 0;
    private List<Transform> pullingResources = new List<Transform>();
    private int beatsSinceLastDeposit = 9999;
    private List<int> DepositBeatsLedger;
    private int DepositBeatsLedgerLength = 20;
    private int FrequencyHistoryCheck = 20;
    public bool PreventPlacement;
    


    protected override void Awake()
    {
        base.Awake();
        id = newId;
        base.preventPlacement = PreventPlacement;
    }

    protected override void  OnDestroy()
    {
        repositories.getBankRepository().Remove(this);
        base.OnDestroy();
    }
    protected override void Start()
    {
        repositories.getBankRepository().Add(this);
        base.Start();
        DepositBeatsLedger = new List<int>();
    }

    public bool DepositResource(Transform resourceTransform)
    {
        if (resourceTransform.GetComponent(accepts.GetType()) == null)
            return false;

        pullingResources.Add(resourceTransform);
        Amount += 1;

        try
        {
            DepositBeatsLedger[DepositBeatsLedger.Count - 1] += 1;
        }
        catch(Exception ex)
        {
            //wtf
            var stuff = 18;
        }

        DeplositFrequency = DepositBeatsLedger.Sum(x => x) / ((float)DepositBeatsLedgerLength);

        return true;
    }

    public override void onSync()
    {
        base.onSync();
        DepositBeatsLedger.Add(0);
        if (DepositBeatsLedger.Count > DepositBeatsLedgerLength)
            DepositBeatsLedger.RemoveAt(0);
    }

    void Update()
    {
        var deletables = new List<Transform>();
        foreach(var resource in pullingResources)
        {
            var relativeGoal = (transform.position - resource.transform.position)/2f;
            resource.transform.position += relativeGoal;
            if((resource.transform.position - transform.position).magnitude < .001f)
            {
                //do recieved animation trigger here;
                deletables.Add(resource.transform);
            }
        }

        for(var i = deletables.Count()-1; i >= 0; i--)
        {
            pullingResources.RemoveAt(0);
            Destroy(deletables[i].gameObject);
        }         
    }

    public int GetStoredAmount()
    {
        return Amount;
    }
}
