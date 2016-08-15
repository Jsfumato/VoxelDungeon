using UnityEngine;
using System.Collections;

public class PlayableScript : MonoBehaviour {

    public float gravity = -5.0f;
    private float yVelocity = 0.0f;

    private Transform transform = null;
    private CharacterController characterController = null;
    public float moveSpeed = 1.0f;

    void Start()
    {
        transform = GetComponent<Transform>();
        characterController = GetComponent<CharacterController>();
    }

	void Update ()
    {
        Vector3 moveDirection = new Vector3();

        yVelocity += (gravity * Time.deltaTime);
        moveDirection.y = yVelocity;

        characterController.Move(moveDirection * Time.deltaTime);

        if (characterController.collisionFlags == CollisionFlags.Below)
            yVelocity = 0.0f;
    }

    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.layer == 12)
        {
            Debug.LogWarning("BOOM!");
        }
    }
}
