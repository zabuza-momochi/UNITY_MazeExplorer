using System.Collections.Generic;
using UnityEngine;

public class MazeSpawner : Singleton<MazeSpawner>
{
    public int width; // Larghezza del labirinto
    public int height; // Altezza del labirinto

    int halfWidth;
    int halfHeight;
    int min_limit;
    int max_limit;

    public GameObject wallPrefab; // Prefab della parete del labirinto
    public GameObject keyPrefab;
    public GameObject doorPrefab;
    public GameObject coinPrefab;
    public GameObject trapPrefab;
    public GameObject mazeContainer;

    public List<GameObject> collectablePrefab;
    public List<GameObject> PowerUpPrefab;
    public List<Material> MaterialList;

    public Material TransparentMaterial;

    List<GameObject> walls; // Lista per tenere traccia delle pareti del labirinto
    List<Vector3> emptyCells;

    private bool[,] visitedCells; // Matrice per tracciare le celle visitate


    Vector3 startBlock = new Vector3(1, 0, 0);
    Vector3 endBlock = new Vector3(0, 0, 0);

    private void Start()
    {
        if (StartMngr.Instance != null)
        {
            switch (StartMngr.Instance.UserDifficulty)
            {
                case Difficulty.Normal:

                    width = Random.Range(5, 16);
                    height = Random.Range(5, 16);

                    break;
                case Difficulty.Hard:

                    width = Random.Range(15, 32);
                    height = Random.Range(15, 32);

                    break;
                case Difficulty.Extreme:

                    width = Random.Range(31, 42);
                    height = Random.Range(31, 42);

                    break;
                case Difficulty.Random:

                    width = Random.Range(5, 51);
                    height = Random.Range(5, 51);

                    break;
            }
        }
        else
        {
            width = 5;
            height = 5;
        }

        if (width % 2 == 0)
        {

            width += 1;
        }

        if (height % 2 == 0)
        {

            height += 1;
        }

        GenerateMaze();
    }

    public void ResetMaze(bool next = true)
    {
        if (next)
        {
            width += 4;
            height += 4;
        }

        foreach (Transform child in mazeContainer.transform)
        {
            Destroy(child.gameObject);
        }

        GenerateMaze();
    }

    public void GenerateMaze()
    {
        emptyCells = new List<Vector3>();

        halfWidth = (int)(width * 0.5f);
        halfHeight = (int)(height * 0.5f);

        if (width <= height)
        {
            min_limit = width;
            max_limit = height;
        }
        else
        {
            min_limit = height;
            max_limit = width;
        }

        // Get container
        mazeContainer = GameObject.FindGameObjectWithTag("Maze");

        visitedCells = new bool[width, height];
        walls = new List<GameObject>();

        // Creazione delle pareti iniziali
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x % 2 == 0 || y % 2 == 0)
                {
                    GameObject wall = Instantiate(wallPrefab, new Vector3(x, 0, y), Quaternion.identity, mazeContainer.transform);
                    walls.Add(wall);
                }
                else
                {
                    emptyCells.Add(new Vector3(x, 0, y));
                }
            }
        }

        // Inizializzazione
        int startX = 1;
        int startY = 1;

        visitedCells[startX, startY] = true;

        // Chiamata all'algoritmo RDFS per generare il labirinto
        RecursiveDepthFirstSearch(startX, startY);

        // Set player pos
        GameManager.Instance.Player.transform.position = new Vector3(1, 0, 1);

        // Set points
        SetStartExitPoints();

        // Set Traps
        SetTraps();

        // Set key
        SetKeys();

        // Set Collectables
        SetCollectables();

        // Set PowerUp
        SetPowerUps();

        // Set Coins
        SetCoins();
    }

    private void RecursiveDepthFirstSearch(int x, int y)
    {
        List<Vector2Int> directions = new List<Vector2Int>()
        {
            new Vector2Int(2, 0),
            new Vector2Int(-2, 0),
            new Vector2Int(0, 2),
            new Vector2Int(0, -2)
        };

        ShuffleList(directions); // Mescolamento delle direzioni per rendere il labirinto più casuale
        ShuffleList(directions); // Mescolamento delle direzioni per rendere il labirinto più casuale


        foreach (Vector2Int direction in directions)
        {
            int nextX = x + direction.x;
            int nextY = y + direction.y;

            if (nextX >= 0 && nextX < width && nextY >= 0 && nextY < height && !visitedCells[nextX, nextY])
            {
                visitedCells[nextX, nextY] = true;
                int wallX = (nextX + x) / 2;
                int wallY = (nextY + y) / 2;
                DestroyWall(wallX, wallY);
                RecursiveDepthFirstSearch(nextX, nextY);
            }
        }
    }

    private void DestroyWall(int x, int y)
    {
        foreach (GameObject wall in walls)
        {
            Vector3 wallPosition = wall.transform.position;
            if (Mathf.Approximately(wallPosition.x, x) && Mathf.Approximately(wallPosition.z, y))
            {
                wall.transform.SetParent(null);
                Destroy(wall);
                walls.Remove(wall);

                emptyCells.Add(new Vector3(x, 0, y));

                break;
            }
        }
    }

    private void ShuffleList<T>(List<T> list)
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

    void SetStartExitPoints()
    {
        // Set start point
        for (int i = 0; i < walls.Count; i++)
        {
            // Check start point pos
            if (walls[i].transform.position == startBlock)
            {
                // Set wall invisible
                walls[i].GetComponent<MeshRenderer>().material = TransparentMaterial;

                // Set tag
                walls[i].tag = "Start";
            }
        }

        // Set second last column for get exit point
        int y_pos = height - 2;

        List<Vector3> index_temp = new List<Vector3>();

        for (int i = 0; i < emptyCells.Count; i++)
        {
            if (emptyCells[i].z == y_pos)
            {
                index_temp.Add(emptyCells[i]);
            }
        }

        ShuffleList(index_temp);

        int random_index = Random.Range(0, index_temp.Count);

        Vector3 randomPos = index_temp[random_index];
        endBlock = randomPos + Vector3.forward;

        // Check wall
        for (int i = 0; i < walls.Count; i++)
        {
            // Check exit point pos
            if (walls[i].transform.position == endBlock)
            {
                // Set wall invisible
                walls[i].GetComponent<MeshRenderer>().material = TransparentMaterial;
                walls[i].transform.position += Vector3.forward;

                GameObject newDoor = Instantiate(doorPrefab, endBlock, Quaternion.identity, mazeContainer.transform);

                // Add tag
                walls[i].tag = "Exit";
                walls[i].GetComponent<BoxCollider>().isTrigger = true;
            }
        }
    }

    void SetTraps()
    {
        int totalItems = (int)((width + height) * 0.4f);
        int totalTraps = Random.Range(min_limit, totalItems + 1);

        List<Vector3> emptyTraps = new List<Vector3>(emptyCells);

        for (int i = 0; i < totalTraps; i++)
        {
            Vector3 randomPos = GetRandomTrap(ref emptyTraps);

            if (randomPos != Vector3.zero)
            {
                GameObject newItem = Instantiate(trapPrefab, new Vector3(randomPos.x, trapPrefab.transform.position.y, randomPos.z), trapPrefab.transform.rotation, mazeContainer.transform);
            }
        }

    }

    void SetKeys()
    {
        Vector3 randomPos = GetRandomPos();

        if (randomPos != Vector3.zero)
        {
            GameObject newKey = Instantiate(keyPrefab, randomPos, keyPrefab.transform.rotation, mazeContainer.transform);
        }
    }

    void SetPowerUps()
    {
        int totalItems = (int)(min_limit * 0.3f);

        for (int i = 0; i < totalItems; i++)
        {
            Vector3 randomPos = GetRandomPos();

            if (randomPos != Vector3.zero)
            {
                int random_index = Random.Range(0, PowerUpPrefab.Count);

                GameObject newItem = Instantiate(PowerUpPrefab[random_index], randomPos, PowerUpPrefab[random_index].transform.rotation, mazeContainer.transform);
            }
        }
    }

    void SetCollectables()
    {
        int totalItems = Random.Range(1, (int)(min_limit * 0.5f));

        for (int i = 0; i < totalItems; i++)
        {
            Vector3 randomPos = GetRandomPos();

            if (randomPos != Vector3.zero)
            {
                int random_index = Random.Range(0, collectablePrefab.Count);
                int random_material = Random.Range(0, MaterialList.Count);

                GameObject newItem = Instantiate(collectablePrefab[random_index], randomPos, collectablePrefab[random_index].transform.rotation, mazeContainer.transform);

                newItem.GetComponent<MeshRenderer>().material = MaterialList[random_material];
            }
            else
            {
                return;
            }
        }

    }

    void SetCoins()
    {
        int total_coins = Random.Range(min_limit, max_limit);

        for (int i = 0; i < total_coins; i++)
        {
            Vector3 randomPos = GetRandomPos();

            if (randomPos != Vector3.zero)
            {
                GameObject newItem = Instantiate(coinPrefab, randomPos, coinPrefab.transform.rotation, mazeContainer.transform);
            }
            else
            {
                return;
            }
        }

    }

    Vector3 GetRandomPos(bool clean_cell = true)
    {
        List<Vector3> index_temp = new List<Vector3>();

        for (int i = 0; i < emptyCells.Count; i++)
        {
            if (emptyCells[i].x != 1 && emptyCells[i].z != 0)
            {
                index_temp.Add(emptyCells[i]);
            }
        }

        if (index_temp.Count == 0)
        {

            return Vector3.zero;
        }

        ShuffleList(index_temp);

        int random_index = Random.Range(0, index_temp.Count);

        Vector3 randomPos = index_temp[random_index];

        foreach (Vector3 vector in emptyCells)
        {
            if (vector == randomPos && clean_cell)
            {
                emptyCells.Remove(vector);
                break;
            }
        }

        //randomPos += new Vector3(0, 0.5f, 0);

        return randomPos;
    }

    Vector3 GetRandomTrap(ref List<Vector3> container)
    {
        List<Vector3> index_temp = new List<Vector3>();

        for (int i = 0; i < container.Count; i++)
        {
            if (container[i].x != 1 && container[i].z != 0)
            {
                index_temp.Add(container[i]);
            }
        }

        if (index_temp.Count == 0)
        {
            return Vector3.zero;
        }

        ShuffleList(index_temp);

        int random_index = Random.Range(0, index_temp.Count);

        Vector3 randomPos = index_temp[random_index];

        List<Vector3> copy = new List<Vector3>(container);

        foreach (Vector3 vector in copy)
        {
            if (vector == randomPos)
            {
                container.Remove(vector);
            }
            else if (vector == randomPos + Vector3.forward)
            {
                container.Remove(vector);
            }
            else if (vector == randomPos + Vector3.back)
            {
                container.Remove(vector);
            }
            else if (vector == randomPos + Vector3.left)
            {
                container.Remove(vector);
            }
            else if (vector == randomPos + Vector3.right)
            {
                container.Remove(vector);
            }
        }

        return randomPos;
    }
}
