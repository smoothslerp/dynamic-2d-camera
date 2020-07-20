using UnityEngine;

public class BottomTouch : MonoBehaviour {

    private Movement movement; 
    private Transform playerTransform;
    private BoxCollider2D boxCollider; 
    public LayerMask layerMask;
    private Player player;

    void Start() {
        this.playerTransform = this.transform.parent;
        this.movement = this.playerTransform.GetComponent<Movement>();
        this.boxCollider = this.playerTransform.GetComponent<BoxCollider2D>();
        this.player = this.playerTransform.GetComponent<Player>();
    }

    void Update() {
        float y = this.boxCollider.size.y;
        float x = this.boxCollider.size.x;
        RaycastHit2D hitDownLeft = Physics2D.Raycast(playerTransform.position + new Vector3(-x/2, 0f, 0f), -playerTransform.up, (3*y)/4, layerMask);
        RaycastHit2D hitDownRight = Physics2D.Raycast(playerTransform.position + new Vector3(x/2, 0f, 0f), -playerTransform.up, (3*y)/4, layerMask);
        Debug.DrawRay(playerTransform.position + new Vector3(-x/2, 0f, 0f), -playerTransform.up * (3*y)/4, Color.red);
        Debug.DrawRay(playerTransform.position + new Vector3(x/2, 0f, 0f), -playerTransform.up * (3*y)/4, Color.green);

        this.movement.isTouchingGround = hitDownLeft.collider != null || hitDownRight.collider != null;

    }
}
