﻿using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
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
	
	/** STATE */
	private bool switchedH = false;
	private bool switchedV = false;
	private CameraLimits current;

    private Camera mainCamera;
	private Transform cameraTransform;
	private Vector2 horizontalNewPosition;
	private Vector2 verticalNewPosition;
	private Vector3 screenPosition;
	private Vector3 horizontalDiff;
	private Vector3 verticalDiff;
	private ICameraTarget cameraTarget;

	public void PreInitialize() {
		mainCamera = GetComponent<Camera>();
		cameraTransform = GetComponent<Transform>();
		horizontalDiff = Vector3.zero;
		verticalDiff = Vector3.zero;
	}

	public void InjectCameraTarget(ICameraTarget cameraTarget) {
		this.cameraTarget = cameraTarget;
	}

	public void Initialize () {
		if (this.leftLimit > this.rightLimit) {
			float temp = this.leftLimit;
			this.leftLimit = this.rightLimit;
			this.rightLimit = temp;
		}

		if (this.downLimit > this.upLimit) {
			float temp = this.downLimit;
			this.downLimit = this.upLimit;
			this.upLimit = temp;
		}

		this.current = new CameraLimits(this.leftLimit, this.rightLimit, this.upLimit, this.downLimit);
	}

	public void FixedTick() {

		if (this.cameraTarget == null) {
			Debug.LogError("Camera Control has not been completely intialized!");
			return;
		}
		moveVertically();
		moveHorizontally();

		SwitchHorizontalAnchor();
		SwitchVerticalAnchor();

		MoveCurrentLimits();
	}

	private void moveHorizontally () {
		
		float minLine = this.current.leftLimit * mainCamera.pixelWidth;
		float maxLine = this.current.rightLimit * mainCamera.pixelWidth;

		screenPosition = mainCamera.WorldToScreenPoint(this.cameraTarget.GetTransform().position);

		float diff = 0;
		if (screenPosition.x > maxLine) {
			diff = screenPosition.x - maxLine;
		} else if (screenPosition.x < minLine) {
			diff = screenPosition.x - minLine;
		} else return;

		horizontalDiff.x = diff;
		horizontalNewPosition = mainCamera.WorldToScreenPoint(this.cameraTransform.position) + horizontalDiff;
		this.cameraTransform.position = Vector3.Lerp(this.cameraTransform.position, mainCamera.ScreenToWorldPoint(horizontalNewPosition), this.speed);
	}

	private void moveVertically () { 
		
		float minLine = this.current.downLimit * mainCamera.pixelHeight;
		float maxLine = this.current.upLimit * mainCamera.pixelHeight;

		screenPosition = mainCamera.WorldToScreenPoint(this.cameraTarget.GetTransform().position);

		float diff = 0;
		if (screenPosition.y > maxLine) {
			diff = screenPosition.y - maxLine;
		} else if (screenPosition.y < minLine) {
			diff = screenPosition.y - minLine;
		} else return;

		verticalDiff.y = diff;
		verticalNewPosition = mainCamera.WorldToScreenPoint(this.cameraTransform.position) + verticalDiff;	
		this.cameraTransform.position = Vector3.Lerp(this.cameraTransform.position, mainCamera.ScreenToWorldPoint(verticalNewPosition), this.speed);
	}

	private void SwitchHorizontalAnchor () {

		var targetVelocity = this.cameraTarget.GetVelocity();
		var targetMaxSpeed = this.cameraTarget.GetMaxSpeed();

		if (cameraTarget.GetFacingValue() < 0 && Mathf.Abs(targetVelocity.x) >= targetMaxSpeed.x * .5f)
		{
			switchedH = true;
		}

		if (cameraTarget.GetFacingValue() > 0 && Mathf.Abs(targetVelocity.x) >= targetMaxSpeed.x * .5f)
		{
			switchedH = false;
		}
	}

	private void SwitchVerticalAnchor () {

		CameraLimits cc = this.GetAnchoredLimits();
		
		float down = cc.downLimit * mainCamera.pixelHeight;
		float up = cc.upLimit * mainCamera.pixelHeight;
		float mid = (up + down) * .5f;

		Vector3 targetScreenPosition = Camera.main.WorldToScreenPoint(this.cameraTarget.GetTransform().position);
		var targetVelocity = this.cameraTarget.GetVelocity();
		var targetMaxSpeed = this.cameraTarget.GetMaxSpeed();

		if (targetScreenPosition.y < mid && Mathf.Abs(targetVelocity.y) >= targetMaxSpeed.y)
		{ // on the way down, want to see down
			switchedV = true;
		}

		if (targetScreenPosition.y > mid && Mathf.Abs(targetVelocity.y) >= targetMaxSpeed.y * .25f)
		{ // on the way up, want to see up
			switchedV = false;
		}
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
