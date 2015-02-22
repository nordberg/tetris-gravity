using UnityEngine;
using System.Collections;

public class Circle : MonoBehaviour {

	public float gravity = 0.01f * 9.82f;

	public Vector3 velocity;

	// Use this for initialization
	void Start () {
		velocity = new Vector3(0, 0, 0);
	}
	
	// Update is called once per frame
	void Update () {
		velocity += new Vector3(0, -gravity, 0);
		transform.position += velocity;
	}
}
