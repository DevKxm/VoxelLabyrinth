using UnityEngine;
using UnityEngine.InputSystem;

public class LightManager : MonoBehaviour
{
    // S³oñce ze sceny
    public Light sunLight;

    // Zmienne do podgl¹du w inspektorze
    public float currentLux = 0.0f;
    public string stanGry = "Czekam na sensor...";

    void Start()
    {
        // Sprawdzamy, czy telefon ma czujnik œwiat³a
        if (LightSensor.current != null)
        {
            InputSystem.EnableDevice(LightSensor.current);
            stanGry = "Sensor wykryty!";
        }
        else
        {
            stanGry = "Brak czujnika œwiat³a!";
        }
    }

    void Update()
    {
        // Pobieramy dane tylko jeœli sensor istnieje
        if (LightSensor.current != null)
        {
            // POPRAWKA OSTATECZNA: W³aœciwoœæ nazywa siê .lightLevel
            currentLux = LightSensor.current.lightLevel.ReadValue();

            // LOGIKA GRY:
            // Jeœli jest ciemno (np. mniej ni¿ 50 luksów)
            if (currentLux < 50.0f)
            {
                // Zmiana œwiat³a w grze na ciemne (NOC)
                sunLight.intensity = Mathf.Lerp(sunLight.intensity, 0.2f, Time.deltaTime * 2.0f);
            }
            else
            {
                // Zmiana œwiat³a na jasne (DZIEÑ)
                sunLight.intensity = Mathf.Lerp(sunLight.intensity, 1.5f, Time.deltaTime * 2.0f);
            }
        }
    }
}