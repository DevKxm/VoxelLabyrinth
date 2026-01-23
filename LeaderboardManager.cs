using UnityEngine;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections.Generic;

public class LeaderboardManager : MonoBehaviour
{
    public GameObject rowPrefab;
    public Transform contentArea;
    public GameObject loadingIndicator;

    // URL Datebase
    private const string DATABASE_URL = "https://voxellabyrinth-default-rtdb.europe-west1.firebasedatabase.app/";
    private DatabaseReference reference;

    void Start()
    {
        reference = FirebaseDatabase.GetInstance(DATABASE_URL).RootReference;
        // Nie wywolujemy LoadLeaderboard w Start, bo robimy to przyciskiem w Menu
        // Ale jesli chcesz testowac bez klikania, mozesz odkomentowac:
        // LoadLeaderboard();
    }

    public void LoadLeaderboard()
    {
        if (loadingIndicator != null) loadingIndicator.SetActive(true);

        // 1. Czyszczenie listy
        foreach (Transform child in contentArea)
        {
            Destroy(child.gameObject);
        }

        // 2. Pobieranie danych
        reference.Child("wyniki").OrderByChild("timeRaw").LimitToFirst(10).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("B³¹d Firebase: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                if (!snapshot.HasChildren)
                {
                    Debug.Log("Brak wyników w bazie!");
                }

                int rank = 1;
                foreach (DataSnapshot score in snapshot.Children)
                {

                    // 1. Pobiera Nick
                    string nick = "Nieznany";
                    if (score.Child("username").Exists)
                        nick = score.Child("username").Value.ToString();

                    // 2. Pobiera Czas
                    string time = "--:--";
                    if (score.Child("timeString").Exists)
                        time = score.Child("timeString").Value.ToString();

                    // Tworzt wiersz
                    CreateRow(rank, nick, time);
                    rank++;
                }
            }

            if (loadingIndicator != null) loadingIndicator.SetActive(false);
        });
    }

    void CreateRow(int rank, string nick, string time)
    {
        GameObject newRow = Instantiate(rowPrefab, contentArea);
        ScoreRow rowScript = newRow.GetComponent<ScoreRow>();
        if (rowScript != null)
        {
            rowScript.SetData(rank, nick, time);
        }
    }
}