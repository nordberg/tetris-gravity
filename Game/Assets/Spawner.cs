using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

	public GameObject[] groups;
	public Circle m_ballPrefab = null;

	private int ballsSpawned = 0;

	public void spawnNext() {

		if (ballsSpawned < 100) {
			int i = Random.Range (0, groups.Length);
			
			Circle c = (Circle) Instantiate (m_ballPrefab,
			             transform.position,
			             Quaternion.identity);
			c.ClearForce();
			c.ApplyGravity();
			c.ApplyGroundForce();



			Grid.m_circles.Add(c);


			//Debug.Log ("New ball");
			//Debug.Log(Grid.m_circles.Count);
			//Det skapas massa bolar och de hamnar i m_circles

			ballsSpawned++;
		}
	}

	// Use this for initialization
	void Start () {
		spawnNext();
	}
	
	// Update is called once per frame
	void Update () {
		//spawnNext ();
		if (Input.GetKeyDown (KeyCode.Space)) {
			spawnNext ();
		}
	}
}
