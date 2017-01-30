using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class Cubify : MonoBehaviour {

    public List<Transform> Models;
    public List<Transform> SkeletonElements;
    public float intensity = .25f;
    private float seed;
    protected void Awake()
    {
        foreach (var Model in Models)
        {
            seed = 0.0f; //must get seed from place common to rock models..  Mathf.PI * 2.0f * UnityEngine.Random.value;

            Mesh origMesh = null;//Model.GetComponent<MeshFilter>().mesh;
            if (origMesh == null)
                origMesh = Model.GetComponent<SkinnedMeshRenderer>().sharedMesh;

            var newMesh = (Mesh)Instantiate(origMesh);

            Vector3[] vertices = newMesh.vertices;
            int i = 0;
            while (i < vertices.Length)
            {
                vertices[i] += (cubify(vertices[i], true)-vertices[i])*intensity;
                newMesh.vertices = vertices;
                i += 1;
            }

            Model.GetComponent<SkinnedMeshRenderer>().sharedMesh = newMesh;
        }

        foreach (var t in SkeletonElements)
        {
            seed = 0.0f; //must get seed from place common to rock models..  Mathf.PI * 2.0f * UnityEngine.Random.value;



            var transforms = gameObject.GetComponentsInChildren<Transform>().Where(x=>x.transform != transform);
            foreach(Transform elem in transforms)
            {
                elem.localPosition += (cubify(elem.localPosition, true)-elem.localPosition)*intensity;
            }
        }


    }
    private Vector3 cubify(Vector3 v, bool noty)
    {
        return new Vector3(cubify(v.x), noty?v.y :cubify(v.y), cubify(v.z));
    }
    private float cubify(float x)
    {
        try
        {

            var abs =
                Mathf.Sqrt(
                    1f + (
                        1f / (
                            -Mathf.Abs(x) - 1f
                        )
                    )
                );
            
            return abs * Mathf.Sign(x);
            
        }
        catch
        {
            return Mathf.Infinity;
        }
    }
}
