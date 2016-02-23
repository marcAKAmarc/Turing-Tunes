using UnityEngine;
using System.Collections;

public class RockWallController : SolidController {
	public Transform Model;
	private float intensity = .25f;

	protected override void Start() {
		base.Start ();
		Mesh mesh = Model.GetComponent<MeshFilter>().mesh;
		Vector3[] vertices = mesh.vertices;
		Vector3[] normals = mesh.normals;
		int i = 0;
		while (i < vertices.Length){
			var abspos = transform.position + vertices[i];
			vertices[i] = vertices[i] + (new Vector3(0//Mathf.Sign (vertices[i].x)*Mathf.Sin (Mathf.Abs (abspos.x*abspos.y))
			                                              ,Mathf.Sign (vertices[i].y)* Mathf.Sin (Mathf.Abs (abspos.x*abspos.z))
			                                              , 0//Mathf.Sign(vertices[i].z)*Mathf.Sin (Mathf.Abs (abspos.z*abspos.y))
			                                              )
			                                  *.25f
			            										
			             					);
			i++;
		}
		mesh.vertices = vertices;
	}
}
