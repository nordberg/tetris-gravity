using UnityEngine;
using System.Collections;
using System;

public class Circle : MonoBehaviour {

	public class PointState
	{
		public PointState()
		{
			Position = Vector3.zero;
			Velocity = Vector3.zero;
		}
		
		public PointState(Vector3 pos, Vector3 vel)
		{
			Position = pos;
			Velocity = vel;
		}
		
		public Vector3 Position;
		public Vector3 Velocity;
		
		public PointState Clone()
		{
			return new PointState(Position, Velocity);
		}
	}
	
	public PointState State { get; set; }
	
	public Vector3 Force{ get; private set; }
	public float Mass = 1.0f;
	
	public void SaveState()
	{
		m_savedState = State.Clone();
	}
	public void LoadState()
	{
		State = m_savedState.Clone();
	}
	
	private PointState m_savedState = new PointState();
	
	//A note about this way of setting the position in Unity:
	//  This in not how you really would use Unity in most
	//  cases. Normally you use their physics engine, which 
	//  would handle this in a better way with interpolation 
	//  and other fancy features. This is only to get clean 
	//  access to Pos/Vel/Force in a consistent way.
	
	void Awake()
	{
		State = new Circle.PointState();
		//Get initial position
		State.Position = transform.position;
	}
	
	void Update()
	{ 
		//Update graphical representation
		if ((State.Position - transform.position).magnitude > 0.01f) {
			transform.position = State.Position;
		}
	}

	public float gravity = 0.01f * 9.82f;

	private Vector3 velocity;

	private float radius = 0.5f;

	private bool spawnedBall = false;
	private bool fixated = true;
	public bool connected = true;

	// Use this for initialization
	void Start () {

		velocity = new Vector3(0, 0, 0);
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
		foreach (Circle c in Grid.m_circles) {
			if (this != c) {
				Vector3 distVec = c.State.Position - State.Position;
				float dist = distVec.magnitude;
				float mCircleStiffness = 400f;
				float mCircleDampning = 10f;

				if(distVec.magnitude <= 0) 
					Debug.Log ("Inte bra");

				if (dist < 2 * radius) {
					float depth = dist - 2*radius;
					Vector3 f1 = depth * mCircleStiffness * distVec.normalized;
					Vector3 dampning = -mCircleDampning * State.Velocity;
					ApplyForce (f1);
					c.ApplyForce (-f1);
					ApplyForce (dampning);
					c.ApplyForce (-dampning);
				}
			}
		}
	}

	public void ApplyGroundForce() {
		// GROUND
		float distToGround = transform.position [1] - 2 * radius;
		Vector3 groundForce = new Vector3 (0, 0, 0);
		float groundStiffness = 800f;
		float m_groundDamping = 10f;

		if (distToGround < 0) {
			float depth = 0 - distToGround;
			groundForce += new Vector3(0, groundStiffness * depth, 0);
			ApplyForce (-m_groundDamping * State.Velocity);
		}

		// LEFT WALL
		if (transform.position [0] <= 0) {
			float depth = Mathf.Abs(0 - transform.position[0]);
			groundForce += new Vector3(groundStiffness * depth, 0, 0);
		}

		if (transform.position [0] >= Grid.w - 2 * radius) {
			float depth = transform.position[0] - (Grid.w - 2 * radius);
			groundForce += new Vector3(-groundStiffness * depth, 0, 0);
		}

		ApplyForce (groundForce);
	}

	public void ApplyNeighborForce() {
		Component parent = GetComponentInParent<Group> ();

		Group parentGroup = (Group)parent;
		parentGroup.NeighborForces ();
	}

	// Update is called once per frame
	/*void Update () {
		/*if (fixated) {
			if (Input.GetKeyDown (KeyCode.LeftArrow)) {
				if (velocity[0] >= -2) {
					velocity += new Vector3(-radius, 0, 0);
				}
			} else if (Input.GetKeyDown (KeyCode.RightArrow)) {
				if (velocity[0] <= 2) {
					velocity += new Vector3(radius, 0, 0);
				}
			} else if (Input.GetKeyDown (KeyCode.Space)) {
				fixated = false;
			}

			if (velocity[0] > 0) {
				velocity += new Vector3(-radius, 0, 0);
			} else if (velocity[0] < 0) {
				velocity += new Vector3(radius, 0, 0);
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
					velocity[1] *= -radius;
					transform.position = new Vector3 (transform.position [0], -radius, 0);;
				}

				if (transform.position[0] <= 0) {
					velocity[0] *= -radius;
					transform.position = new Vector3 (0.1f, transform.position [1], 0);
				} else if (transform.position[0] >= Grid.w - 1) {
					velocity[0] *= -radius;
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
	}*/

	
}
