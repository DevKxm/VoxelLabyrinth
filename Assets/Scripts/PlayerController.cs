using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 10.0f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // WA¯NE: ¯yroskop trzeba rêcznie w³¹czyæ, inaczej nie zadzia³a!
        // Akcelerometr dzia³a zawsze, ¿yroskop domyœlnie jest wy³¹czony, by oszczêdzaæ bateriê.
        if (SystemInfo.supportsGyroscope)
        {
            Input.gyro.enabled = true;
        }
    }

    void FixedUpdate()
    {
        // Sprawdzamy, czy ¿yroskop jest dostêpny, ¿eby nie wywali³o b³êdu na komputerze
        if (SystemInfo.supportsGyroscope)
        {
            // Input.gyro.gravity to wektor grawitacji wyliczony przy pomocy ¯YROSKOPU (jest stabilniejszy ni¿ sam akcelerometr)
            float moveHorizontal = Input.gyro.gravity.x;
            float moveVertical = Input.gyro.gravity.y;

            Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
            rb.AddForce(movement * speed);
        }
    }
}