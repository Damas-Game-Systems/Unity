using UnityEngine;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour
{
    
    public static BoardManager Instance { get; private set; }

   
    //Keep track of whose turn it is 
   [SerializeField] private PieceColor currentPlayer = PieceColor.White;

    
    private Piece[,] board = new Piece[8, 8];

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

    
    
    
    public void RegisterPiece(Piece piece, int x, int y)
    {
        board[y, x] = piece;
        piece.boardX = x;
        piece.boardY = y;
    }

    
   
    public void OnPieceClicked(Piece piece)
    {
       
        if (piece.color != currentPlayer)
        {
            // It's the opponent's piece → ignore or display error
            return;
        }

        // If we have no piece selected, select this piece
        if (selectedPiece == null)
        {
            SelectPiece(piece);
        }
        else
        {
            // Already have a selected piece:
            //re-select. //capture logi would also go here
            DeselectPiece();
            SelectPiece(piece);
        }
    }

    
    
    public void OnTileClicked(Tile tile)
    {
        if (selectedPiece == null) return; // No piece selected

        Vector2Int clickedPos = new Vector2Int(tile.boardX, tile.boardY);

        // If the tile is in the valid moves list, move there
        if (validMoves.Contains(clickedPos))
        {
            MovePiece(selectedPiece, tile.boardX, tile.boardY);

           
            SwitchTurn(); 
        }

        // Clear selection 
        DeselectPiece();
    }

    
   
    private void SelectPiece(Piece piece)
    {
        selectedPiece = piece;
        validMoves = GetValidMoves(piece);

        // Highlight each tile in validMoves
        foreach (Vector2Int pos in validMoves)
        {
            Tile t = GetTileAt(pos.x, pos.y);
            t.SetHighlight(true);
        }
    }

    
    private void DeselectPiece()
    {
        if (selectedPiece != null)
        {
            // Turn off highlights
            foreach (Vector2Int pos in validMoves)
            {
                Tile t = GetTileAt(pos.x, pos.y);
                t.SetHighlight(false);
            }
        }
        selectedPiece = null;
        validMoves.Clear();
    }

 
    // Move a piece to (targetX, targetY), capture logic here
    public void MovePiece(Piece piece, int targetX, int targetY)
    {
         int oldX = piece.boardX;
         int oldY = piece.boardY;

        // If there's a piece at the target square, destroy it
        Piece occupant = board[targetY, targetX];
        if (occupant != null && occupant != piece)
        {
            
            Destroy(occupant.gameObject);
            board[targetY, targetX] = null;
        }

        // Remove from old location
        board[oldY, oldX] = null;

        // Update piece coordinates
        piece.boardX = targetX;
        piece.boardY = targetY;

        // Place piece in the new location
        board[targetY, targetX] = piece;

       
        piece.transform.position = new Vector2(targetX, targetY);
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
        int x = piece.boardX;
        int y = piece.boardY;

        int direction = (piece.color == PieceColor.White) ? +1 : -1;

        // Forward 1
        int forwardY = y + direction;
        if (IsInBounds(x, forwardY) && board[forwardY, x] == null)
        {
            moves.Add(new Vector2Int(x, forwardY));

            // Forward 2 if on starting row
            bool onStartingRow = (piece.color == PieceColor.White && y == 1)
                              || (piece.color == PieceColor.Black && y == 6);
            if (onStartingRow)
            {
                int forwardY2 = y + 2 * direction;
                if (IsInBounds(x, forwardY2) && board[forwardY2, x] == null)
                {
                    moves.Add(new Vector2Int(x, forwardY2));
                }
            }
        }

        // Diagonal captures
        int diagLeftX = x - 1;
        int diagRightX = x + 1;
        if (IsInBounds(diagLeftX, forwardY))
        {
            Piece occupant = board[forwardY, diagLeftX];
            if (occupant != null && occupant.color != piece.color)
                moves.Add(new Vector2Int(diagLeftX, forwardY));
        }
        if (IsInBounds(diagRightX, forwardY))
        {
            Piece occupant = board[forwardY, diagRightX];
            if (occupant != null && occupant.color != piece.color)
                moves.Add(new Vector2Int(diagRightX, forwardY));
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
        int x = piece.boardX;
        int y = piece.boardY;

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
                Piece occupant = board[ny, nx];
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
        int x = piece.boardX;
        int y = piece.boardY;

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
                    Piece occupant = board[ny, nx];
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
        int x = piece.boardX;
        int y = piece.boardY;

        while (true)
        {
            x += dx;
            y += dy;
            if (!IsInBounds(x, y)) break; // off-board => stop

            Piece occupant = board[y, x];
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

    private bool IsInBounds(int x, int y)
    {
        return (x >= 0 && x < 8 && y >= 0 && y < 8);
    }
    
    private void SwitchTurn() 
    {
        if (currentPlayer == PieceColor.White)
            currentPlayer = PieceColor.Black;
        else
            currentPlayer = PieceColor.White;

        
    }

    private Tile[,] tiles = new Tile[8, 8];

    public void RegisterTile(Tile tile, int x, int y)
    {
        tiles[y, x] = tile;
    }

    public Tile GetTileAt(int x, int y)
    {
        if (x < 0 || x >= 8 || y < 0 || y >= 8)
            return null;
        return tiles[y, x];
    }

}
