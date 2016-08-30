using UnityEngine;
using System.Collections;

public class PlayableScript : MonoBehaviour {

    public float gravity = -5.0f;
    private float yVelocity = 0.0f;

    private CharacterController characterController = null;
    public float moveSpeed = 1.0f;
    private Animator anim;


    void Start()
    {
        characterController = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();

        anim.Play("idle");
    }

	void Update ()
    {
        CheckNearCandle();

        Vector3 moveDirection = new Vector3();

        yVelocity += (gravity * Time.deltaTime);
        moveDirection.y = yVelocity;

        characterController.Move(moveDirection * Time.deltaTime);

        if (characterController.collisionFlags == CollisionFlags.Below)
            yVelocity = 0.0f;
    }

    void OnCollisionEnter(Collision col)
    {
        Debug.Log("collider enter");
        if(col.collider.gameObject.layer == 15)
            col.gameObject.GetComponent<PlateLinkingScript>().SetTriggerOn();

        if (col.collider.gameObject.layer == 16)
            Debug.Log("Damaged");
    }

    void CheckNearCandle()
    {
        Collider[] colliders;
        if ((colliders = Physics.OverlapSphere(transform.position, 1f)).Length > 1)
        {
            foreach (var collider in colliders)
            {
                var go = collider.gameObject;
                if (go.layer != 14)
                    continue;

                go.GetComponent<CandleScript>().SetLightOn();
            }
        }
    }
}
