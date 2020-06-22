using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour {
	
	private bool paused = false;
	static GameManager instance;


	public static GameManager GetInstance() {
		return instance;
	}
	
	void Awake() {
		if (!instance) instance = this;
	}

	void Start () {
		// ignore coll with helper layer 
        Physics2D.IgnoreLayerCollision(11, 10);
		// ignore coll with defualt layer
        Physics2D.IgnoreLayerCollision(11, 0);
		// Physics2D.SetLayerCollisionMask(11, 9);
	}

	void Update () {
		if (Input.GetKeyDown(KeyCode.M)) {
			  SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}
	}
	
}
