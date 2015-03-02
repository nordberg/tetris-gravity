using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {
	
	public GameObject[] groups;
	
	public Group activeGroup = null;
	
	public void spawnNext() {
		int i = Random.Range (0, groups.Length);
		
		GameObject go = (GameObject) Instantiate (groups[i],
		                                          transform.position,
		                                          Quaternion.identity);
		
		Group g = (Group) go.GetComponent<Group>();
		g.dontMove();
		
		activeGroup = g;
	}
	
	// Use this for initialization
	void Start () {
		spawnNext();
	}
	
	float timeSince = 0f;
	float timeSpawn = 0f;
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) {
			if (activeGroup != null) {
				activeGroup.StartMoving();
				
				foreach (Circle c in activeGroup.children) {
					c.ClearForce();
					c.ApplyGravity();
					c.ApplyGroundForce();
					Grid.m_circles.Add(c);
				}
				activeGroup = null;
				timeSpawn =	Time.time;
			}
		}
		
		timeSince = Time.time - timeSpawn;
		if (activeGroup == null) {
			if (timeSince >= 2) {
				spawnNext();
			}
		}
	}
}