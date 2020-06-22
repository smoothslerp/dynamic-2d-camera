using UnityEngine;

public class PlayerControl : MonoBehaviour {
	
	public KeyCode jump;
	[HideInInspector]
	public bool jumpPressed;
	[HideInInspector]
	public bool jumpReleased;

	public KeyCode pause;
	public bool pausePressed;
	private bool paused = false;

	[HideInInspector]	
	public bool up = false;
	[HideInInspector]	
	public bool down = false;
	[HideInInspector]	
	public float horizontalInput;
	public static PlayerControl instance; 
	
	
	private void OnEnable() {
		if (instance == null) {
			instance = this;
		}
	}

	// Update is called once per frame
	void Update () {

		if (instance == null) {
			instance = this;
		}

		this.jumpPressed = Input.GetKey(this.jump);
		this.jumpReleased = Input.GetKeyUp(this.jump);
		this.pausePressed = Input.GetKeyUp(this.pause);

		float horizInput = Input.GetAxis("Horizontal");
		if (Mathf.Abs(horizInput) > 0.3) {
			this.horizontalInput = horizInput > 0 ? 1f : -1f;
		} else {
			this.horizontalInput = 0f;
		}

		this.up = Input.GetAxis("Vertical") > 0.3f ? true : false;
		this.down = Input.GetAxis("Vertical") < -0.3f ? true : false;

	}
}
