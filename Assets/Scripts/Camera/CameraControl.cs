using UnityEngine;

public class CameraControl : MonoBehaviour {

	/** SETTINGS */
	[Range(1f, 10f)]	
	public float leftLimit;
	[Range(1f, 10f)]	
	public float rightLimit;
	[Range(1f, 10f)]	
	public float upLimit;
	[Range(1f, 10f)]	
	public float downLimit;
	public float switchSpeed;
	[Range(0f, 1f)]
	public float speed; 

	/** INSTANCES  */
	public static CameraControl instance; 
	private Transform tracking;

	/** DELEGATES */
	public delegate void HorizontalAnchorSwitcher(float left, float right, ref bool switchedH);
	public HorizontalAnchorSwitcher horizontalAnchorSwitcher;
	public delegate void VerticalAnchorSwitcher(float up, float down, ref bool swtichedV);
	public VerticalAnchorSwitcher verticalAnchorSwitcher;
	
	/** STATE */
	private bool init = false; 
	private bool switchedH = false;
	private bool switchedV = false;
	private CameraLimits currrent;
	private Transform camT;
	private Camera cam;

	void Awake() {
		if (instance == null) {
			instance = this;
		}
	}

	public void Init(Transform tracking, HorizontalAnchorSwitcher hSwitcher, VerticalAnchorSwitcher vSwitcher) {
		this.tracking = tracking;
		this.horizontalAnchorSwitcher = hSwitcher;
		this.verticalAnchorSwitcher = vSwitcher;
		this.init = this.tracking != null && this.horizontalAnchorSwitcher != null && verticalAnchorSwitcher != null;
	}

	void Start () {
		cam = GetComponent<Camera>();
		this.currrent =  new CameraLimits(this.leftLimit, this.rightLimit, this.upLimit, this.downLimit);
	}

	private void FixedUpdate() {

		if (!this.init) {
			Debug.LogError("Camera Control has not been completely intialized!");
			return;
		}

		SwitchHorizontalAnchor();
		SwitchVerticalAnchor();

		verticalCameraMovement();
		horizontalCameraMovement();

		MoveCurrentLimits();
	}

	private void horizontalCameraMovement () {

		float minLine = (this.currrent.leftLimit/10f) * this.cam.pixelWidth;
		float maxLine = (this.currrent.rightLimit/10f) * this.cam.pixelWidth;

		Vector3 screenPos = cam.WorldToScreenPoint(this.tracking.position);

		float diff = 0;
		if (screenPos.x > maxLine) {
			diff = screenPos.x - maxLine;
		} else  {
			diff = screenPos.x - minLine;
		}

		Vector3 newPosition = cam.WorldToScreenPoint(this.transform.position) + new Vector3(diff, 0f, 0f);
		this.transform.position = Vector3.Lerp(this.transform.position, cam.ScreenToWorldPoint(newPosition), speed);
	}

	private void verticalCameraMovement () { 
		
		float minLine = (this.currrent.downLimit/10f) * this.cam.pixelHeight;
		float maxLine = (this.currrent.upLimit/10f) * this.cam.pixelHeight;

		Vector3 screenPos = cam.WorldToScreenPoint(this.tracking.position);

		float diff = 0;
		if (screenPos.y > maxLine) {
			diff = screenPos.y - maxLine;
		} else if (screenPos.y < minLine ) {
			diff = screenPos.y - minLine;
		}

		Vector3 newPosition = cam.WorldToScreenPoint(this.transform.position) + new Vector3(0f, diff, 0f);	
		this.transform.position = Vector3.Lerp(this.transform.position, cam.ScreenToWorldPoint(newPosition), speed);
	}

	private void SwitchHorizontalAnchor () {

		CameraLimits cc = this.GetAnchoredLimits();

		float left = (cc.leftLimit/10f) * this.cam.pixelWidth;
		float right = (cc.rightLimit/10f) * this.cam.pixelWidth;

		this.horizontalAnchorSwitcher(left, right, ref this.switchedH);
	}

	private void SwitchVerticalAnchor () {

		CameraLimits cc = this.GetAnchoredLimits();
		
		float down = (cc.downLimit/10f) * this.cam.pixelHeight;
		float up = (cc.upLimit/10f) * this.cam.pixelHeight;
		
		this.verticalAnchorSwitcher(up, down, ref this.switchedV);
	}

	public CameraLimits GetAnchoredLimits() {
		float left = this.switchedH ? 10 - this.rightLimit : this.leftLimit;
		float right = this.switchedH ? 10 - this.leftLimit : this.rightLimit;

		float down  = this.switchedV ? 10 - this.upLimit : this.downLimit;
		float up  = this.switchedV ? 10 - this.downLimit : this.upLimit;

		return new CameraLimits(left, right, up, down);
	}

	public CameraLimits GetCurrentCameraLimits() {
		return this.currrent;
	}

	private void MoveCurrentLimits() {

		CameraLimits cc = this.GetAnchoredLimits();
		
		Vector2 leftRight = Vector2.Lerp(new Vector2(currrent.leftLimit, currrent.rightLimit), new Vector2(cc.leftLimit, cc.rightLimit), this.switchSpeed * Time.deltaTime);
		Vector2 downUp = Vector2.Lerp(new Vector2(currrent.downLimit, currrent.upLimit), new Vector2(cc.downLimit, cc.upLimit), this.switchSpeed * Time.deltaTime);

		this.currrent.leftLimit = leftRight.x;
		this.currrent.rightLimit = leftRight.y;
		this.currrent.upLimit = downUp.y;
		this.currrent.downLimit = downUp.x;
	}

}
