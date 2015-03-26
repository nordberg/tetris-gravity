using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class Grid : MonoBehaviour {

	// The grid size
	public static int w = 10;
	public static int h = 20;

	// The grid to keep track of spheres and if there's a full row
	public static Transform[,] grid = new Transform[w,h];

	public static List<List<Transform>> rows = new List<List<Transform>>(h);
	public static List<Circle> m_circles = new List<Circle> ();

	private float m_integratorTimeStep = 1.0f / 60.0f;
	private float m_accumulator = 0.0f;

	private static Integrator rk4 = new RK4Integrator ();

	private static int score = 0;
	public static Text scoreText = null;
	
	public static Vector2 roundVec3(Vector3 v) {
		return new Vector2(Mathf.Round (v.x),
		                   Mathf.Round (v.y));
	}

	public static bool insideBorder(Vector2 pos) {
		return (pos.x >= 0 &&
		        pos.x + 1 <= w);
	}

	private static void addScore(int pscore) {
		score += pscore;
		updateScore();
	}

	private static void updateScore() {
		scoreText.text = "Score: " + score;
	}

	public static void deleteRow(int y) {
		foreach (Transform child in rows[y]) {
			child.gameObject.GetComponentInChildren<Circle>().removeThis();

		}
		rows[y].Clear();
	}

	// Just having a full row is too easy, you need a full row
	// with a certain amount of colours aswell.
	public static bool isRowFull(int y) {
		
		// Colours represented in the full row
		List<string> dupl = new List<string> ();

		if (rows [y].Count < 9) {
			return false;
		}

		// Count the amount of colours represented
		foreach (Transform child in rows[y]) {
			if (!dupl.Contains(child.name)) {
				dupl.Add (child.name);
			}
		}

		// 6 colours needed to get points
		if (dupl.Count < 6) {
			return false;
		}

		addScore(dupl.Count * 100);

		return true;
	}

	void ApplyForces(float timeStep) {
		ClearAndApplyGravity ();
	}

	void ClearAndApplyGravity() {
		foreach (Circle c in m_circles.ToList()) {
			c.ClearForce ();
			c.ApplyGravity ();
			c.ApplyGroundForce();
			c.ResolveCollisions();
			c.ApplyNeighborForce();
		}
	}

	// Use this for initialization
	void Start () {
		scoreText = FindObjectOfType<Text>();
		rows = new List<List<Transform>> ();
		for (int i = 0; i < h; i++) {
			rows.Add(new List<Transform>());
		}

		Debug.Log ("Grid start");
	}
	
	// Update is called once per frame
	void Update () {

		m_accumulator += Mathf.Min(Time.deltaTime / m_integratorTimeStep, 5.0f);

		while (m_accumulator > 1.0f)
		{
			m_accumulator -= 1.0f;
			AdvanceSimulation();
		}
	}

	void AdvanceSimulation()
	{
		rk4.Advance(m_circles, ApplyForces, m_integratorTimeStep);
	}
}
