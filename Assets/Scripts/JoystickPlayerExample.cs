using UnityEngine;

public class JoystickPlayerExample : MonoBehaviour
{
    public float speed = 5f;
    public VariableJoystick variableJoystick;
    public Transform cameraTransform; // 🎯 Kamerayı buraya bağlayacağız

    private Animator animator;
    private PlayerInteraction interaction;
    private Rigidbody rb;

    void Start()
    {
        animator = GetComponent<Animator>();
        interaction = GetComponent<PlayerInteraction>();
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // 1️⃣ Joystick yönünü al
        Vector2 input = new Vector2(variableJoystick.Horizontal, variableJoystick.Vertical);

        // 2️⃣ Kamera yönüne göre dönüştür
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0;
        camRight.y = 0;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 direction = camForward * input.y + camRight * input.x;

        bool isMoving = direction.magnitude >= 0.1f;
        bool isCarrying = interaction != null && interaction.IsHoldingItem();

        // 3️⃣ Animasyon
        animator.SetBool("isRunning", isMoving);
        animator.SetBool("isCarrying", isCarrying);

        // 4️⃣ Hareket ettir
        if (isMoving)
        {
            Vector3 move = direction.normalized * speed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + move);

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 10f);
        }
    }
}
