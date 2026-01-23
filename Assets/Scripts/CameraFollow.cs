using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;       // Obiekt, za ktorym podaza kamera - kulka
    public float smoothSpeed = 0.125f; // Jak plynnnie kamera ma "dociagac siê" (0-1)
    public Vector3 offset;         // Przesuniecie kamery wzgledem gracza

    void Start()
    {

        offset = transform.position - target.position;
    }

    void LateUpdate()
    {
        if (target != null)
        {
            // Gdzie ma byæ: Pozycja gracza + nasze przesuniecie
            Vector3 desiredPosition = target.position + offset;

            // Plynne przejscie z obecnej pozycji do docelowej (Smooth Damping)
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

            // Aktualizacja pozycji
            transform.position = smoothedPosition;
        }
    }
}