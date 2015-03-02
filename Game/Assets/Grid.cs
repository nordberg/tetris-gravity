using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class Grid : MonoBehaviour {

	public static int w = 10;
	public static int h = 20;
	public static GameObject[,] grid = new GameObject[w,h];
	public static List<Circle> m_circles = new List<Circle> ();
	private float m_integratorTimeStep = 1.0f / 60.0f;
	private float m_accumulator = 0.0f;
	private IntegratorType m_integratorType = IntegratorType.RK4;
	private static Integrator rk4 = new RK4Integrator ();
	
	public static Vector2 roundVec3(Vector3 v) {
		return new Vector2(Mathf.Round (v.x),
		                   Mathf.Round (v.y));
	}

	public static bool insideBorder(Vector2 pos) {
		return ((int)pos.x >= 0 &&
		        (int)pos.x + 1 < w &&
		        (int)pos.y >= 0);
	}

	public static void deleteRow(int y) {
		for (int x = 0; x < w; ++x) {
			grid[x, y].gameObject.GetComponentInChildren<Circle>().removeThis();
			grid[x, y] = null;
		}
	}

	public static bool isRowFull(int y) {
		List<string> dupl = new List<string> ();
		for (int x = 0; x < w; ++x) {
			if (grid[x, y] == null) {
				return false;
			}
			if (!dupl.Contains (grid[x,y].name)) {
				dupl.Add(grid[x,y].name);
			}
		}

		if (dupl.Count < 5) {
			return false;
		}
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
		Debug.Log ("Grid start");
	}
	
	// Update is called once per frame
	void Update () {
		m_accumulator += Mathf.Min(Time.deltaTime / m_integratorTimeStep, 3.0f);

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
