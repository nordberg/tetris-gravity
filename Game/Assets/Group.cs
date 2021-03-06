﻿using UnityEngine;
using System.Collections;

public class Group : MonoBehaviour {
	public Component[] children;
	float[,] distanceMatrix;

	private bool fixated = false;
	public static Group activeGroup = null;

	// Use this for initialization
	void Start () {
		children = GetComponentsInChildren<Circle> ();
		distanceMatrix = new float[children.Length, children.Length];

		// Determine how close spheres in a group are with a distance matrix
		// distanceMatrix[i,j] is the distance between i and j
		// If they're too far from eachother (<2), -1 is used and they'll
		// no longer be attached to eachother
		for (int i = 0; i < children.Length; i++) {
			for (int j = 0; j < children.Length; j++) {
				if (i == j) {
					continue;
				}
				Circle c1 = (Circle) children[i];
				Circle c2 = (Circle) children[j];
				Vector3 r_vec = c1.transform.position - c2.transform.position;
				
				float dist = r_vec.magnitude;
				if(dist < 2) {
					distanceMatrix[i,j] = dist;
					distanceMatrix[j,i] = dist;
				} else {
					distanceMatrix[i,j] = -1;
					distanceMatrix[j,i] = -1;
				}
			}
		}
	}

	public void StartMoving() {
		fixated = false;
		for (int j = 0; j < children.Length; j++) {
			Circle c1 = (Circle) children[j];
			c1.fixated = false;
		}
	}

	public void dontMove() {
		fixated = true;
		for (int j = 0; j < children.Length; j++) {
			Circle c1 = (Circle) children[j];
			c1.fixated = true;
		}
	}

	
	// Update is called once per frame
	void Update () {
		if (fixated) {
			if (Input.GetKeyDown (KeyCode.LeftArrow)) {
				transform.position +=  new Vector3(-1.0f,0f,0f);
				if (!isValidPos()) {
					transform.position +=  new Vector3(1.0f,0f,0f);
				}
			} else if (Input.GetKeyDown (KeyCode.RightArrow)) {
				transform.position +=  new Vector3(1.0f,0f,0f);
				if (!isValidPos()) {
					transform.position +=  new Vector3(-1.0f,0f,0f);
				}
			} else if (Input.GetKey (KeyCode.UpArrow)) {
				transform.Rotate(0,0,-7f);
				if (!isValidPos()) {
					transform.Rotate (0,0,7);
				}
			} else if (Input.GetKey (KeyCode.DownArrow)) {
				transform.Rotate(0,0,7f);
				if (!isValidPos()) {
					transform.Rotate(0,0,-7f);
				}
			} 
		}
	}

	// If the new position is valid
	bool isValidPos() {
		foreach (Circle child in children) {
			Vector2 pos = new Vector2(child.transform.position.x, child.transform.position.y);
			if (!Grid.insideBorder(pos)) {
				return false;
			}
		}
		return true;
	}

	public void NeighborForces() {
		for (int i = 0; i < children.Length; i++) {
			Circle c1 = (Circle) children[i];
			if (c1 == null) 
				continue;

			for (int j = 0; j < children.Length; j++) {
				
				// Code to check if they are close enough to stick together
				if (i == j) {
					continue;
				}

				if (distanceMatrix[i,j] == -1) {
					continue;
				}

				Circle c2 = (Circle) children[j];
				if (c2 == null) 
					continue;

				Vector3 r_vec = c1.transform.position - c2.transform.position;
				
				float dist = r_vec.magnitude;
				
				float diff_dist = dist - distanceMatrix[i,j];

				if (diff_dist > 0.1f) {
					distanceMatrix[i, j] = -1;
					continue;
				}

				// They are close enough to stick together. Calculate forces
				// to keep them together. Introduce a tough spring between them.
				float m_attraction = 250f;
				float m_dampning = 4f;
				float max_force = 100f;

				Vector3 force = m_dampning * 
					(c1.State.Velocity - c2.State.Velocity) + 
					m_attraction * diff_dist * r_vec.normalized;

				float magnitude = force.magnitude;

				if (magnitude > max_force) {
					for (int k = 0; k < children.Length; k++) {
						distanceMatrix[i, k] = -1;
						distanceMatrix[k, i] = -1;
					}
					force = force.normalized * max_force;
				}

				Color c = Color.green;
				if(magnitude > max_force/4f) {
					c = Color.blue;
					if (magnitude > max_force /2f ) {
						c = Color.red;
					}
				}

				Debug.DrawLine (c1.State.Position, c2.State.Position,c);
				c1.ApplyForce (-force);
				c2.ApplyForce (force);
			}
		}
	}
}
