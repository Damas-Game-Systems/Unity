using Damas.UI;
using System.Collections.Generic;
using UnityEngine;

namespace Damas
{
    public class ChessBoardGenerator : MonoBehaviour
    {
        [Header("Board Dimensions")]
        public int columns = 8;
        public int rows = 8;

        [Header("Tile Settings")]
        public GameObject tilePrefab;
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

        private Dictionary<Vector2Int, Tile> tilesAtPositions = new();
        private Dictionary<Vector2Int, Piece> piecesAtPositions = new();

        private void Start()
        {
            GenerateBoard();
            PlaceAllPieces();
            BoardManager.Instance.Initialize(
                columns,
                rows,
                tilesAtPositions,
                piecesAtPositions);
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
                    Vector2Int boardPos = new(x, y);

                    Vector3 spawnPos = new(x, y, 1);
                    GameObject tileGO = Instantiate(tilePrefab, spawnPos, Quaternion.identity, transform);
                    tileGO.name = $"Tile ({x},{y})";

                    // Grab Tile script
                    Tile tile = tileGO.GetComponent<Tile>();

                    // Alternate color
                    bool isDark = (x + y) % 2 == 1;
                    tile.defaultColor = isDark ? darkColor : lightColor;
                    tile.GetComponent<SpriteRenderer>().color = tile.defaultColor;

                    tile.OnSpawn(boardPos);

                    // Add tile to dictionary
                    tilesAtPositions[boardPos] = tile;

                    // Add an empty key to the pieces dictionary.
                    // This way, we can still use it like an array,
                    // but with added lookup fns
                    piecesAtPositions[boardPos] = default;
                }
            }
        }

        private void PlaceAllPieces()
        {
            // --- White Pieces ---
            // Row 0
            SpawnPiece(whiteRookPrefab, 0, 0);
            SpawnPiece(whiteKnightPrefab, 1, 0);
            SpawnPiece(whiteBishopPrefab, 2, 0);
            SpawnPiece(whiteQueenPrefab, 3, 0);
            SpawnPiece(whiteKingPrefab, 4, 0);
            SpawnPiece(whiteBishopPrefab, 5, 0);
            SpawnPiece(whiteKnightPrefab, 6, 0);
            SpawnPiece(whiteRookPrefab, 7, 0);

            // Row 1 (White pawns)
            for (int col = 0; col < 8; col++)
                SpawnPiece(whitePawnPrefab, col, 1);

            // --- Black Pieces ---
            // Row 7
            SpawnPiece(blackRookPrefab, 0, 7);
            SpawnPiece(blackKnightPrefab, 1, 7);
            SpawnPiece(blackBishopPrefab, 2, 7);
            SpawnPiece(blackQueenPrefab, 3, 7);
            SpawnPiece(blackKingPrefab, 4, 7);
            SpawnPiece(blackBishopPrefab, 5, 7);
            SpawnPiece(blackKnightPrefab, 6, 7);
            SpawnPiece(blackRookPrefab, 7, 7);

            // Row 6 (Black pawns)
            for (int col = 0; col < 8; col++)
                SpawnPiece(blackPawnPrefab, col, 6);
        }

        private void SpawnPiece(GameObject prefab, int x, int y)
        {
            if (prefab == null)
            {
                Debug.LogWarning($"Missing prefab at location ({x},{y})");
                return;
            }

            Vector2Int pos = new(x, y);
            GameObject pieceGO = Instantiate(prefab, transform);

            Piece piece = pieceGO.GetComponent<Piece>();

            piece.OnSpawn(pos);
            // Add to piece dictionary
            piecesAtPositions[pos] = piece;
        }
    }
}