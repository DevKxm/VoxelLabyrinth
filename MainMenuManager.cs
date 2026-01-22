using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_InputField nameInput; 
    public GameObject errorText; 

    void Start()
    {
        // Jeœli masz zapisany stary nick, wczytaj go dla wygody
        if (PlayerPrefs.HasKey("PlayerNick"))
        {
            nameInput.text = PlayerPrefs.GetString("PlayerNick");
        }

        if (errorText != null) errorText.SetActive(false);
    }

    public void StartGame()
    {
        // 1. Sprawdzamy czy wpisano nick
        string nick = nameInput.text;

        if (string.IsNullOrEmpty(nick))
        {
            Debug.LogWarning("Musisz wpisaæ nick!");
            if (errorText != null) errorText.SetActive(true);
            return;
        }

        // 2. Zapisujemy nick w pamiêci globalnej (PlayerPrefs)
        PlayerPrefs.SetString("PlayerNick", nick);
        PlayerPrefs.Save();

        // 3. £adujemy scenê z gr¹ (zmieñ "GameScene" na nazwê swojej sceny z labiryntem)
        SceneManager.LoadScene("GameScene");
    }

    public void QuitGame()
    {
        Debug.Log("Wychodzenie z gry...");
        Application.Quit();
    }
}