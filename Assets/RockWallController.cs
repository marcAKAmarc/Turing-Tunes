using UnityEngine;
using System.Collections;

public class RockWallController : SolidController {
	public Transform Model;
	private float intensity = .25f;
	private float seed;
    public bool Deletable;
    protected override void Awake()
    {
        base.Awake();
        base.deletable = Deletable;
    }
    protected override void Start() {
		base.Start ();

		seed = 0.0f; //must get seed from place common to rock models..  Mathf.PI * 2.0f * UnityEngine.Random.value;

		Mesh mesh = Model.GetComponent<MeshFilter>().mesh;
		Vector3[] vertices = mesh.vertices;
		int i = 0;
		float maxy = -9999.0f;
		while (i < vertices.Length) {
			if (vertices[i].y> maxy)
				maxy = vertices[i].y;
			i++;
		}
		i = 0;
		while (i < vertices.Length){
			if(vertices[i].y == maxy){
				var abspos = transform.position + vertices[i];
				vertices[i] = vertices[i] + (new Vector3(		/*Mathf.Sign (vertices[i].x)**/Mathf.Sin (Mathf.Abs (abspos.x*abspos.y)+seed)
				                                              ,	/*Mathf.Sign (vertices[i].y)**/Mathf.Sin (Mathf.Abs (abspos.x*abspos.z)+seed)
				                                              , /*Mathf.Sign (vertices[i].z)**/Mathf.Sin (Mathf.Abs (abspos.z*abspos.y)+seed)
				                                              )
				                                  *intensity
				            										
				             					);

			}
			i++;
		}
		mesh.vertices = vertices;
	}
}
