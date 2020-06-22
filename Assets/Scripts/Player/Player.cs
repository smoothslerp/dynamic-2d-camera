﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	/** STATE */
	private int facing = 1;
	[HideInInspector]
	private bool canJump = false;
	private bool canShoot = true;
	private bool isJumping = false;
	private float accelerationDuration = 0f;

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

		this.animator.SetBool("IsRunning", false);
		this.spriteRenderer = this.transform.GetChild(0).GetComponent<SpriteRenderer>();
	}

	void Update() {
		handleInputs();
	}

	void handleInputs () {
		jumpInput();
		
		this.ChangeFaceDirection(PlayerControl.instance.horizontalInput);
		this.animator.SetBool("IsRunning", this.playerMovement.isTouchingGround && Mathf.Abs(PlayerControl.instance.horizontalInput) > 0.1f);

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

		// when in the air, should not be able to jump again
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


	public float GetHorizontalSpeed() {
		return this.rb.velocity.x;
	}

	public float GetVerticalSpeed() {
		return this.rb.velocity.y;
	}

	public PlayerState GetState() {
		PlayerState state = new PlayerState();

		state.accelerationDuration = this.accelerationDuration;
		state.facing = this.facing;

		return state;
	}

	public Movement GetMovement() {
		return this.playerMovement;
	}

}
