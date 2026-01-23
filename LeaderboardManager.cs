using UnityEngine;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections.Generic;
using System.Linq; // Odwracanie listy!

public class LeaderboardManager : MonoBehaviour
{
    public GameObject rowPrefab;
    public Transform contentArea;
    public GameObject loadingIndicator;

    private const string DATABASE_URL = "https://voxellabyrinth-default-rtdb.europe-west1.firebasedatabase.app/";
    private DatabaseReference reference;

    void Start()
    {
        reference = FirebaseDatabase.GetInstance(DATABASE_URL).RootReference;
    }

    public void LoadLeaderboard()
    {
        if (loadingIndicator != null) loadingIndicator.SetActive(true);

        foreach (Transform child in contentArea)
        {
            Destroy(child.gameObject);
        }

        // ZMIANA 1: LimitToLast(10), ¿eby wzi¹æ NAJWIÊKSZE czasy
        // Jeœli  najszybsze (ma³e) czasy, czyli odwrotnie, to trzeba zostawiæ LimitToFirst.

        reference.Child("wyniki").OrderByChild("timeRaw").LimitToLast(10).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                // ZMIANA 2: Wrzucamy wyniki do listy i odwracamy kolejnoœæ
                List<DataSnapshot> allScores = new List<DataSnapshot>();
                foreach (DataSnapshot child in snapshot.Children)
                {
                    allScores.Add(child);
                }

                // Odwracamy listê (Najwiêkszy czas bêdzie pierwszy)
                allScores.Reverse();

                int rank = 1;
                foreach (DataSnapshot score in allScores)
                {
                    string nick = "Nieznany";
                    if (score.Child("username").Exists)
                        nick = score.Child("username").Value.ToString();

                    string time = "--:--";
                    if (score.Child("timeString").Exists)
                        time = score.Child("timeString").Value.ToString();

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