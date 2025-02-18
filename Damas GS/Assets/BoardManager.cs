﻿using UnityEngine;
using System.Collections.Generic;
using static UnityEngine.GraphicsBuffer;
using Damas.Utils;
using UnityEngine.UIElements;
using Damas.Combat;

public class BoardManager : MonoBehaviour
{
    [SerializeField] dbug log;

    public static BoardManager Instance { get; private set; }
   
    //Keep track of whose turn it is 
    [SerializeField] private PieceColor currentPlayerColor = PieceColor.White;

    [SerializeField] private int width;
    [SerializeField] private int height;

    private Dictionary<Vector2Int, Piece> pieces = new();
    private Dictionary<Vector2Int, Tile> tiles = new();

    // Currently selected piece
    [SerializeField]private Piece selectedPiece = null;

    // The list of valid squares for the selected piece
    private List<Vector2Int> validMoves
    {
        get
        {
            return selectedPiece != null
                ? GetValidMoves(selectedPiece)
                : new ();
        }
    }

    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        log.warn($"Selected Piece: {selectedPiece}");
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
        this.tiles = tiles;
        this.pieces = pieces;
    }

    public void RegisterPiece(Piece piece)
    {
        Vector2Int key = piece.GetPositionData();

        if (pieces[key] != null && pieces[key] != piece)
        {
            log.error(
                $"Couldn't register {piece} at {key}." +
                $"{pieces[key]} was already registered here.");
            return;
        }

        // Register the new piece in the pieces dictionary
        piece.BeenCaptured += HandleCapture;
        pieces[key] = piece;
    }

    public void DeregisterPiece(Piece piece)
    {

        Vector2Int key = piece.GetPositionData();

        if (pieces[key] != piece)
        {
            log.error(
                $"Couldn't deregister {piece} at {key}." +
                $"{pieces[key]} was not registered here.");
            return;
        }

        // Deregister the new piece from the pieces dictionary
        piece.BeenCaptured -= HandleCapture;
        pieces[key] = null;
    }

    public void OnPieceClicked(Piece clickedPiece)
    {
        SetOverlaysOnValidMoves(false);

        if (selectedPiece == null)
        {
            if (clickedPiece.color == currentPlayerColor)
            {
                SelectPiece(clickedPiece);
            }
            else
            {
                DisplayPieceInfo(clickedPiece);
            }
        }
        else if (clickedPiece == selectedPiece)
        {
            DeselectPiece();
        }
        else if (clickedPiece.color == selectedPiece.color)
        {
            DeselectPiece();
            SelectPiece(clickedPiece);
        }
        else
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

            AttackCommand command = new(selectedPiece.Attack, clickedPiece.Health);
            if (command.WouldKill())
            {
                command.Execute();
            }
            else
            {
                command.Execute();
                SwitchTurn();
            }
        }

        //if (clickedPiece == selectedPiece)
        //{
        //    DeselectPiece();
        //    return;
        //}

        //// Clicked piece was enemy of current turn owner
        //if (clickedPiece.color != currentPlayerColor)
        //{
        //    // If we don't have a piece selected
        //    if (selectedPiece == null)
        //    {
        //        DisplayPieceInfo(clickedPiece);
        //    }
        //    else
        //    {

        //    }
        //}
        //else if (clickedPiece.color == currentPlayerColor)
        //{

        //}
    }    
    
    public void OnTileClicked(Tile tile)
    {
        Vector2Int tilePos = tile.GetPositionData();

        if (TryGetOccupant(tile, out Piece clickedPiece))
        {
            log.print($"{tile.gameObject.name} had an occupant: {clickedPiece.gameObject}");
            OnPieceClicked(clickedPiece);
        }
        else if (validMoves.Contains(tilePos))
        {
            MovePiece(selectedPiece, tilePos);
            SwitchTurn();
        }
        else
        {
            /// TODO:
            /// Indicate that it was an invalid move attempt?
        }

        //if (selectedPiece == null)
        //{
        //    if (pieces.TryGetValue(tilePos, out Piece piece))
        //    {
        //        if (piece != null)
        //        {
        //            OnPieceClicked(piece);
        //            return;
        //        }
        //    }
        //}
        //else
        //{
        //    // If the tile is in the valid moves list
        //    if (validMoves.Contains(tilePos))
        //    {
        //        Piece occupant = pieces[tilePos];

        //        if (occupant != null)
        //        {

        //        }
        //        else
        //        {

        //        }
        //    }
        //    else
        //    {

        //    }

        //    //// Clear selection
        //    //DeselectPiece();
        //}
    }

    public bool TryGetOccupant(Tile tile, out Piece occupant)
    {
        Vector2Int tilePos = tile.GetPositionData();

        if (!tiles.TryGetValue(tilePos, out Tile foundTile))
        {
            occupant = null;
            return false;
        }

        if (tile != foundTile)
        {
            Debug.LogError("Tile map is fucked");
            occupant = null;
            return false;
        }

        if (!pieces.TryGetValue(tilePos, out occupant))
        {
            return false;
        }

        return occupant != null;
    }

    private void SelectPiece(Piece piece)
    {
        log.print($"Selecting {piece.gameObject.name}");
        selectedPiece = piece;

        //TurnOnHighlights();
        SetOverlaysOnValidMoves(true);
    }
    
    private void DeselectPiece()
    {
        log.print($"Deselecting piece: {selectedPiece}");
        //TurnOffHighlights();
        selectedPiece = null;
    }

    private void DisplayPieceInfo(Piece piece)
    {
        /// TODO
    }

    // Move a piece to (targetX, targetY), capture logic here
    public void MovePiece(Piece piece, Vector2Int targetPos)
    {
        SetOverlaysOnValidMoves(false);
        piece.MoveTo(targetPos);
    }

    private void DestroyPieceAt(Vector2Int pos)
    {
        // If there's a piece at the target square, destroy it
        Piece occupant = pieces[pos];

        if (occupant != null)
        {
            Destroy(occupant.gameObject);
            pieces[pos] = null;
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
        if (pieces.TryGetValue(targetPos, out occupant))
        {
            if (occupant == null)
                moves.Add(targetPos);
        }

        // Forward 2 if on starting row
        if (!piece.HasMoved)
        {
            targetPos = new(x, forwardY2);
            if (pieces.TryGetValue(targetPos, out occupant))
            {
                if (occupant == null)
                    moves.Add(targetPos);
            }
        }

        // Diagonal captures
        targetPos = new(diagLeftX, forwardY);
        if (pieces.TryGetValue(targetPos, out occupant))
        {
            if (occupant != null && occupant.color != piece.color)
                moves.Add(targetPos);
        }

        targetPos = new(diagRightX, forwardY);
        if (pieces.TryGetValue(targetPos, out occupant))
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
                Piece occupant = pieces[new(nx, ny)];
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
                    Piece occupant = pieces[new(ny, nx)];
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

            Piece occupant = pieces[new(y, x)];
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
            if (tiles.TryGetValue(pos, out Tile t))
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
            if (tiles.TryGetValue(pos, out Tile t))
            {
                t.SetHighlight(false);
            }
        }
    }

    private void SetOverlaysOnValidMoves(bool turnOn)
    {
        log.print(
            $"Setting overlays to {(turnOn ? "on":"off")}." +
            $"Valid moves count: {validMoves.Count}");

        foreach (Vector2Int pos in validMoves)
        {
            if (!tiles.TryGetValue(pos, out Tile t))
            {
                continue;
            }

            if (!turnOn)
            {
                t.ClearOverlay();
                continue;
            }

            if (pieces.ContainsKey(pos))
            {
                t.SetOverlay(pieces[pos] != null);
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
        DeselectPiece();

        currentPlayerColor = currentPlayerColor == PieceColor.White
            ? PieceColor.Black
            : PieceColor.White;
    }

    private void HandleCapture(Piece piece)
    {
        selectedPiece.Captures.Add(piece);

        if (currentPlayerColor == PieceColor.White)
        {
            /// TODO:
            /// Go to White's captured pieces
            /// For now,
            DestroyPieceAt(piece.GetPositionData());
        }
        else
        {
            /// TODO:
            /// Go to Black's captured pieces
            /// For now,
            DestroyPieceAt(piece.GetPositionData());
        }

        MovePiece(selectedPiece, piece.GetPositionData());
        SwitchTurn();
    }
}
