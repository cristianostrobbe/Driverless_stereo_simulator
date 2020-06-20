using UnityEngine;
using System.Collections;

public class Cam : MonoBehaviour {
	public float speed = 0.5f;

	
	// Update is called once per frame
	void Update () {
	
		transform.Rotate (Vector3.up * Time.deltaTime * speed);
	
	
	
	}
}
