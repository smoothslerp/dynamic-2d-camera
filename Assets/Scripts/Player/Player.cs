﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	/** STATE */
	private int facing = 1;
	[HideInInspector]
	private bool canJump = false;
	private bool isJumping = false;

	/** COMPONENTS */
	private Rigidbody2D rb;
	private SpriteRenderer spriteRenderer;
	private BoxCollider2D boxCollider;
	private Animator animator;
	private Movement playerMovement;

	void Start () {
		this.rb = this.GetComponent<Rigidbody2D>();
		this.boxCollider = this.GetComponent<BoxCollider2D>();
		this.animator = this.transform.GetChild(0).GetComponent<Animator>();
		
		this.playerMovement = GetComponent<Movement>();
		this.playerMovement.Init(this.rb);

		this.spriteRenderer = this.transform.GetChild(0).GetComponent<SpriteRenderer>();

		CameraControl.instance.Init(this.transform, this.HorizontalAnchorSwitcher, this.VerticalAnchorSwitcher);
		
		this.animator.SetBool("IsRunning", false);
	}

	void Update() {
		jumpInput();
		
		this.ChangeFaceDirection(PlayerControl.instance.horizontalInput);
		this.animator.SetBool("IsRunning", Mathf.Abs(PlayerControl.instance.horizontalInput) > 0.1f);

		this.playerMovement.ReceiveJumpInput(this.isJumping, this.canJump);
		this.playerMovement.ReceiveHorizontalInput(PlayerControl.instance.horizontalInput);
	}

	private void jumpInput() {
		if (PlayerControl.instance.jumpPressed) {
			isJumping = true;
		}

		if (PlayerControl.instance.jumpReleased) {
			isJumping = false;
		}

		if (!this.playerMovement.isTouchingGround && (PlayerControl.instance.jumpReleased || this.rb.velocity.y < 0f)) {
			this.canJump = false;
		}

		if (this.playerMovement.isTouchingGround) {
			if (!PlayerControl.instance.jumpPressed) this.canJump = true;
		}
	}
	
	public void ChangeFaceDirection(float horizontalInput) {
		
		if (horizontalInput == 0) return;
		
		this.facing = horizontalInput > 0 ? 1 : -1;
		this.spriteRenderer.flipX = facing < 0 ? true : false;
	}
	
	// Passed as delegate to camera control
	public void HorizontalAnchorSwitcher(float left, float right, ref bool switchedH) {
		Vector3 playerScreenPos = Camera.main.WorldToScreenPoint(this.transform.position);
		
		if (this.facing < 0 && Mathf.Abs(this.rb.velocity.x) >= (this.playerMovement.maxHorizontalSpeed/2f)) {
			switchedH = true;
		}
		
		if (this.facing > 0 && Mathf.Abs(this.rb.velocity.x) >= this.playerMovement.maxHorizontalSpeed/2f) {
			switchedH = false;
		}
	}

	// Passed as delegate to camera control	
	public void VerticalAnchorSwitcher(float up, float down, ref bool switchedV) {
		float mid = (up + down)/2f;

		Vector3 playerScreenPos = Camera.main.WorldToScreenPoint(this.transform.position);
		
		if (playerScreenPos.y < mid && Mathf.Abs(this.rb.velocity.y) >= this.playerMovement.maxVerticalSpeed) { // on the way down, want to see down
			switchedV = true;
		}
		
		if (playerScreenPos.y > mid && Mathf.Abs(this.rb.velocity.y) >= this.playerMovement.maxVerticalSpeed/4f) { // on the way up, want to see up
			switchedV = false;
		}
	}

}
