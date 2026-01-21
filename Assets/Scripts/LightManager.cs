using UnityEngine;
using UnityEngine.InputSystem;
using TMPro; 

public class LightManager : MonoBehaviour
{
    public Light sunLight;
    public TMP_Text debugText; 

    private float currentLux = 0.0f;

    void Start()
    {
        // W³¹czamy sensor, jeœli istnieje
        if (LightSensor.current != null)
        {
            InputSystem.EnableDevice(LightSensor.current);
        }
    }

    void Update()
    {
        if (LightSensor.current != null)
        {
            // Czytamy wartoœæ œwiat³a
            currentLux = LightSensor.current.lightLevel.ReadValue();

            // WYŒWIETLAMY NA EKRANIE (Diagnostyka)
            if (debugText != null)
            {
                debugText.text = "Œwiat³o: " + currentLux.ToString("F2") + " lx";
            }

            // LOGIKA GRY
            // Próg do 100, bo telefony ró¿nie reaguj¹
            if (currentLux < 50.0f)
            {
                // NOC - œciemniamy s³oñce
                sunLight.intensity = Mathf.Lerp(sunLight.intensity, 0.0f, Time.deltaTime * 3.0f);
            }
            else
            {
                // DZIEÑ - rozjaœniamy s³oñce
                sunLight.intensity = Mathf.Lerp(sunLight.intensity, 1.5f, Time.deltaTime * 3.0f);
            }
        }
        else
        {
            if (debugText != null) debugText.text = "BRAK SENSORA ŒWIAT£A";
        }
    }
}