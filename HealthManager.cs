using UnityEngine;
using UnityEngine.SceneManagement;

public class HealthManager : MonoBehaviour
{
    public int health = 3;
    public GameObject[] heartsUI; // Tablica obrazków serc w Canvasie (mo¿esz przypisaæ 3 obrazki)

    // Referencje
    private GameTimer gameTimer;
    private PlayerController playerController;

    void Start()
    {
        gameTimer = FindObjectOfType<GameTimer>();
        playerController = GetComponent<PlayerController>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Pamiêtaj o dodaniu tagu "Wall" do œcian!
        if (collision.gameObject.CompareTag("Wall"))
        {
            TakeDamage();
        }
    }

    void TakeDamage()
    {
        health--;

        // Wy³¹czamy serduszko w UI (np. ostatnie dostêpne)
        if (health >= 0 && health < heartsUI.Length)
        {
            heartsUI[health].SetActive(false);
        }

        Debug.Log("Ouch! ¯ycia: " + health);

        if (health <= 0)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        Debug.Log("GAME OVER");

        // 1. Zatrzymaj czas
        if (gameTimer != null) gameTimer.StopTimer();

        // 2. Zatrzymaj gracza
        if (playerController != null) playerController.enabled = false;
        GetComponent<Rigidbody>().isKinematic = true; // Ca³kowite zamro¿enie fizyki

        // 3. Zapisz wynik do Firebase
        SaveScoreToDatabase();

        // 4. Tutaj mo¿esz pokazaæ Panel Game Over (z przyciskiem Restart/Menu)
        // np. gameOverPanel.SetActive(true);
    }

    void SaveScoreToDatabase()
    {
        // Pobieramy nick, który wpisaliœmy w Menu
        string nick = PlayerPrefs.GetString("PlayerNick", "Anonim");

        // Pobieramy czas
        float finalTime = gameTimer != null ? gameTimer.GetFinalTime() : 0f;
        string formattedTime = gameTimer != null ? gameTimer.GetFormattedTime() : "00:00";

        // Wywo³ujemy Twój skrypt Firebase (musimy tam dodaæ tê metodê!)
        // Zak³adam, ¿e masz FirebaseManager na scenie
        FirebaseManager fb = FindObjectOfType<FirebaseManager>();
        if (fb != null)
        {
            fb.SaveScore(nick, finalTime, formattedTime);
        }
        else
        {
            Debug.LogError("Brak FirebaseManager na scenie!");
        }
    }
}