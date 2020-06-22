using UnityEngine; 

// movement code applied to the GameObject including jumping and gravity
public class Movement: MonoBehaviour {

    private Rigidbody2D rb; // reference to the rigidbody being controlled
    private float accelerationDuration;
    private float maxAccelerationDuration;
    private bool canJump;
    private bool crossedMaxVThreshold;
    private bool j;
    public float kAcc;
    public float kDec;
    public float kFric;
    public float kAirFric;
    public float maxVerticalSpeed;
    public float maxHorizontalSpeed;
    public float lowJumpMultiplier;
    public float fallMultiplier;
    public float terminalFallMultiplier;
    public float inAirAccMultiplier;
    public bool isTouchingGround = true;

    /** EVENTS */
	public delegate void BlastImpactDelegate(float duration);
    public static event BlastImpactDelegate OnBlastImpact;


    public void Init(Rigidbody2D rb) {
        this.rb = rb;
        this.maxAccelerationDuration = 100f;
    }

    void FixedUpdate() {
        accelerate();   
        jump();
    }

    private void accelerate() {
		float k = this.isTouchingGround ? this.kAcc : this.kAcc * this.inAirAccMultiplier;
        int dir = this.accelerationDuration > 0 ? 1 : -1;

        float newXVel = dir * PlateauCurve.getSpeed(Mathf.Abs(this.accelerationDuration), this.maxHorizontalSpeed, 0, k);
		this.updateVelocity(new Vector2(newXVel, this.rb.velocity.y));
	}

    private void jump() {

		float currentYVel = rb.velocity.y + -Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;

        if (currentYVel > this.maxVerticalSpeed) {
            this.crossedMaxVThreshold = true;
        }

        if (this.j && this.canJump && !this.crossedMaxVThreshold && currentYVel <= this.maxVerticalSpeed) {
			Vector2 newVel = new Vector2(this.rb.velocity.x, (rb.velocity + Vector2.up * -Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime).y);
			this.updateVelocity(newVel);

		} else {

            float fm = Mathf.Abs(this.rb.velocity.y) < 3f ? terminalFallMultiplier : fallMultiplier;
			Vector2 newVel = new Vector2(this.rb.velocity.x, (rb.velocity - Vector2.up * -Physics2D.gravity.y * (fm - 1) * Time.deltaTime).y);
			this.updateVelocity(newVel);
		}
    }

    public void ReceiveJumpInput(bool j, bool canJump) {

        if (this.isTouchingGround) {
            this.crossedMaxVThreshold = false;
        }

        this.canJump = canJump; 
        this.j = j;
	}

    public void ReceiveHorizontalInput(float h) {

		int currentVelDirection = this.accelerationDuration > 0 ? 1 : -1;
		bool opposite = h * currentVelDirection >= 0 ? false : true;
		
		if (h != 0f) {
			float deltaTime = opposite ? Time.deltaTime * this.kDec : Time.deltaTime;

			if (h > 0) this.accelerationDuration = Mathf.Min(this.maxAccelerationDuration, this.accelerationDuration + deltaTime);
			else this.accelerationDuration = Mathf.Max(-this.maxAccelerationDuration, this.accelerationDuration - deltaTime);

		} else if (Mathf.Abs(this.rb.velocity.x) > 0f) {

            float fric = this.isTouchingGround ? this.kFric : this.kAirFric;
            
			if (this.accelerationDuration > 0) this.accelerationDuration = Mathf.Max(0f, this.accelerationDuration - (Time.deltaTime * fric));
			else this.accelerationDuration = Mathf.Min(0f, this.accelerationDuration + (Time.deltaTime * fric));
		}
	}

    private void updateVelocity (Vector2 newVelocity) {
		this.rb.velocity = newVelocity;
	}
}
