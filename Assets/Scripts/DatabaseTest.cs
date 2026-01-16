using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;

public class DatabaseTest : MonoBehaviour
{
    DatabaseReference reference;

    void Start()
    {
        // 1. Sprawdza, czy wszystkie pliki Firebase s¹ na miejscu
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            var dependencyStatus = task.Result;

            if (dependencyStatus == DependencyStatus.Available)
            {
                // JEST OK! £¹czy siê
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Nie uda³o siê po³¹czyæ z Firebase: " + dependencyStatus);
            }
        });
    }

    void InitializeFirebase()
    {
        Debug.Log("Po³¹czono z Firebase! Wysy³am dane...");

        string databaseUrl = "https://voxellabyrinth-default-rtdb.europe-west1.firebasedatabase.app/";

        // Pobieramy "korzeñ" bazy danych
        reference = FirebaseDatabase.GetInstance(databaseUrl).RootReference;

        // Testowa wiadomoœæ
        // Tworzy w bazie szufladkê "test_polaczenia" i wpisuje "Sukces"
        reference.Child("test_polaczenia").SetValueAsync("Udalo sie polaczyc z Unity!");
    }
}