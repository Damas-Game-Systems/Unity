using UnityEngine;
using System.Collections.Generic;
using static UnityEngine.GraphicsBuffer;
using Damas.Utils;
using UnityEngine.UIElements;

public class BoardManager : MonoBehaviour
{
    [SerializeField] dbug log;

    public static BoardManager Instance { get; private set; }
   
    //Keep track of whose turn it is 
    [SerializeField] private PieceColor currentPlayerColor = PieceColor.White;

    [SerializeField] private int width;
    [SerializeField] private int height;

    private Piece[,] pieces = new Piece[8, 8];
    private Tile[,] tiles = new Tile[8, 8];

    private Dictionary<Vector2Int, Piece> piecesD = new();
    private Dictionary<Vector2Int, Tile> tilesD = new();

    // Currently selected piece
    [SerializeField]private Piece selectedPiece = null;
    // The list of valid squares for the selected piece
    private List<Vector2Int> validMoves = new List<Vector2Int>();

    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// I think some of this is backward re: responsibilities
    /// between the BoardGenerator and the BoardManager.
    /// I feel like the generator should generate a board to give
    /// to the manager.
    /// </summary>
    public void Initialize(
        int width,
        int height,
        Dictionary<Vector2Int, Tile> tiles,
        Dictionary<Vector2Int, Piece> pieces)
    {
        this.width = width;
        this.height = height;
        tilesD = tiles;
        piecesD = pieces;
    }

    public void RegisterPiece(Piece piece)
    {
        Vector2Int key = piece.GetPositionData();

        if (piecesD[key] != null && piecesD[key] != piece)
        {
            log.error(
                $"Couldn't register {piece} at {key}." +
                $"{piecesD[key]} was already here.");
        }

        // Register the new piece
        piecesD[key] = piece;
    }

    public void DeregisterPiece(Piece piece)
    {
        Vector2Int key = piece.GetPositionData();

        if (piecesD[key] != null && piecesD[key] != piece)
        {
            log.error(
                $"Couldn't deregister {piece} at {key}." +
                $"{piecesD[key]} was already here.");
        }

        // Register the new piece
        piecesD[key] = piece;
    }

    public void OnPieceClicked(Piece piece)
    {
        // Clicked piece was enemy of current turn owner
        if (piece.color != currentPlayerColor)
        {
            // If we don't have a piece selected
            if (selectedPiece == null)
            {
                DisplayPieceInfo(piece);
                return;
            }
        }
        else if (piece.color == currentPlayerColor)
        {
            DeselectPiece();
            SelectPiece(piece);
        }
    }    
    
    public void OnTileClicked(Tile tile)
    {
        Vector2Int tilePos = tile.GetPositionData();

        if (selectedPiece == null)
        {
            if (piecesD.TryGetValue(tilePos, out Piece piece))
            {
                if (piece != null)
                {
                    OnPieceClicked(piece);
                    return;
                }
            }
        }
        else
        {
            // If the tile is in the valid moves list
            if (validMoves.Contains(tilePos))
            {
                Piece occupant = piecesD[tilePos];

                if (occupant != null)
                {
                    /// TODO:
                    /// Create an AttackCommand
                    /// - if the command would kill the piece,
                    ///   - issue the command.
                    ///   - capture target to attacker.
                    ///   - move attacker to tile.
                    /// - if not,
                    ///   - issue the command
                    ///

                    DestroyPieceAt(tilePos);
                }
                else
                {
                    MovePiece(selectedPiece, tilePos);
                    SwitchTurn();
                }
            }
            else
            {
                /// TODO:
                /// Indicate that it was an invalid move attempt?
            }

            // Clear selection
            DeselectPiece();
        }
    }

    private void SelectPiece(Piece piece)
    {
        selectedPiece = piece;
        validMoves = GetValidMoves(piece);

        TurnOnHighlights();
    }
    
    private void DeselectPiece()
    {
        TurnOffHighlights();
        selectedPiece = null;
        validMoves.Clear();
    }

    private void DisplayPieceInfo(Piece piece)
    {
        /// TODO
    }

    // Move a piece to (targetX, targetY), capture logic here
    public void MovePiece(Piece piece, Vector2Int targetPos)
    {
        piece.MoveTo(targetPos);
    }

    private void DestroyPieceAt(Vector2Int pos)
    {
        // If there's a piece at the target square, destroy it
        Piece occupant = piecesD[pos];

        if (occupant != null)
        {
            Destroy(occupant.gameObject);
            piecesD[pos] = null;
        }
    }

    public List<Vector2Int> GetValidMoves(Piece piece)
    {
        switch (piece.type)
        {
            case PieceType.Pawn:
                return GetPawnMoves(piece);

            case PieceType.Rook:
                return GetRookMoves(piece);

            case PieceType.Knight:
                return GetKnightMoves(piece);

            case PieceType.Bishop:
                return GetBishopMoves(piece);

            case PieceType.Queen:
                return GetQueenMoves(piece);

            case PieceType.King:
                return GetKingMoves(piece);
        }
        
        return new List<Vector2Int>();
    }
    private List<Vector2Int> GetPawnMoves(Piece piece)
    {
        var moves = new List<Vector2Int>();

        int direction = (piece.color == PieceColor.White) ? +1 : -1;
        Vector2Int targetPos;
        Piece occupant;

        int x           = piece.X;
        int y           = piece.Y;
        int forwardY    = y + direction;
        int forwardY2   = y + (2 * direction);
        int diagLeftX   = x - 1;
        int diagRightX  = x + 1;

        // Forward 1
        targetPos = new(x, forwardY);        
        if (piecesD.TryGetValue(targetPos, out occupant))
        {
            if (occupant == null)
                moves.Add(targetPos);
        }

        // Forward 2 if on starting row
        if (!piece.HasMoved)
        {
            targetPos = new(x, forwardY2);
            if (piecesD.TryGetValue(targetPos, out occupant))
            {
                if (occupant == null)
                    moves.Add(targetPos);
            }
        }

        // Diagonal captures
        targetPos = new(diagLeftX, forwardY);
        if (piecesD.TryGetValue(targetPos, out occupant))
        {
            if (occupant != null && occupant.color != piece.color)
                moves.Add(targetPos);
        }

        targetPos = new(diagRightX, forwardY);
        if (piecesD.TryGetValue(targetPos, out occupant))
        {
            if (occupant != null && occupant.color != piece.color)
                moves.Add(targetPos);
        }

        return moves;
    }

    private List<Vector2Int> GetRookMoves(Piece piece)
    {
        var moves = new List<Vector2Int>();
        // Up
        moves.AddRange(GetMovesInDirection(piece, 0, +1));
        // Down
        moves.AddRange(GetMovesInDirection(piece, 0, -1));
        // Left
        moves.AddRange(GetMovesInDirection(piece, -1, 0));
        // Right
        moves.AddRange(GetMovesInDirection(piece, +1, 0));
        return moves;
    }

    private List<Vector2Int> GetKnightMoves(Piece piece)
    {
        var moves = new List<Vector2Int>();
        int x = piece.X;
        int y = piece.Y;

        int[,] offsets = {
         { +2, +1 }, { +2, -1 }, { -2, +1 }, { -2, -1 },
         { +1, +2 }, { +1, -2 }, { -1, +2 }, { -1, -2 }
     };

        for (int i = 0; i < offsets.GetLength(0); i++)
        {
            int nx = x + offsets[i, 0];
            int ny = y + offsets[i, 1];
            if (IsInBounds(nx, ny))
            {
                log.warn($"{new Vector3(nx, ny)}");
                Piece occupant = piecesD[new(nx, ny)];
                if (occupant == null || occupant.color != piece.color)
                    moves.Add(new Vector2Int(nx, ny));
            }
        }
        return moves;
    }

    private List<Vector2Int> GetBishopMoves(Piece piece)
    {
        var moves = new List<Vector2Int>();
        // Diagonals
        moves.AddRange(GetMovesInDirection(piece, +1, +1));
        moves.AddRange(GetMovesInDirection(piece, +1, -1));
        moves.AddRange(GetMovesInDirection(piece, -1, +1));
        moves.AddRange(GetMovesInDirection(piece, -1, -1));
        return moves;
    }

    private List<Vector2Int> GetQueenMoves(Piece piece)
    {
        var moves = new List<Vector2Int>();
        // Rook-like
        moves.AddRange(GetMovesInDirection(piece, 0, +1));
        moves.AddRange(GetMovesInDirection(piece, 0, -1));
        moves.AddRange(GetMovesInDirection(piece, +1, 0));
        moves.AddRange(GetMovesInDirection(piece, -1, 0));
        // Bishop-like
        moves.AddRange(GetMovesInDirection(piece, +1, +1));
        moves.AddRange(GetMovesInDirection(piece, +1, -1));
        moves.AddRange(GetMovesInDirection(piece, -1, +1));
        moves.AddRange(GetMovesInDirection(piece, -1, -1));
        return moves;
    }

    private List<Vector2Int> GetKingMoves(Piece piece)
    {
        var moves = new List<Vector2Int>();
        int x = piece.X;
        int y = piece.Y;

        int[] dx = { -1, 0, +1 };
        int[] dy = { -1, 0, +1 };

        foreach (int ix in dx)
        {
            foreach (int iy in dy)
            {
                if (ix == 0 && iy == 0) continue;
                int nx = x + ix;
                int ny = y + iy;
                if (IsInBounds(nx, ny))
                {
                    Piece occupant = piecesD[new(ny, nx)];
                    if (occupant == null || occupant.color != piece.color)
                        moves.Add(new Vector2Int(nx, ny));
                }
            }
        }
        return moves;
    }

    /// <summary>
    /// Moves in a straight line until blocked or off-board.
    /// For Rooks/Bishops/Queens.
    /// </summary>
    private List<Vector2Int> GetMovesInDirection(Piece piece, int dx, int dy)
    {
        var moves = new List<Vector2Int>();
        int x = piece.X;
        int y = piece.Y;

        while (true)
        {
            x += dx;
            y += dy;
            if (!IsInBounds(x, y)) break; // off-board => stop

            Piece occupant = piecesD[new(y, x)];
            if (occupant == null)
            {
                // empty => can move
                moves.Add(new Vector2Int(x, y));
            }
            else
            {
                
                if (occupant.color != piece.color)
                    moves.Add(new Vector2Int(x, y));
                
                break;
            }
        }
        return moves;
    }

    private void TurnOnHighlights()
    {
        // Highlight each tile in validMoves
        foreach (Vector2Int pos in validMoves)
        {            
            if (tilesD.TryGetValue(pos, out Tile t))
            {
                t.SetHighlight(true); 
            }
        }
    }
    private void TurnOffHighlights()
    {
        // Turn off highlights
        foreach (Vector2Int pos in validMoves)
        {
            if (tilesD.TryGetValue(pos, out Tile t))
            {
                t.SetHighlight(false);
            }
        }
    }

    private bool IsInBounds(Vector2Int pos)
    {
        return IsInBounds(pos.x, pos.y);
    }

    private bool IsInBounds(int x, int y)
    {
        return (x >= 0 && x < 8 && y >= 0 && y < 8);
    }
    
    private void SwitchTurn() 
    {
        if (currentPlayerColor == PieceColor.White)
            currentPlayerColor = PieceColor.Black;
        else
            currentPlayerColor = PieceColor.White;        
    }
}
