using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickPlayerExample : MonoBehaviour
{
    public float speed = 5f;
    public VariableJoystick variableJoystick;
    private Animator animator;
    private PlayerInteraction interaction;

    void Start()
    {
        animator = GetComponent<Animator>();
        interaction = GetComponent<PlayerInteraction>();
    }

    void Update()
    {
        Vector3 direction = Vector3.forward * variableJoystick.Vertical + Vector3.right * variableJoystick.Horizontal;

        bool isMoving = direction.magnitude >= 0.1f;
        bool isCarrying = interaction != null && interaction.IsHoldingItem();

        animator.SetBool("isRunning", isMoving);
        animator.SetBool("isCarrying", isCarrying);

        if (isMoving)
        {
            transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);

            Quaternion toRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, Time.deltaTime * 10f);
        }
    }
}
