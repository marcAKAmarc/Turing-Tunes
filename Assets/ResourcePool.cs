using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourcePool : MonoBehaviour {
    protected static int newId { get { return _id++; } }
    private static int _id;

    private Repositories repositories = new Repositories();

    [HideInInspector]
    public int id;
    public Transform resourcePrefab;
    public int Amount = -1;

    void Awake()
    {
        id = newId;
    }
    void OnDestroy()
    {
        repositories.getResourcePoolRepository().Remove(this);
    }
	void Start ()
    {
        repositories.getResourcePoolRepository().Add(this);
    }

    public Transform GetResource()
    {
        var resource = Transform.Instantiate(resourcePrefab, transform.position, transform.rotation) as Transform;
        return resource;
    }

    public Type GetResourceType()
    {
        return resourcePrefab.GetComponent<Resource>().GetType();
    }
}





