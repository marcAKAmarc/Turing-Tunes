using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bank : MonoBehaviour
{
    protected static int newId { get { return _id++; } }
    private static int _id;

    private Repositories repositories = new Repositories();

    [HideInInspector]
    public int id;
    public MonoBehaviour accepts;
    public int Amount = -1;

    private List<Transform> pullingResources = new List<Transform>();


    void Awake()
    {
        id = newId;
    }
    void OnDestroy()
    {
        repositories.getBankRepository().Remove(this);
    }
    void Start()
    {
        repositories.getBankRepository().Add(this);
    }

    public bool DepositResource(Transform resourceTransform)
    {
        if (resourceTransform.GetComponent(accepts.GetType()) == null)
            return false;

        pullingResources.Add(resourceTransform);
        Amount += 1;
        return true;
    }

    void Update()
    {
        foreach(var resource in pullingResources)
        {
            var relativeGoal = (transform.position - resource.transform.position)/2f;
            resource.transform.position += relativeGoal;
        }
    }
}
