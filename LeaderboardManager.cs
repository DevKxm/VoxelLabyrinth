using UnityEngine;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections.Generic;

public class LeaderboardManager : MonoBehaviour
{
    public GameObject rowPrefab;  
    public Transform contentArea; 
    public GameObject loadingIndicator; // Opcjonalnie napis "£adowanie..."

    // URL (musi byæ ten sam co w FirebaseManager!)
    private const string DATABASE_URL = "https://voxellabyrinth-default-rtdb.europe-west1.firebasedatabase.app/";
    private DatabaseReference reference;

    void Start()
    {
        reference = FirebaseDatabase.GetInstance(DATABASE_URL).RootReference;

        LoadLeaderboard();
    }

    public void LoadLeaderboard()
    {
        if (loadingIndicator != null) loadingIndicator.SetActive(true);

        // 1. Wyczyœci star¹ listê (¿eby nie dublowaæ wyników)
        foreach (Transform child in contentArea)
        {
            Destroy(child.gameObject);
        }

        // 2. Pobierze dane z Firebase
        // OrderByChild("rawTime") - sortuje od najmniejszego czasu (najszybszy wygrywa)
        // LimitToFirst(10) - bierze tylko TOP 10
        reference.Child("wyniki").OrderByChild("rawTime").LimitToFirst(10).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("B³¹d Firebase: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                // 3. Iteruje po wynikach
                int rank = 1;

                // Firebase mo¿e zwróciæ wyniki w odwrotnej kolejnoœci przy sortowaniu,
                // ale przy OrderByChild zazwyczaj jest rosn¹co (co nam pasuje dla czasu).
                foreach (DataSnapshot score in snapshot.Children)
                {
                    // Wyci¹gamy dane z JSONa
                    string nick = score.Child("nick").Value.ToString();
                    string time = score.Child("displayTime").Value.ToString();

                    // Tworzymy wiersz w tabeli
                    CreateRow(rank, nick, time);
                    rank++;
                }
            }

            if (loadingIndicator != null) loadingIndicator.SetActive(false);
        });
    }

    void CreateRow(int rank, string nick, string time)
    {
        // Tworzymy obiekt z prefabrykatu wewn¹trz Content
        GameObject newRow = Instantiate(rowPrefab, contentArea);

        // Ustawiamy dane w skrypcie ScoreRow
        ScoreRow rowScript = newRow.GetComponent<ScoreRow>();
        if (rowScript != null)
        {
            rowScript.SetData(rank, nick, time);
        }
    }
}