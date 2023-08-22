﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class Player : MonoBehaviour, ICameraTarget {

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
	private Transform playerTransform;

	public void Initialize () {
		this.playerTransform = GetComponent<Transform>();
		this.rb = this.GetComponent<Rigidbody2D>();
		this.boxCollider = this.GetComponent<BoxCollider2D>();
		this.animator = this.transform.GetChild(0).GetComponent<Animator>();
		this.playerMovement = GetComponent<Movement>();
		this.playerMovement.Init(this.rb);
		this.spriteRenderer = this.transform.GetChild(0).GetComponent<SpriteRenderer>();		
		this.animator.SetBool("IsRunning", false);
	}

	public void Tick() {
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

    public Transform GetTransform()
    {
		return this.playerTransform;
    }

    public int GetFacingValue()
    {
		return this.facing;
    }

    public Vector2 GetVelocity()
    {
		return rb.velocity;
    }

    public Vector2 GetMaxSpeed()
    {
		return new Vector2(playerMovement.maxHorizontalSpeed, playerMovement.maxVerticalSpeed);
    }
}
