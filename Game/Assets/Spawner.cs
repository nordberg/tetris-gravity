using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

	public GameObject[] groups;

	private int ballsSpawned = 0;

	public void spawnNext() {

		if (ballsSpawned < 100) {
			int i = Random.Range (0, groups.Length);
			
			Instantiate (groups [i],
			             transform.position,
			             Quaternion.identity);

			ballsSpawned++;
		}
	}

	// Use this for initialization
	void Start () {
		spawnNext();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
