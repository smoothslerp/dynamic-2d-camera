using UnityEngine;

public class BottomTouch : MonoBehaviour {

    private Movement movement; 

    void Start() {
        this.movement = this.transform.parent.GetComponent<Movement>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.transform.tag == "Platform") {
            this.movement.isTouchingGround = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.transform.tag == "Platform") {
            this.movement.isTouchingGround = false;
        }
    }
    
}
