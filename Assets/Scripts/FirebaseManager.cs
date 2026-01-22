using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions; // Wa¿ne do obs³ugi w¹tków

public class FirebaseManager : MonoBehaviour
{
    private DatabaseReference reference;
    private bool isFirebaseReady = false;

    void Start()
    {
        // Sprawdzamy zale¿noœci Firebase (standardowa procedura)
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                // Firebase dzia³a! Inicjalizujemy bazê
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
        // Pobieramy g³ówny punkt dostêpu do bazy danych
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        isFirebaseReady = true;
        Debug.Log("Firebase po³¹czony i gotowy!");
    }

    // Tê funkcjê wywo³uje Twój HealthManager!
    public void SaveScore(string nick, float rawTime, string displayTime)
    {
        if (!isFirebaseReady || reference == null)
        {
            Debug.LogError("Firebase nie jest jeszcze gotowy!");
            return;
        }

        // Tworzymy obiekt z danymi do wys³ania
        UserScore scoreData = new UserScore(nick, rawTime, displayTime);

        // Zamieniamy obiekt na format JSON (tekstowy)
        string json = JsonUtility.ToJson(scoreData);

        // Generujemy unikalny klucz dla wpisu (¿eby wyniki siê nie nadpisywa³y)
        string key = reference.Child("wyniki").Push().Key;

        // Wysy³amy do bazy
        reference.Child("wyniki").Child(key).SetRawJsonValueAsync(json)
            .ContinueWithOnMainThread(task => {
                if (task.IsCompleted)
                {
                    Debug.Log("Wynik zapisany w chmurze pomyœlnie!");
                }
                else
                {
                    Debug.LogError("Nie uda³o siê zapisaæ wyniku: " + task.Exception);
                }
            });
    }
}

// Klasa pomocnicza - struktura danych, któr¹ wysy³amy
[System.Serializable]
public class UserScore
{
    public string username;
    public float timeRaw;     // Czas w sekundach (do sortowania)
    public string timeString; // Czas ³adny (01:23:45) do wyœwietlania

    public UserScore(string name, float t, string ts)
    {
        this.username = name;
        this.timeRaw = t;
        this.timeString = ts;
    }
}