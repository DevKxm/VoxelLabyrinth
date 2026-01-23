using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Do ³adowania scen
using TMPro; // Jeœli u¿ywa siê TMP do wyœwietlania wyniku

public class HealthManager : MonoBehaviour
{
    public int maxHealth = 3;
    public int currentHealth;

    [Header("UI Reference")]
    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    [Header("Game Over UI")]
    public GameObject gameOverPanel; // Tutaj swój GameOverPanel z Canvasa
    public LeaderboardManager leaderboardManager; // Tutaj obiekt Managers ze sceny

    private bool isImmune = false;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHearts();

        // Na starcie ekran koñcowy MUSI byæ ukryty
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall") && !isImmune && !isDead)
        {
            TakeDamage();
        }
    }

    void TakeDamage()
    {
        if (isDead) return;

        currentHealth--;
        UpdateHearts();

        if (currentHealth <= 0)
        {
            GameOver();
        }
        else
        {
            StartCoroutine(BecomeImmune());
        }
    }

    void UpdateHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < currentHealth) hearts[i].sprite = fullHeart;
            else hearts[i].sprite = emptyHeart;
        }
    }

    System.Collections.IEnumerator BecomeImmune()
    {
        isImmune = true;
        // Opcjonalnie: Migalnie lub zmiana koloru gracza, tutaj czerwony
        if (GetComponent<MeshRenderer>() != null)
            GetComponent<MeshRenderer>().material.color = Color.red;

        yield return new WaitForSeconds(2.0f);

        if (GetComponent<MeshRenderer>() != null)
            GetComponent<MeshRenderer>().material.color = Color.white;
        isImmune = false;
    }

    void GameOver()
    {
        isDead = true;
        Debug.Log("GAME OVER!");

        // 1. ZATRZYMAJ GRACZA
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        PlayerController pc = GetComponent<PlayerController>();
        if (pc != null) pc.enabled = false;

        // 2. ZATRZYMAJ CZAS I POBIERZ WYNIK
        GameTimer timer = FindObjectOfType<GameTimer>();
        float finalTime = 0f;
        string displayTime = "00:00:00";

        if (timer != null)
        {
            timer.StopTimer();
            finalTime = timer.GetFinalTime();
            displayTime = timer.GetFormattedTime();
        }

        // 3. ZAPISZ WYNIK W FIREBASE
        string nick = PlayerPrefs.GetString("PlayerNick", "Anonim");
        FirebaseManager firebase = FindObjectOfType<FirebaseManager>();

        if (firebase != null)
        {
            firebase.SaveScore(nick, finalTime, displayTime);
        }

        // 4. POKA¯ EKRAN KOÑCOWY
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);

            // Opcjonalnie: Od razu za³aduj tabelê wyników (jeœli jest widoczna na ekranie)
            if (leaderboardManager != null)
            {
                // Czekamy chwilkê (np. 1s) ¿eby Firebase zd¹¿y³ zapisaæ nasz nowy wynik zanim go pobierzemy
                Invoke("RefreshLeaderboard", 1.0f);
            }
        }
    }

    // Metoda pomocnicza do odœwie¿enia tabeli z opóŸnieniem
    void RefreshLeaderboard()
    {
        if (leaderboardManager != null) leaderboardManager.LoadLeaderboard();
    }

    // --- FUNKCJE DLA PRZYCISKÓW ---

    public void RestartGame()
    {
        // £aduje ponownie aktualn¹ scenê
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        // Nazwa sceny w Build Settings musi byæ dok³adnie "MainMenu"
        SceneManager.LoadScene("MainMenu");
    }
}