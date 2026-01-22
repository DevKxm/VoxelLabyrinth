using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public TMP_Text timerText; // Przeci¹gnij tekst z UI w grze

    private bool isRunning = false;
    private float elapsedTime = 0f;

    void Start()
    {
        // Startujemy licznik od razu po za³adowaniu sceny
        isRunning = true;
        elapsedTime = 0f;
    }

    void Update()
    {
        if (isRunning)
        {
            elapsedTime += Time.deltaTime;
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        if (timerText != null)
        {
            // Format 00:00:00
            int minutes = Mathf.FloorToInt(elapsedTime / 60F);
            int seconds = Mathf.FloorToInt(elapsedTime % 60F);
            int milliseconds = Mathf.FloorToInt((elapsedTime * 100F) % 100F);
            timerText.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
        }
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    // Metoda, ¿eby inne skrypty mog³y pobraæ koñcowy czas
    public float GetFinalTime()
    {
        return elapsedTime;
    }

    // Metoda pomocnicza do ³adnego wyœwietlania czasu w tabeli wyników
    public string GetFormattedTime()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60F);
        int seconds = Mathf.FloorToInt(elapsedTime % 60F);
        int milliseconds = Mathf.FloorToInt((elapsedTime * 100F) % 100F);
        return string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
    }
}