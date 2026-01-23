using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Ustawienia ruchu")]
    public float speed = 10.0f;          // Sterowanie (lewo/prawo/przód/ty³)
    public float autoMoveSpeed = 5.0f;   // Sta³a si³a pchaj¹ca do przodu

    [Header("Ustawienia Odbicia")]
    public float bounceForce = 5.0f;     // Si³a odrzutu od œciany

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (SystemInfo.supportsGyroscope)
        {
            Input.gyro.enabled = true;
        }
    }

    void FixedUpdate()
    {
        // 1. Sterowanie ¯yroskopem
        if (SystemInfo.supportsGyroscope)
        {
            float moveHorizontal = Input.gyro.gravity.x;
            float moveVertical = Input.gyro.gravity.y;

            Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
            rb.AddForce(movement * speed);
        }
        // Opcjonalnie sterowanie klawiatur¹ do testów na PC
        else
        {
            float moveH = Input.GetAxis("Horizontal");
            float moveV = Input.GetAxis("Vertical");
            Vector3 movement = new Vector3(moveH, 0.0f, moveV);
            rb.AddForce(movement * speed);
        }

        // 2. Automatyczny ruch do przodu (Globalny Z)
        // Pchamy tylko wtedy, gdy prêdkoœæ nie jest zerowa (¿eby nie pchaæ w œcianê na si³ê, gdy stoimy)
        // Ale dla uproszczenia w labiryncie: pchamy zawsze lekko do przodu
        rb.AddForce(Vector3.forward * autoMoveSpeed);
    }

    // --- (ODBICIE) ---
    void OnCollisionEnter(Collision collision)
    {
        // Jeœli uderzyliœmy w coœ co ma tag "Wall"
        if (collision.gameObject.CompareTag("Wall"))
        {
            // Oblicz wektor odbicia (normalna œciany mówi nam, w któr¹ stronê jest "od œciany")
            Vector3 normal = collision.contacts[0].normal;

            // Dodajemy nag³¹ si³ê (Impulse) w kierunku "od œciany"
            // To przebije si³ê autoMoveSpeed i pozwoli kulce odskoczyæ
            rb.AddForce(normal * bounceForce, ForceMode.Impulse);

            Debug.Log("Odbicie od œciany!");
        }
    }
}