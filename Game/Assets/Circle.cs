using UnityEngine;
using System.Collections;
using System;

public class Circle : MonoBehaviour {

	public float gravity = 0.01f * 9.82f;

	public Vector3 velocity;

	private bool spawnedBall = false;
	private bool fixated = true;

	// Use this for initialization
	void Start () {

		velocity = new Vector3(0, 0, 0);
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

				if (!spawnedBall) {
					FindObjectOfType<Spawner>().spawnNext();
					spawnedBall = true;
				}
			}
			return true;
		}

		return false;
	}

	// Update is called once per frame
	void Update () {
		if (fixated) {
			if (Input.GetKeyDown (KeyCode.LeftArrow)) {
				transform.velocity += new Vector3(-1, 0, 0);
			} else if (Input.GetKeyDown (KeyCode.RightArrow)) {
				transform.velocity += new Vector3(1, 0, 0);
			} else if (Input.GetKeyDown (KeyCode.Space)) {
				fixated = false;
			}

			if (velocity[0] > 0) {
				velocity += new Vector3(-0.1, 0, 0);
			} else if (velocity[0] < 0) {
				velocity += new Vector3(0.1, 0, 0);
			}

			transform.position += velocity;

		} else {
			velocity += new Vector3 (0, -gravity, 0);

			if (velocity [1] < 0.01) {
				velocity[0] *= 0.8f;
			}

			transform.position += velocity;

			if (!isValidPos ()) {

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
	}

	bool isValidPos() {
		Vector2 v = Grid.roundVec2(transform.position);
		
		// Not inside Border?
		if (!Grid.insideBorder(v))
			return false;

		return true;
	}
}
