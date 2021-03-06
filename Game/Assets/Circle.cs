﻿using UnityEngine;
using System.Collections;
using System;

public class Circle : MonoBehaviour {

	public class CircleState
	{
		public CircleState()
		{
			Position = Vector3.zero;
			Velocity = Vector3.zero;
		}
		
		public CircleState(Vector3 pos, Vector3 vel)
		{
			Position = pos;
			Velocity = vel;
		}
		
		public Vector3 Position;
		public Vector3 Velocity;
		
		public CircleState Clone()
		{
			return new CircleState(Position, Velocity);
		}
	}
	
	public CircleState State { get; set; }

	public void SaveState()
	{
		m_savedState = State.Clone();
	}
	public void LoadState()
	{
		State = m_savedState.Clone();
	}
	
	private CircleState m_savedState = new CircleState();
	
	void Awake()
	{
		State = new Circle.CircleState();
		//Get initial position
		State.Position = transform.position;
	}

	public Vector3 Force{ get; private set; }
	public float Mass = 1.0f;
	private int prev_row = 0;
	private int prev_col = 0;
	
	private float radius = 0.5f;

	public bool fixated = true;
	
	void Update()
	{ 
		// Fixated when at the top of the screen
		if (fixated) {
			State.Position = transform.position;
			State.Velocity = Vector3.zero;
		} else {
			//Update graphical representation
			if ((State.Position - transform.position).magnitude > 0.01f) {
				transform.position = State.Position;
			}
			Vector2 rounded = Grid.roundVec3(transform.position);

			// Place in the grid
			int row = (int) rounded.y;
			int col = (int) rounded.x;
			for (int i = 0; i < Grid.h; i++) {
				if (Grid.rows[i].Contains(transform)) {
					Grid.rows[i].Remove (transform);
					break;
				}
			}

			Grid.rows[row].Add(transform);

			for (int i = 0; i < Grid.h; i++) {
				if (Grid.isRowFull(i)) {
					Grid.deleteRow(i);
				}
			}
		}
	}

	// Remove this circle
	public void removeThis() {
		Grid.m_circles.Remove (this);
		Destroy (this.transform.gameObject);
		enabled = false;
	}

	// Use this for initialization
	void Start () {
		Force = new Vector3 (0, 0, 0);
	}

	public void ClearForce() {
		Force = new Vector3 (0, 0, 0);
	}

	public void ApplyGravity() {
		Force += new Vector3 (0, -9.82f, 0);
	}

	public void ApplyForce(Vector3 f) {
		Force += f;
	}

	public void ResolveCollisions() {
		foreach (Circle c in Grid.m_circles.ToArray()) {
			if (this != c) {
				Vector3 distVec = c.State.Position - State.Position;
				float dist = distVec.magnitude;

				if (dist > 2 * radius) {
					continue;
				}

				float mCircleStiffness = 800f;
				float mCircleDampning = 10f;

				float depth = dist - 2*radius;

				// When two spheres are intersecting, apply a force that is directed 
				// away from the other sphere. Dampning is a force loss.
				Vector3 f1 = depth * mCircleStiffness * distVec.normalized;
				Vector3 dampning = -mCircleDampning * State.Velocity;

				ApplyForce (f1);
				c.ApplyForce (-f1);

				ApplyForce (dampning);
				c.ApplyForce (-dampning);
			}
		}
	}

	public void ApplyGroundForce() {
		// GROUND
		float distToGround = transform.position [1] - 2 * radius;
		Vector3 groundForce = new Vector3 (0, 0, 0);
		float groundStiffness = 800f;
		float m_groundDamping = 20f;

		if (distToGround < 0) {
			float depth = 0 - distToGround;
			groundForce += new Vector3(0, groundStiffness * depth, 0);
			ApplyForce(-m_groundDamping * State.Velocity); // Varför kan inte den här adderas till groundForce?
		}

		if (transform.position [0] <= 0) {
			float depth = Mathf.Abs(0 - transform.position[0]);
			groundForce += new Vector3(groundStiffness * depth, 0, 0);
		} else if (transform.position [0] >= Grid.w - 2 * radius) {
			float depth = transform.position[0] - (Grid.w - 2 * radius);
			groundForce += new Vector3(-groundStiffness * depth, 0, 0);
		}

		ApplyForce (groundForce);
	}

	public void ApplyNeighborForce() {
		Component parent = GetComponentInParent<Group> ();

		Group parentGroup = (Group) parent;
		parentGroup.NeighborForces ();
	}
}
