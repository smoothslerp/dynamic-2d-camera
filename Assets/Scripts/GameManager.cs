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

	private Player player;
	private CameraControl cameraControl;
	
	void Awake() {
		if (!instance) instance = this;

		player = FindObjectOfType<Player>();
		cameraControl = FindObjectOfType<CameraControl>();
		cameraControl.PreInitialize();
	}

	void Start () {
		// ignore coll with helper layer 
        Physics2D.IgnoreLayerCollision(11, 10);
		// ignore coll with defualt layer
        Physics2D.IgnoreLayerCollision(11, 0);
		// Physics2D.SetLayerCollisionMask(11, 9);

		player.Initialize();
		cameraControl.Initialize();
		cameraControl.InjectCameraTarget(player);
	}

    void FixedUpdate()
    {
		cameraControl.FixedTick();
    }

    void Update () {
		if (Input.GetKeyDown(KeyCode.M)) {
			  SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}

		player.Tick();
	}
	
}
