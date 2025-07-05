using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickPlayerExample : MonoBehaviour
{
    public float speed = 5f;
    public VariableJoystick variableJoystick;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>(); // Animator bileşeni alınır
    }

    void Update()
    {
        Vector3 direction = Vector3.forward * variableJoystick.Vertical + Vector3.right * variableJoystick.Horizontal;

        // Hareket kontrolü
        bool isMoving = direction.magnitude >= 0.1f;

        // Animator'a hareket durumu gönderilir
        animator.SetBool("isRunning", isMoving);

        if (isMoving)
        {
            // Hareket ettir
            transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);

            // Yöne dön
            Quaternion toRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, Time.deltaTime * 10f);
        }
    }
}
