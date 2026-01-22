using UnityEngine;
using System.Collections; // Potrzebne do Coroutine (odliczania czasu)

public class HealthManager : MonoBehaviour
{
    [Header("Ustawienia")]
    public int maxHealth = 3;
    private int currentHealth;

    // Czas ochronny po uderzeniu (zeby nie tracic 3 serc na raz)
    public float immunityTime = 2.0f;
    private bool isImmune = false; // Czy jestesmy niesmiertelni?

    [Header("UI")]
    public GameObject[] hearts; // Tu przeciagnij PELNE serca

    [Header("Audio")]
    public AudioClip hitSound;
    private AudioSource audioSource;

    void Start()
    {
        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>();

        // Reset serc na starcie
        foreach (GameObject heart in hearts)
        {
            if (heart != null) heart.SetActive(true);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // 1. Sprawdzamy czy to sciana
        // 2. Sprawdzamy czy NIE jestesmy niesmiertelni (isImmune == false)
        if (collision.gameObject.CompareTag("Wall") && !isImmune)
        {
            TakeDamage();
        }
    }

    void TakeDamage()
    {
        // Wlacz niesmiertelnosc
        StartCoroutine(BecomeImmune());

        currentHealth--;
        Debug.Log("A³a! Zosta³o ¿ycia: " + currentHealth);

        // Dzwiek
        if (audioSource != null && hitSound != null)
        {
            audioSource.PlayOneShot(hitSound);
        }

        // Aktualizacja UI (Wylaczamy odpowiednie serce)
        if (currentHealth >= 0 && currentHealth < hearts.Length)
        {
            if (hearts[currentHealth] != null)
            {
                hearts[currentHealth].SetActive(false);
            }
        }

        if (currentHealth <= 0)
        {
            GameOver();
        }
    }

    // To jest licznik czasu niesmiertelnosci
    IEnumerator BecomeImmune()
    {
        isImmune = true;

        // Opcjonalnie: Tutaj mozna dodac miganie gracza (zmienianie koloru)
        GetComponent<MeshRenderer>().material.color = Color.red; // Robi sie czerwony

        yield return new WaitForSeconds(immunityTime); // Czekaj 2 sekundy

        GetComponent<MeshRenderer>().material.color = Color.white; // Wraca do normy
        isImmune = false;
    }

    void GameOver()
    {
        Debug.Log("GAME OVER - Zapisywanie wyniku...");

        string nick = PlayerPrefs.GetString("PlayerNick", "Anonim");

        GameTimer timerScript = FindObjectOfType<GameTimer>();
        float wynikCzasowy = 0f;
        string ladnyCzas = "00:00:00";

        if (timerScript != null)
        {
            timerScript.StopTimer();
            wynikCzasowy = timerScript.GetFinalTime();
            ladnyCzas = timerScript.GetFormattedTime();
        }

        FirebaseManager firebase = FindObjectOfType<FirebaseManager>();

        // Zabezpieczenie przed szybka smiercia zanim Firebase wstanie
        if (firebase != null)
        {
            // Próbujemy wys³aæ - skrypt Firebase sam sprawdzi czy jest gotowy
            firebase.SaveScore(nick, wynikCzasowy, ladnyCzas);
        }

        // Zniszcz gracza
        Destroy(gameObject);
    }
}