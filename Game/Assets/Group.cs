using UnityEngine;
using System.Collections;

public class Group : MonoBehaviour {

	float gravity = 0.0001f * 9.82f;

	bool isValidGridPos() {
		foreach (Transform child in transform) {
			Vector2 v = Grid.roundVec2(child.position);
			
			// Not inside Border?
			if (!Grid.insideBorder(v))
				return false;
			
			// Block in grid cell (and not part of same group)?
			if (Grid.grid[(int)v.x, (int)v.y] != null &&
			    Grid.grid[(int)v.x, (int)v.y].parent != transform)
				return false;
		}
		return true;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.UpArrow)) {
			transform.Rotate (0, 0, -90);

			if (isValidGridPos()) {
				updateGrid();
			} else {
				transform.Rotate (0, 0, 90);
			}
		} else if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			transform.position += new Vector3(-1, 0, 0);
			if (isValidGridPos()) {
				updateGrid();
			} else {
				transform.position += new Vector3(1, 0, 0);
			}
		} else if (Input.GetKeyDown (KeyCode.RightArrow)) {
			transform.position += new Vector3(1, 0, 0);
			if (isValidGridPos()) {
				updateGrid();
			} else {
				transform.position += new Vector3(-1, 0, 0);
			}
		} else if (Input.GetKeyDown (KeyCode.DownArrow)) {
			transform.position += new Vector3(0, -1, 0);

			if (isValidGridPos()) {
				updateGrid();
			} else {
				transform.position += new Vector3(0, 1, 0);

				FindObjectOfType<Spawner>().spawnNext();

				enabled = false;
			}
		}
	}
			
	void updateGrid() {
		// Remove old children from grid
		for (int y = 0; y < Grid.h; ++y)
			for (int x = 0; x < Grid.w; ++x)
				if (Grid.grid[x, y] != null)
					if (Grid.grid[x, y].parent == transform)
						Grid.grid[x, y] = null;
		
		// Add new children to grid
		foreach (Transform child in transform) {
			Vector2 v = Grid.roundVec2(child.position);
			Grid.grid[(int)v.x, (int)v.y] = child;
		}        
	}
}
