using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections; 

public class FirebaseManager : MonoBehaviour
{
    private DatabaseReference reference;
    private bool isFirebaseReady = false;

    // WA¯NE: Adres Twojej bazy w Europie
    private const string DATABASE_URL = "https://voxellabyrinth-default-rtdb.europe-west1.firebasedatabase.app/";

    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("B³¹d Firebase: " + dependencyStatus);
            }
        });
    }

    void InitializeFirebase()
    {
        try
        {
            // Tutaj u¿ywamy konkretnego URL zamiast DefaultInstance
            reference = FirebaseDatabase.GetInstance(DATABASE_URL).RootReference;
            isFirebaseReady = true;
            Debug.Log("Firebase po³¹czony i gotowy (Europa)!");
        }
        catch (System.Exception e)
        {
            Debug.LogError("B³¹d inicjalizacji bazy: " + e.Message);
        }
    }

    public void SaveScore(string nick, float rawTime, string displayTime)
    {
        if (!isFirebaseReady || reference == null)
        {
            Debug.LogWarning("Firebase nie gotowy, próbuje wys³aæ za chwilê...");
            StartCoroutine(WaitForFirebaseAndSend(nick, rawTime, displayTime));
            return;
        }

        SendToDatabase(nick, rawTime, displayTime);
    }

    System.Collections.IEnumerator WaitForFirebaseAndSend(string n, float r, string d)
    {
        float timer = 0;
        // Czekaj max 5 sekund
        while (!isFirebaseReady && timer < 5.0f)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        if (isFirebaseReady)
        {
            SendToDatabase(n, r, d);
        }
        else
        {
            Debug.LogError("Timeout! Firebase nie po³¹czy³ siê w 5 sekund.");
        }
    }

    void SendToDatabase(string nick, float rawTime, string displayTime)
    {
        UserScore scoreData = new UserScore(nick, rawTime, displayTime);
        string json = JsonUtility.ToJson(scoreData);
        string key = reference.Child("wyniki").Push().Key;

        reference.Child("wyniki").Child(key).SetRawJsonValueAsync(json)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log($"SUKCES! Wys³ano do bazy: {nick} - {displayTime}");
                }
                else
                {
                    Debug.LogError("B³¹d wysy³ania: " + task.Exception);
                }
            });
    }
}

[System.Serializable]
public class UserScore
{
    public string username;
    public float timeRaw;
    public string timeString;

    public UserScore(string name, float t, string ts)
    {
        this.username = name;
        this.timeRaw = t;
        this.timeString = ts;
    }
}