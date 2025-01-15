using UnityEngine;

public class ChessBoardGenerator : MonoBehaviour
{
    [Header("Board Dimensions")]
    public int rows = 8;
    public int columns = 8;

    [Header("Tile Settings")]
    public GameObject tilePrefab;
    public float tileSize = 1f;
    public Color lightColor = Color.white;
    public Color darkColor = Color.gray;

    [Header("White Piece Prefabs")]
    public GameObject whitePawnPrefab;
    public GameObject whiteRookPrefab;
    public GameObject whiteKnightPrefab;
    public GameObject whiteBishopPrefab;
    public GameObject whiteQueenPrefab;
    public GameObject whiteKingPrefab;

    [Header("Black Piece Prefabs")]
    public GameObject blackPawnPrefab;
    public GameObject blackRookPrefab;
    public GameObject blackKnightPrefab;
    public GameObject blackBishopPrefab;
    public GameObject blackQueenPrefab;
    public GameObject blackKingPrefab;

    private void Start()
    {
        GenerateBoard();
        PlaceAllPieces();
    }

    private void GenerateBoard()
    {
        if (tilePrefab == null)
        {
            Debug.LogError("Assign the tile prefab");
            return;
        }

        // Create 8×8 tiles
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                Vector2 spawnPos = new Vector2(x * tileSize, y * tileSize);
                GameObject tileGO = Instantiate(tilePrefab, spawnPos, Quaternion.identity, transform);
                tileGO.name = $"Tile ({x},{y})";

                // Grab Tile script
                Tile tile = tileGO.GetComponent<Tile>();
                tile.boardX = x;
                tile.boardY = y;

                // Alternate color
                bool isDark = ((x + y) % 2 == 1);
                tile.defaultColor = isDark ? darkColor : lightColor;
                tile.GetComponent<SpriteRenderer>().color = tile.defaultColor;
            }
        }
    }

    private void PlaceAllPieces()
    {
        // --- White Pieces ---
        // Row 0
        PlacePiece(whiteRookPrefab, 0, 0);
        PlacePiece(whiteKnightPrefab, 1, 0);
        PlacePiece(whiteBishopPrefab, 2, 0);
        PlacePiece(whiteQueenPrefab, 3, 0);
        PlacePiece(whiteKingPrefab, 4, 0);
        PlacePiece(whiteBishopPrefab, 5, 0);
        PlacePiece(whiteKnightPrefab, 6, 0);
        PlacePiece(whiteRookPrefab, 7, 0);

        // Row 1 (White pawns)
        for (int col = 0; col < 8; col++)
            PlacePiece(whitePawnPrefab, col, 1);

        // --- Black Pieces ---
        // Row 7
        PlacePiece(blackRookPrefab, 0, 7);
        PlacePiece(blackKnightPrefab, 1, 7);
        PlacePiece(blackBishopPrefab, 2, 7);
        PlacePiece(blackQueenPrefab, 3, 7);
        PlacePiece(blackKingPrefab, 4, 7);
        PlacePiece(blackBishopPrefab, 5, 7);
        PlacePiece(blackKnightPrefab, 6, 7);
        PlacePiece(blackRookPrefab, 7, 7);

        // Row 6 (Black pawns)
        for (int col = 0; col < 8; col++)
            PlacePiece(blackPawnPrefab, col, 6);
    }

    private void PlacePiece(GameObject prefab, int x, int y)
    {
        if (prefab == null)
        {
            Debug.LogWarning($"Missing prefab at location ({x},{y})");
            return;
        }

        Vector2 pos = new Vector2(x * tileSize, y * tileSize);
        GameObject pieceGO = Instantiate(prefab, pos, Quaternion.identity, transform);

        
        Piece piece = pieceGO.GetComponent<Piece>();
        
        // Register with BoardManager
        BoardManager.Instance.RegisterPiece(piece, x, y);
    }
}
