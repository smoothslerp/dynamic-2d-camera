using UnityEngine;

public class CameraControl : MonoBehaviour {

	/** SETTINGS */
	[Range(0.1f, 1f)]	
	public float leftLimit;
	[Range(0.1f, 1f)]	
	public float rightLimit;
	[Range(0.1f, 1f)]	
	public float upLimit;
	[Range(0.1f, 1)]	
	public float downLimit;
	[Range(0f, 1f)]
	public float speed; 
	[Range(0f, 1f)]
	public float switchSpeed;

	/** INSTANCES  */
	public static CameraControl instance; 
	private Transform tracking;

	/** DELEGATES */
	public delegate void AnchorSwitcher(float min, float max, ref bool switchedHV);
	public AnchorSwitcher horizontalAnchorSwitcher;
	public AnchorSwitcher verticalAnchorSwitcher;
	
	/** STATE */
	private bool init = false; 
	private bool switchedH = false;
	private bool switchedV = false;
	private CameraLimits current;

	void Awake() {
		if (instance == null) {
			instance = this;
		}
	}

	public void Init(Transform tracking, AnchorSwitcher hSwitcher, AnchorSwitcher vSwitcher) {
		this.tracking = tracking;
		this.horizontalAnchorSwitcher = hSwitcher;
		this.verticalAnchorSwitcher = vSwitcher;
		this.init = this.tracking != null && this.horizontalAnchorSwitcher != null && verticalAnchorSwitcher != null;
	}

	void Start () {
		this.current = new CameraLimits(this.leftLimit, this.rightLimit, this.upLimit, this.downLimit);
	}

	private void FixedUpdate() {

		if (!this.init) {
			Debug.LogError("Camera Control has not been completely intialized!");
			return;
		}
		verticalCameraMovement();
		horizontalCameraMovement();

		SwitchHorizontalAnchor();
		SwitchVerticalAnchor();

		MoveCurrentLimits();
	}

	private void horizontalCameraMovement () {

		Camera cam = Camera.main;
		
		float minLine = (this.current.leftLimit) * cam.pixelWidth;
		float maxLine = (this.current.rightLimit) * cam.pixelWidth;

		Vector3 screenPos = cam.WorldToScreenPoint(this.tracking.position);

		float diff = 0;
		if (screenPos.x > maxLine) {
			diff = screenPos.x - maxLine;
		} else if (screenPos.x < minLine)  {
			diff = screenPos.x - minLine;
		} else return;

		Vector3 newPosition = cam.WorldToScreenPoint(this.transform.position) + new Vector3(diff, 0f, 0f);
		this.transform.position = Vector3.Lerp(this.transform.position, cam.ScreenToWorldPoint(newPosition), this.speed);
	}

	private void verticalCameraMovement () { 

		Camera cam = Camera.main;
		
		float minLine = (this.current.downLimit) * cam.pixelHeight;
		float maxLine = (this.current.upLimit) * cam.pixelHeight;

		Vector3 screenPos = cam.WorldToScreenPoint(this.tracking.position);

		float diff = 0;
		if (screenPos.y > maxLine) {
			diff = screenPos.y - maxLine;
		} else if (screenPos.y < minLine) {
			diff = screenPos.y - minLine;
		} else return;

		Vector3 newPosition = cam.WorldToScreenPoint(this.transform.position) + new Vector3(0f, diff, 0f);	
		this.transform.position = Vector3.Lerp(this.transform.position, cam.ScreenToWorldPoint(newPosition), this.speed);
	}

	private void SwitchHorizontalAnchor () {

		Camera cam = Camera.main;
		CameraLimits cc = this.GetAnchoredLimits();

		float left = (cc.leftLimit) * cam.pixelWidth;
		float right = (cc.rightLimit) * cam.pixelWidth;

		this.horizontalAnchorSwitcher(left, right, ref this.switchedH);
	}

	private void SwitchVerticalAnchor () {

		Camera cam = Camera.main;
		CameraLimits cc = this.GetAnchoredLimits();
		
		float down = (cc.downLimit) * cam.pixelHeight;
		float up = (cc.upLimit) * cam.pixelHeight;
		
		this.verticalAnchorSwitcher(up, down, ref this.switchedV);
	}

	public CameraLimits GetAnchoredLimits() {
		float left = this.switchedH ? 1 - this.rightLimit : this.leftLimit;
		float right = this.switchedH ? 1 - this.leftLimit : this.rightLimit;

		float down  = this.switchedV ? 1 - this.upLimit : this.downLimit;
		float up  = this.switchedV ? 1 - this.downLimit : this.upLimit;

		return new CameraLimits(left, right, up, down);
	}

	public CameraLimits GetCurrentCameraLimits() {
		return new CameraLimits(this.current); // return copy
	}

	private void MoveCurrentLimits() {

		CameraLimits cc = this.GetAnchoredLimits();
		
		this.current.leftLimit = Mathf.Lerp(this.current.leftLimit, cc.leftLimit, this.switchSpeed);
		this.current.rightLimit = Mathf.Lerp(this.current.rightLimit, cc.rightLimit, this.switchSpeed);
		this.current.downLimit = Mathf.Lerp(this.current.downLimit, cc.downLimit, this.switchSpeed);
		this.current.upLimit = Mathf.Lerp(this.current.upLimit, cc.upLimit, this.switchSpeed);

	}

}
