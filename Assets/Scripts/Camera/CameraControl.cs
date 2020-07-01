using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

	/** In screen coordinates */
	private float currentLeftLimit;
	private float currentRightLimit;
	private float currentUpLimit;
	private float currentDownLimit;

	[Range(1f, 10f)]	
	public float leftLimit;
	[Range(1f, 10f)]	
	public float rightLimit;
	[Range(1f, 10f)]	
	public float upLimit;
	[Range(1f, 10f)]	
	public float downLimit;
	private Player player;
	[Range(50f, 500f)]
	public float stdDiff;
	public float switchSlowDownRate;
	private bool switchedH = false;
	private bool switchedV = false;
	public float switchSpeed;
	private Vector2 diff;
	private static CameraControl instance; 

	
	// -------------- GET CAMERA FROM CHILD OBJECT -------------- //
	private Transform camT;
	private Camera cam;

	void Awake() {
		if (instance == null) {
			instance = this;
		}
	}

	// Use this for initialization
	void Start () {
		player = GameObject.Find("Player").GetComponent<Player>();
		camT = this.transform.GetChild(0);
		cam = camT.GetComponent<Camera>();

		this.currentLeftLimit = this.leftLimit;
		this.currentRightLimit = this.rightLimit;
		this.currentUpLimit = this.upLimit;
		this.currentDownLimit = this.downLimit;
	}

	public static CameraControl GetCameraControlInstance() {
		return instance;
	}
	
	private void FixedUpdate() {
		SwitchHorizontalAnchor();
		SwitchVerticalAnchor();
		verticalCameraMovement();
		horizontalCameraMovement();	

		MoveCurrentLimits();
	}

	private void horizontalCameraMovement () {

		float minLine = (this.currentLeftLimit/10f) * this.cam.pixelWidth;
		float maxLine = (this.currentRightLimit/10f) * this.cam.pixelWidth;

		Vector3 playerScreenPos = cam.WorldToScreenPoint(player.transform.position);

		if (playerScreenPos.x > maxLine) {
			float diff = playerScreenPos.x - maxLine;

			float standardDiff = this.stdDiff;
			// if (Mathf.Abs(diff) > this.cam.pixelWidth/30f) {
			// 	standardDiff *= this.switchSlowDownRate;
			// }

			Vector3 newPosition = cam.WorldToScreenPoint(this.transform.position) + new Vector3(diff, 0f, 0f);
			this.UpdatePosition(Vector3.Lerp(this.transform.position, cam.ScreenToWorldPoint(newPosition), Mathf.Abs(diff)/standardDiff));
			
		} else if (playerScreenPos.x < minLine) {
			float diff = playerScreenPos.x - minLine;

			float standardDiff = this.stdDiff;
			if (Mathf.Abs(diff) > this.cam.pixelWidth/30f) {
				standardDiff *= this.switchSlowDownRate;
			}

			Vector3 newPosition = cam.WorldToScreenPoint(this.transform.position) + new Vector3(diff, 0f, 0f);	
			this.UpdatePosition(Vector3.Lerp(this.transform.position, cam.ScreenToWorldPoint(newPosition), Mathf.Abs(diff)/standardDiff));
		}
	}

	private void verticalCameraMovement () { 
		
		float minLine = (this.currentDownLimit/10f) * this.cam.pixelHeight;
		float maxLine = (this.currentUpLimit/10f) * this.cam.pixelHeight;

		Vector3 playerScreenPos = cam.WorldToScreenPoint(player.transform.position);

		if (playerScreenPos.y > maxLine) {
			float diff = playerScreenPos.y - maxLine;

			Vector3 newPosition = cam.WorldToScreenPoint(this.transform.position) + new Vector3(0f, diff, 0f);
			this.UpdatePosition(Vector3.Lerp(this.transform.position, cam.ScreenToWorldPoint(newPosition), Mathf.Abs(diff)/this.stdDiff));
		} else if (playerScreenPos.y < minLine ) {
			float diff = playerScreenPos.y - minLine;

			Vector3 newPosition = cam.WorldToScreenPoint(this.transform.position) + new Vector3(0f, diff, 0f);	
			this.UpdatePosition(Vector3.Lerp(this.transform.position, cam.ScreenToWorldPoint(newPosition), Mathf.Abs(diff)/this.stdDiff));
		}
	}

	private void SwitchHorizontalAnchor () {

		CameraLimits cc = this.GetCameraLimits(); // for switching anchor we need the hard values

		float left = (cc.leftLimit/10f) * this.cam.pixelWidth;
		float right = (cc.rightLimit/10f) * this.cam.pixelWidth;
		float mid = (left + right)/2f;

		Vector3 playerScreenPos = cam.WorldToScreenPoint(player.transform.position);

		PlayerState ps = this.player.GetState();
		Movement m = this.player.GetMovement();
		float h = this.player.GetHorizontalSpeed();

		if (playerScreenPos.x < mid && ps.facing < 0 && Mathf.Abs(h) >= (m.maxHorizontalSpeed-3f)) {
			this.switchedH = true;
		}
		
		if (playerScreenPos.x > mid && ps.facing > 0 && Mathf.Abs(h) >= m.maxHorizontalSpeed-3f) {
			this.switchedH = false;
		}
	}

	private void SwitchVerticalAnchor () {

		CameraLimits cc = this.GetCameraLimits(); // for switching anchor we need the hard values
		
		float down = (cc.downLimit/10f) * this.cam.pixelHeight;
		float up = (cc.upLimit/10f) * this.cam.pixelHeight;
		float mid = (down + up)/2f;

		Vector3 playerScreenPos = cam.WorldToScreenPoint(player.transform.position);
		Movement m = this.player.GetMovement();
		float v = this.player.GetVerticalSpeed();

		if (playerScreenPos.y < mid && Mathf.Abs(v) >= 5f) { // on the way down, want to see down
			this.switchedV = true;
		}
		
		if (playerScreenPos.y > mid && Mathf.Abs(v) >= 5f) { // on the way up, want to see up
			this.switchedV = false;
		}
	}

	public CameraLimits GetCameraLimits() {
		float left = this.switchedH ? 10 - this.rightLimit : this.leftLimit;
		float right = this.switchedH ? 10 - this.leftLimit : this.rightLimit;

		float down  = this.switchedV ? 10 - this.upLimit : this.downLimit;
		float up  = this.switchedV ? 10 - this.downLimit : this.upLimit;

		return new CameraLimits(left, right, up, down);
	}

	public CameraLimits GetCurrentCameraLimits() {
		return new CameraLimits(currentLeftLimit, currentRightLimit, currentUpLimit, currentDownLimit);
	}

	private void MoveCurrentLimits() {

		CameraLimits cc = this.GetCameraLimits();
		
		Vector2 leftRight = Vector2.Lerp(new Vector2(currentLeftLimit, currentRightLimit), new Vector2(cc.leftLimit, cc.rightLimit), this.switchSpeed * Time.deltaTime);
		Vector2 downUp = Vector2.Lerp(new Vector2(currentDownLimit, currentUpLimit), new Vector2(cc.downLimit, cc.upLimit), this.switchSpeed * Time.deltaTime);

		this.currentLeftLimit = leftRight.x;
		this.currentRightLimit = leftRight.y;
		this.currentDownLimit = downUp.x;
		this.currentUpLimit = downUp.y;
		
	}

	private void UpdatePosition (Vector3 newPos) {
		this.diff = new Vector2(newPos.x - this.transform.position.x, newPos.y - this.transform.position.y);
		this.transform.position = newPos;
	}

}
