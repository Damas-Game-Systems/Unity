using UnityEngine;
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

    // // The list of valid squares for the selected piece
    // private List<Vector2Int> validMoves
    // {
    //     get
    //     {
    //         return selectedPiece != null
    //             ? GetValidMoves(selectedPiece)
    //             : new ();
    //     }
    // }

    
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

            List<Vector2Int> validMoves = selectedPiece.GetValidMoves();
            Vector2Int enemyPos = clickedPiece.GetPositionData();

            if (!validMoves.Contains(enemyPos))
            {
                log.warn("Invalid capture: Enemy is not in range.");
                return;
            }

            // If tile is valid, perform the attack
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
        
    }

    public void OnTileClicked(Tile tile)
    {
        Vector2Int tilePos = tile.GetPositionData();

        if (TryGetOccupant(tile, out Piece clickedPiece))
        {
            log.print($"{tile.gameObject.name} had an occupant: {clickedPiece.gameObject}");
            OnPieceClicked(clickedPiece);
        }
        else if (selectedPiece != null && selectedPiece.GetValidMoves().Contains(tilePos))
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
        selectedPiece?.Select();

        SetOverlaysOnValidMoves(true);
    }
    
    private void DeselectPiece()
    {
        log.print($"Deselecting piece: {selectedPiece}");

        selectedPiece?.Deselect();

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

   
    /// <summary>
    /// Moves in a straight line until blocked or off-board.
    /// For Rooks/Bishops/Queens. kept this here for now might get moved to piece class - lee
    /// </summary>
    public List<Vector2Int> GetMovesInDirection(Piece piece, int dx, int dy)
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
        foreach (Vector2Int pos in selectedPiece?.GetValidMoves() ?? new List<Vector2Int>())
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
        foreach (Vector2Int pos in selectedPiece?.GetValidMoves() ?? new List<Vector2Int>())
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
            $"Valid moves count: {selectedPiece?.GetValidMoves().Count}");

        foreach (Vector2Int pos in selectedPiece?.GetValidMoves() ?? new List<Vector2Int>())
        {
            if (!tiles.TryGetValue(pos, out Tile tile)) continue;

            if (!turnOn)
            {
                tile.ClearOverlay();
                continue;
            }

            tile.SetOverlay(pieces.ContainsKey(pos) && pieces[pos] != null);
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
    
    public bool IsTileEmpty(Vector2Int pos)
    {
        return pieces.ContainsKey(pos) && pieces[pos] == null;
    }

    public bool IsOpponentPiece(Piece piece, Vector2Int pos)
    {
        return pieces.TryGetValue(pos, out Piece target) && target != null && target.color != piece.color;
    }

    public bool IsValidMove(Piece piece, Vector2Int pos)
    {
        return IsTileEmpty(pos) || IsOpponentPiece(piece, pos);
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
