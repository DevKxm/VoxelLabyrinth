using UnityEngine;
using System.Collections.Generic;

public class MazeGenerator : MonoBehaviour
{
    [Header("Ustawienia Labiryntu")]
    public GameObject wallPrefab; // Tu wrzucisz swojego Cube'a
    public GameObject floorPrefab; // Opcjonalnie: pod³oga (jeœli chcesz kafelki)

    public int width = 10;  // Szerokoœæ labiryntu
    public int depth = 10;  // G³êbokoœæ labiryntu
    public float scale = 1f; // Wielkoœæ klocka (zazwyczaj 1)

    private int[,] maze; // 1 = œciana, 0 = puste

    void Start()
    {
        // Uruchamiamy generowanie na starcie gry
        GenerateMaze();
    }

    void GenerateMaze()
    {
        maze = new int[width, depth];

        // 1. Wype³nij wszystko œcianami
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                maze[x, z] = 1; // 1 to œciana
            }
        }

        // 2. Wydr¹¿ korytarze (Algorytm Recursive Backtracker)
        // Zaczynamy od œrodka lub rogu (1,1)
        CarvePassagesFrom(1, 1);

        // 3. Zbuduj to fizycznie na scenie
        BuildMaze();
    }

    void CarvePassagesFrom(int cx, int cz)
    {
        // Ustawiamy bie¿¹c¹ komórkê jako pust¹ (0)
        maze[cx, cz] = 0;

        // Lista kierunków (Góra, Dó³, Lewo, Prawo)
        var directions = new List<Vector2Int>
        {
            new Vector2Int(0, 2), new Vector2Int(0, -2),
            new Vector2Int(2, 0), new Vector2Int(-2, 0)
        };

        // Mieszamy kierunki (losowoœæ!)
        Shuffle(directions);

        foreach (var dir in directions)
        {
            int nx = cx + dir.x;
            int nz = cz + dir.y;

            // Sprawdzamy czy s¹siad jest w granicach planszy i czy jest œcian¹
            if (IsInBounds(nx, nz) && maze[nx, nz] == 1)
            {
                // Burzymy œcianê pomiêdzy nami a s¹siadem
                maze[cx + dir.x / 2, cz + dir.y / 2] = 0;
                maze[nx, nz] = 0;

                // Rekurencja - idziemy dalej
                CarvePassagesFrom(nx, nz);
            }
        }
    }

    void BuildMaze()
    {
        // Pêtla po ca³ej mapie
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                Vector3 position = new Vector3(x * scale, 0.5f * scale, z * scale);

                // Jeœli w tablicy jest 1 -> stawiamy œcianê
                if (maze[x, z] == 1)
                {
                    Instantiate(wallPrefab, position, Quaternion.identity, transform);
                }
                // (Opcjonalnie) Jeœli 0 -> stawiamy pod³ogê
                /*
                else if (floorPrefab != null)
                {
                    Instantiate(floorPrefab, new Vector3(x*scale, -0.5f, z*scale), Quaternion.identity, transform);
                }
                */
            }
        }

        // Zrób miejsce na start dla gracza (np. na pozycji 1,1)
        // Upewnij siê, ¿e gracz stoi tam, gdzie jest pusto!
    }

    bool IsInBounds(int x, int z)
    {
        return x > 0 && x < width - 1 && z > 0 && z < depth - 1;
    }

    // Funkcja do mieszania listy (Fisher-Yates shuffle)
    void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}