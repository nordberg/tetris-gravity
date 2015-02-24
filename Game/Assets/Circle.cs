using UnityEngine;
using System.Collections;
using System;

public class Circle : MonoBehaviour {

	public float gravity = 0.01f * 9.82f;

	public Vector3 velocity;

	private bool spawnedBall = false;

	// Use this for initialization
	void Start () {
		System.Random r = new System.Random ();

		velocity = new Vector3(0.1f * ((float) r.NextDouble() - 0.5f), 0, 0);
	}

	bool isColliding(Circle c) {
		Vector3 distVec = transform.position - c.transform.position;
		if (distVec.magnitude <= 1) {
			transform.position = c.transform.position + distVec.normalized;
			Vector3 tmp = velocity;
			velocity = 0.9f*c.velocity;
			c.velocity = tmp;

			if (velocity[1] < 0.001 && velocity[0] < 0.001 &&
			    c.velocity[1] < 0.001 && c.velocity[2] < 0.001) {
				//c.velocity = new Vector3(0, 0, 0);
				//velocity = new Vector3(0, 0, 0);

				if (!spawnedBall) 
					FindObjectOfType<Spawner>().spawnNext();
				spawnedBall = true;
			}
			return true;
		}

		return false;
	}

	// Update is called once per frame
	void Update () {
		velocity += new Vector3 (0, -gravity, 0);

		if (velocity [1] < 0.01) {
			velocity[0] *= 0.8f;
		}

		transform.position += velocity;
		if (!isValidPos ()) {
			//transform.position = new Vector3 (transform.position [0], -0.5f, 0);
			//velocity = -0.5f * velocity;
			if (transform.position[1] <= 0) {
				velocity[1] *= -0.5f;
				transform.position = new Vector3 (transform.position [0], -0.5f, 0);;
			}

			if (transform.position[0] <= 0) {
				velocity[0] *= -0.5f;
				transform.position = new Vector3 (0.1f, transform.position [1], 0);
			} else if (transform.position[0] >= Grid.w - 1) {
				velocity[0] *= -0.5f;
				transform.position = new Vector3 (Grid.w - 1.1f, transform.position [1], 0);
			}

			if (Mathf.Abs (velocity [1]) < 0.01 && Mathf.Abs (velocity [0]) < 0.01) {
				velocity = new Vector3 (0, 0, 0);
				if (!spawnedBall) {
					FindObjectOfType<Spawner> ().spawnNext ();
					spawnedBall = true;
				}
			}
		}

		foreach (Circle c in FindObjectsOfType<Circle>()) {
			if (c.transform.position != transform.position) {
				isColliding(c);
			} 
		}
	}

	bool isValidPos() {
		Vector2 v = Grid.roundVec2(transform.position);
		
		// Not inside Border?
		if (!Grid.insideBorder(v))
			return false;
		
		// Block in grid cell (and not part of same group)?
		/*if (Grid.grid[(int)v.x, (int)v.y] != null &&
		    Grid.grid[(int)v.x, (int)v.y].parent != transform)
			return false;*/
		return true;
	}
}
