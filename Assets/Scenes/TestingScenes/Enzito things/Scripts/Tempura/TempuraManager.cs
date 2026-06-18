using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TempuraKitchen.Minigames.WASDGrid
{
    public class WASDMinigameController : MonoBehaviour
    {
        [Header("Grid Dimensions")]
        [SerializeField] private int gridWidth = 4;
        [SerializeField] private int gridHeight = 4;

        [Header("Grid Settings")]
        [SerializeField] private GameObject tilePrefab;
        [SerializeField] private float tileSpacing = 1.2f;

        [Header("Materials for MeshRenderer")]
        [SerializeField] private Material defaultMaterial;
        [SerializeField] private Material activeMaterial;
        [SerializeField] private Material startMaterial;
        [SerializeField] private Material endMaterial;
        [SerializeField] private Material pathMaterial;

        [Header("Connector Settings")]
        [SerializeField] private GameObject connectorPrefab;

        [Header("Tempura Settings")]
        [SerializeField] private GameObject tempuraPrefab;
        [SerializeField] private float tempuraHoverHeight = 0.8f;   
        [SerializeField] private float tempuraMoveSpeed = 12f;      
        [SerializeField] private float tempuraRotationSpeed = 120f; 

        [Header("Input Tolerance")]
        [SerializeField] private float diagonalBufferTime = 0.05f;

        private GameObject[,] gridObjects;
        private MeshRenderer[,] gridRenderers;
        private MovementDirection[,] gridDirections;
        private GameObject[,] connectorInstances;

        private List<TileCoordinate> solutionPath = new List<TileCoordinate>();
        private TileCoordinate startCoordinate;
        private TileCoordinate endCoordinate;
        private TileCoordinate playerCurrentCoordinate;

        private int winStreak = 0;
        private const int requiredWins = 3;
        private bool isGameActive = true;

        private bool isWaitingForInputBuffer = false;
        private bool bufferW, bufferS, bufferA, bufferD;

        [SerializeField] private MiniGamesBase minigame;

        private GameObject tempuraInstance;
        private Vector3 tempuraTargetPosition;
        private bool isInitialized = false;

        [Header("Sounds Section")]
        [SerializeField] private AudioClip moveTempur0;
        [SerializeField] private AudioClip moveTempur1;
        private int counterMove;
        [SerializeField] private AudioClip completeOneSection;
        [SerializeField] private AudioClip finihTempur;
        private SoundManager soundManager;

        private void InitializeController()
        {
            if (isInitialized) return;

            gridObjects = new GameObject[gridWidth, gridHeight];
            gridRenderers = new MeshRenderer[gridWidth, gridHeight];
            gridDirections = new MovementDirection[gridWidth, gridHeight];
            connectorInstances = new GameObject[gridWidth, gridHeight];

            startCoordinate = new TileCoordinate(0, 0);
            endCoordinate = new TileCoordinate(gridWidth - 1, gridHeight - 1);

            isInitialized = true;
        }

        private void Awake()
        {
            InitializeController();
        }

        private void Start()
        {
            InitializeController();
            GenerateGridVisuals();
            SpawnTempura();
            StartNewRound();

            soundManager = GameManager.instance._soundManager;
        }

        private void OnEnable()
        {
            isGameActive = true;
            counterMove = 0;
            InitializeController();
            GenerateGridVisuals();
            SpawnTempura();
            StartNewRound();
        }

        private void Update()
        {
            HandleTempuraAnimation();

            if (!isGameActive) return;

            if (Input.GetKeyDown(KeyCode.W)) { bufferW = true; StartInputBuffer(); }
            if (Input.GetKeyDown(KeyCode.S)) { bufferS = true; StartInputBuffer(); }
            if (Input.GetKeyDown(KeyCode.A)) { bufferA = true; StartInputBuffer(); }
            if (Input.GetKeyDown(KeyCode.D)) { bufferD = true; StartInputBuffer(); }
        }

        private void StartInputBuffer()
        {
            if (isWaitingForInputBuffer) return;

            isWaitingForInputBuffer = true;

            switch (counterMove)
            {
                case 0:
                    soundManager.ReproduceSound(moveTempur0);
                    counterMove = 1;
                    break;

                case 1:
                    soundManager.ReproduceSound(moveTempur1);
                    counterMove = 0;
                    break;
            }
            Invoke(nameof(EvaluateBufferedInput), diagonalBufferTime);
        }

        private void EvaluateBufferedInput()
        {
            MovementDirection inputDirection = GetInputDirection(bufferW, bufferS, bufferA, bufferD);

            if (inputDirection != MovementDirection.None)
            {
                ProcessPlayerMove(inputDirection);
            }

            bufferW = false;
            bufferS = false;
            bufferA = false;
            bufferD = false;
            isWaitingForInputBuffer = false;
        }

        private void GenerateGridVisuals()
        {
            if (gridObjects[0, 0] != null) return;

            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    Vector3 spawnPos = transform.position + new Vector3(x * tileSpacing, 0, y * tileSpacing);
                    GameObject newTile = Instantiate(tilePrefab, spawnPos, Quaternion.identity, transform);

                    gridObjects[x, y] = newTile;
                    gridRenderers[x, y] = newTile.GetComponent<MeshRenderer>();

                    if (connectorPrefab != null)
                    {
                        GameObject newConnector = Instantiate(connectorPrefab, spawnPos, Quaternion.identity, newTile.transform);
                        connectorInstances[x, y] = newConnector;
                        newConnector.SetActive(false);
                    }
                }
            }
        }

        private void SpawnTempura()
        {
            if (tempuraPrefab == null) return;

            if (tempuraInstance == null)
            {
                tempuraInstance = Instantiate(tempuraPrefab, transform.position, Quaternion.identity, transform);
            }

            tempuraInstance.SetActive(true);
            UpdateTempuraTargetPosition();
            tempuraInstance.transform.position = tempuraTargetPosition;
        }

        private void StartNewRound()
        {
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    gridDirections[x, y] = MovementDirection.None;
                }
            }

            solutionPath = GenerateRandomValidPath(startCoordinate, endCoordinate);
            CalculatePathDirections();

            playerCurrentCoordinate = new TileCoordinate(startCoordinate.x, startCoordinate.y);

            UpdateGridColors();
            UpdateConnectorVisuals();
            UpdateTempuraTargetPosition();
        }

        private void HandleTempuraAnimation()
        {
            if (tempuraInstance == null) return;

            tempuraInstance.transform.position = Vector3.Lerp(
                tempuraInstance.transform.position,
                tempuraTargetPosition,
                Time.deltaTime * tempuraMoveSpeed
            );

            tempuraInstance.transform.Rotate(Vector3.right, tempuraRotationSpeed * Time.deltaTime, Space.Self);
        }

        private void UpdateTempuraTargetPosition()
        {
            if (gridObjects == null || tempuraInstance == null) return;

            GameObject currentTile = gridObjects[playerCurrentCoordinate.x, playerCurrentCoordinate.y];
            if (currentTile != null)
            {
                float tileHeightOffset = 0f;
                MeshFilter mf = currentTile.GetComponent<MeshFilter>();
                if (mf != null && mf.sharedMesh != null)
                {
                    tileHeightOffset = (mf.sharedMesh.bounds.size.y * currentTile.transform.localScale.y) / 2f;
                }

                tempuraTargetPosition = currentTile.transform.position + Vector3.up * (tileHeightOffset + tempuraHoverHeight);
            }
        }

        private List<TileCoordinate> GenerateRandomValidPath(TileCoordinate start, TileCoordinate end)
        {
            List<TileCoordinate> path = new List<TileCoordinate>();
            HashSet<TileCoordinate> visited = new HashSet<TileCoordinate>();

            TileCoordinate current = start;
            path.Add(current);
            visited.Add(current);

            int safetyCounter = 0;

            while (!current.Equals(end) && safetyCounter < 200)
            {
                safetyCounter++;
                List<TileCoordinate> validNeighbors = new List<TileCoordinate>();

                int[] dx = { 0, 0, 1, -1, 1, -1, 1, -1 };
                int[] dy = { 1, -1, 0, 0, 1, 1, -1, -1 };

                for (int i = 0; i < 8; i++)
                {
                    TileCoordinate neighbor = new TileCoordinate(current.x + dx[i], current.y + dy[i]);

                    if (neighbor.x >= 0 && neighbor.x < gridWidth && neighbor.y >= 0 && neighbor.y < gridHeight)
                    {
                        if (!visited.Contains(neighbor))
                        {
                            validNeighbors.Add(neighbor);
                        }
                    }
                }

                if (validNeighbors.Count == 0)
                {
                    path.Clear();
                    visited.Clear();
                    current = start;
                    path.Add(current);
                    visited.Add(current);
                    continue;
                }

                TileCoordinate nextStep = validNeighbors[Random.Range(0, validNeighbors.Count)];
                path.Add(nextStep);
                visited.Add(nextStep);
                current = nextStep;
            }

            return path;
        }

        private void CalculatePathDirections()
        {
            for (int i = 0; i < solutionPath.Count - 1; i++)
            {
                TileCoordinate current = solutionPath[i];
                TileCoordinate next = solutionPath[i + 1];

                int diffX = next.x - current.x;
                int diffY = next.y - current.y;

                MovementDirection requiredDir = MovementDirection.None;

                if (diffX == 0 && diffY == 1) requiredDir = MovementDirection.North;
                else if (diffX == 0 && diffY == -1) requiredDir = MovementDirection.South;
                else if (diffX == 1 && diffY == 0) requiredDir = MovementDirection.East;
                else if (diffX == -1 && diffY == 0) requiredDir = MovementDirection.West;
                else if (diffX == 1 && diffY == 1) requiredDir = MovementDirection.NorthEast;
                else if (diffX == -1 && diffY == 1) requiredDir = MovementDirection.NorthWest;
                else if (diffX == 1 && diffY == -1) requiredDir = MovementDirection.SouthEast;
                else if (diffX == -1 && diffY == -1) requiredDir = MovementDirection.SouthWest;

                gridDirections[current.x, current.y] = requiredDir;
            }
        }

        private void UpdateConnectorVisuals()
        {
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    if (connectorInstances[x, y] != null)
                    {
                        connectorInstances[x, y].SetActive(false);
                    }
                }
            }

            int px = playerCurrentCoordinate.x;
            int py = playerCurrentCoordinate.y;

            if (connectorInstances[px, py] == null || playerCurrentCoordinate.Equals(endCoordinate)) return;

            int currentPathIndex = solutionPath.FindIndex(p => p.x == playerCurrentCoordinate.x && p.y == playerCurrentCoordinate.y);
            if (currentPathIndex == -1) return;

            TileCoordinate nextCoord = solutionPath[currentPathIndex + 1];

            GameObject currentTile = gridObjects[px, py];
            GameObject nextTile = gridObjects[nextCoord.x, nextCoord.y];

            connectorInstances[px, py].SetActive(true);

            Vector3 startPos = currentTile.transform.position;
            Vector3 endPos = nextTile.transform.position;

            // 1. Calculamos el punto medio en X y Z entre las dos baldosas
            Vector3 middlePoint = (startPos + endPos) / 2f;

            // 2. Calculamos el alto real del cubo para saber dónde está la superficie superior
            float tileHeightOffset = 0f;
            float cubeSize = 1.0f;
            MeshFilter mf = currentTile.GetComponent<MeshFilter>();
            if (mf != null && mf.sharedMesh != null)
            {
                tileHeightOffset = (mf.sharedMesh.bounds.size.y * currentTile.transform.localScale.y) / 2f;
                cubeSize = mf.sharedMesh.bounds.size.x * currentTile.transform.localScale.x;
            }

            // CORRECCIÓN VERTICAL FIJA: Ponemos la flecha en el medio horizontal, pero en Y la subimos a la superficie
            middlePoint.y = startPos.y + tileHeightOffset;
            connectorInstances[px, py].transform.position = middlePoint;

            Vector3 direction = endPos - startPos;
            connectorInstances[px, py].transform.rotation = Quaternion.LookRotation(direction, Vector3.up);

            float totalDistance = direction.magnitude;
            float actualLineLength = 0f;

            bool isDiagonal = (px != nextCoord.x) && (py != nextCoord.y);

            if (isDiagonal)
            {
                actualLineLength = totalDistance - (cubeSize * 1.4142f);
            }
            else
            {
                actualLineLength = totalDistance - cubeSize;
            }

            Vector3 prefabScale = connectorPrefab.transform.localScale;
            connectorInstances[px, py].transform.localScale = new Vector3(prefabScale.x, prefabScale.y, actualLineLength);
        }

        private void ProcessPlayerMove(MovementDirection pressedDirection)
        {
            MovementDirection expectedDirection = gridDirections[playerCurrentCoordinate.x, playerCurrentCoordinate.y];

            if (pressedDirection == expectedDirection)
            {
                int currentPathIndex = solutionPath.FindIndex(p => p.x == playerCurrentCoordinate.x && p.y == playerCurrentCoordinate.y);
                playerCurrentCoordinate = solutionPath[currentPathIndex + 1];

                UpdateGridColors();
                UpdateConnectorVisuals();
                UpdateTempuraTargetPosition();

                if (playerCurrentCoordinate.Equals(endCoordinate))
                {
                    winStreak++;
                    if (winStreak >= requiredWins)
                    {
                        soundManager.ReproduceSound(finihTempur);
                        IReachPoint minigameReach = minigame as IReachPoint;
                        minigameReach.Complete();
                        isGameActive = false;
                        winStreak = 0;
                    }
                    else
                    {
                        StartNewRound();
                        soundManager.ReproduceSound(completeOneSection);
                    }
                }
            }
            else
            {
                playerCurrentCoordinate = startCoordinate;
                UpdateGridColors();
                UpdateConnectorVisuals();
                UpdateTempuraTargetPosition();
            }
        }

        private void UpdateGridColors()
        {
            int playerIndex = solutionPath.FindIndex(p => p.x == playerCurrentCoordinate.x && p.y == playerCurrentCoordinate.y);

            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    TileCoordinate tileCoord = new TileCoordinate(x, y);

                    if (tileCoord.x == playerCurrentCoordinate.x && tileCoord.y == playerCurrentCoordinate.y)
                    {
                        gridRenderers[x, y].material = activeMaterial;
                    }
                    else if (tileCoord.x == startCoordinate.x && tileCoord.y == startCoordinate.y)
                    {
                        gridRenderers[x, y].material = startMaterial;
                    }
                    else if (tileCoord.x == endCoordinate.x && tileCoord.y == endCoordinate.y)
                    {
                        gridRenderers[x, y].material = endMaterial;
                    }
                    else if (solutionPath.Any(p => p.x == x && p.y == y))
                    {
                        int thisTileIndex = solutionPath.FindIndex(p => p.x == x && p.y == y);

                        if (thisTileIndex < playerIndex)
                        {
                            gridRenderers[x, y].material = activeMaterial;
                        }
                        else
                        {
                            gridRenderers[x, y].material = pathMaterial;
                        }
                    }
                    else
                    {
                        gridRenderers[x, y].material = defaultMaterial;
                    }
                }
            }
        }

        public MovementDirection GetTileDirection(int x, int y)
        {
            return gridDirections[x, y];
        }

        private MovementDirection GetInputDirection(bool w, bool s, bool a, bool d)
        {
            if (w && d) return MovementDirection.NorthEast;
            if (w && a) return MovementDirection.NorthWest;
            if (s && d) return MovementDirection.SouthEast;
            if (s && a) return MovementDirection.SouthWest;

            if (w) return MovementDirection.North;
            if (s) return MovementDirection.South;
            if (d) return MovementDirection.East;
            if (a) return MovementDirection.West;

            return MovementDirection.None;
        }
    }
}