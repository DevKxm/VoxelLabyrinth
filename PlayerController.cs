using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Ustawienia ruchu")]
    public float speed = 10.0f; // Szybkosc sterowania
    public float autoMoveSpeed = 2.0f; // Sta³a si³a pchaj¹ca kulkê do przodu

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // W³¹cz ¿yroskop
        if (SystemInfo.supportsGyroscope)
        {
            Input.gyro.enabled = true;
        }
    }

    void FixedUpdate()
    {
        // 1. Sterowanie ¿yroskopem (gracz)
        if (SystemInfo.supportsGyroscope)
        {
            float moveHorizontal = Input.gyro.gravity.x;
            float moveVertical = Input.gyro.gravity.y;

            // Wektor sterowania od gracza
            Vector3 userMovement = new Vector3(moveHorizontal, 0.0f, moveVertical);
            rb.AddForce(userMovement * speed);
        }

        // 2. Automatyczny ruch do przodu 
        // Kulka zawsze bedzie delikatnie pchana w osi Z (do gory ekranu)
        Vector3 constantForce = new Vector3(0.0f, 0.0f, 1.0f);
        rb.AddForce(constantForce * autoMoveSpeed);
    }
}