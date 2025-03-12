using Damas.Combat;
using Damas.Utils;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;
using System.Linq;

namespace Damas
{
    public class BoardManager : Singleton<BoardManager>
    {
        [SerializeField] dbug log;

        public bool Initialized { get; private set; } = false;

        // Currently selected tile
        [field: SerializeField, ReadOnly] public Tile SelectedTile { get; private set; }
        // Currently selected piece
        public Piece SelectedPiece => SelectedTile?.Occupant;

        // Currently selected tile
        //[field: SerializeField, ReadOnly] public Tile TargetedTile { get; private set; }
        // Currently selected piece
        //[field: SerializeField, ReadOnly] public Piece TargetedPiece => TargetedTile.Occupant;

        //Keep track of whose turn it is 
        [field: SerializeField, ReadOnly] public PieceColor CurrentPlayerColor { get; private set; } = PieceColor.White;

        
        public Dictionary<Vector2Int, Piece> pieces = new();
        public Dictionary<Vector2Int, Tile> tiles = new();

        static int registeredPieces = 0;

        private void Update()
        {
            string selectionMsg = SelectedPiece != null
                ? $"Selected Piece: {SelectedPiece.name}"
                : $"No piece selected";
            log.warn(selectionMsg);
        }

        /// <summary>
        /// I think some of this is backward re: responsibilities
        /// between the BoardGenerator and the BoardManager.
        /// I feel like the generator should generate a board to give
        /// to the manager. 
        /// </summary>
        public void Initialize(
            Dictionary<Vector2Int, Tile> tiles,
            Dictionary<Vector2Int, Piece> pieces)
        {
            this.tiles = new(tiles);
            this.pieces = new(pieces);
            Initialized = true;
        }

        public bool IsValidKey(Vector2Int key)
        {
            return tiles.ContainsKey(key);
        }

        public bool IsEmptyAt(Vector2Int key)
        {
            return !TryGetPiece(key, out Piece piece);
        }

        public bool IsOpponentAt(Vector2Int key, Piece opponentOf)
        {
            return TryGetPiece(key, out Piece piece)
                && piece.color != opponentOf.color;
        }

        public bool RegisterPiece(Piece piece, out string msg)
        {
            msg = "";

            Vector2Int key = piece.BoardKey;

            Assert.IsNotNull(tiles);
            Assert.IsNotNull(pieces);
            Assert.IsTrue(tiles.Count == pieces.Count);
            Assert.IsTrue(tiles.ContainsKey(key));
            Assert.IsNotNull(piece);

            if (pieces[key] != null && pieces[key] != piece)
            {
                msg =
                    $"Couldn't register {piece.name} at {key}." +
                    $"{pieces[key].name} was already registered here.";
                return false;
            }

            // Register the new piece in the pieces dictionary
            pieces[key] = piece;
            pieces[key].BeenCaptured += HandleCapture;
            tiles[key].AddPiece(pieces[key]);
            registeredPieces++;
            return true;
        }

        public bool DeregisterPiece(Piece piece, out string msg)
        {
            msg = "";

            Vector2Int key = piece.BoardKey;

            if (!pieces.ContainsKey(key))
            {
                msg =
                    $"Couldn't deregister {piece.name} at {key}." +
                    $"no key at {key}";
                return false;
            }
            else if (pieces[key] != piece)
            {
                msg =
                    $"Couldn't deregister {piece.name} at {key}." +
                    $"{piece.name} is not registered to {key}";
                return false;
            }

            // Deregister the new piece from the pieces dictionary
            pieces[key].BeenCaptured -= HandleCapture;
            pieces[key] = null;
            tiles[key].RemovePiece();
            return true;
        }

        public void OnTileClicked(Tile clickedTile)
        {
            if (SelectedTile == null)
            {
                // turn owner's piece
                if (clickedTile.Occupant.color == CurrentPlayerColor)
                {
                    SelectedTile = clickedTile;
                    SelectedTile.Select();
                }
                else // opponent piece
                {
                    // display info
                }
            }
            else if (clickedTile == SelectedTile)
            {
                SelectedTile.Deselect();
                SelectedTile = null;

                // targetedTile.StopTargeting()
                // TargetedTile = null;
            }
            else if (SelectedPiece != null
                && SelectedPiece.GetValidMoves().Contains(clickedTile.BoardKey))
            {
                if (clickedTile.Occupant == null)
                {
                    MovePiece(SelectedPiece, clickedTile.BoardKey);
                    SwitchTurn();
                }
                else if (clickedTile.Occupant.color == SelectedPiece.color)
                {
                    SelectedTile.Deselect();
                    SelectedTile = clickedTile;
                    SelectedTile.Select();
                }
                else
                {
                    //List<Vector2Int> validMoves = SelectedPiece.GetValidMoves();
                    //Vector2Int enemyPos = clickedPiece.BoardKey;

                    //if (!validMoves.Contains(enemyPos))
                    //{
                    //    log.warn("Invalid capture: Enemy is not in range.");
                    //    return;
                    //}

                    /// Create an AttackCommand
                    /// - if the command would kill the piece,
                    ///   - issue the command.
                    ///   - capture target to attacker.
                    ///   - move attacker to tile.
                    /// - if not,
                    ///   - issue the command
                    ///
                    // If tile is valid, perform the attack
                    AttackCommand command = new(SelectedPiece, clickedTile.Occupant);
                    if (command.WouldKill())
                    {
                        command.Execute();
                    }
                    else
                    {
                        command.Execute();
                        SwitchTurn();
                    }

                    // TargetedTile = clickedTile;
                    // clickedTile.StartTargeting();

                    // hmm hmm mobile game... hmmmm...
                    // big dumb red fire button appears?? :)
                }
            }          
            else
            {
                /// TODO:
                /// Indicate that it was an invalid move attempt?
            }
        }

        //public void OnPieceClicked(Piece clickedPiece)
        //{
        //    SetOverlaysOnValidMoves(false);

        //    if (SelectedPiece == null)
        //    {
        //        if (clickedPiece.color == CurrentPlayerColor)
        //        {
        //            SelectPiece(clickedPiece);
        //        }
        //        else
        //        {
        //            TargetPiece(clickedPiece);
        //        }
        //    }
        //    else if (clickedPiece == SelectedPiece)
        //    {
        //        DeselectPiece();
        //    }
        //    else if (clickedPiece.color == SelectedPiece.color)
        //    {
        //        DeselectPiece();
        //        SelectPiece(clickedPiece);
        //    }
        //    else
        //    {
        //    }
        //}


        public bool TryGetTile(Vector2Int boardKey, out Tile tile)
        {
            if (!IsValidKey(boardKey))
            {
                log.warn(
                    $"Either you are trying to access a tile at " +
                    $"an invalid position, or there was an error " +
                    $"during board generation.");

                tile = null;
                return false;
            }
            else
            {
                Assert.IsNotNull(
                    tiles[boardKey],
                    $"{boardKey} is a valid BoardKey, but the tile there was" +
                    $"NULL. This probably means that there was an error " +
                    $"during board generation. ");

                tile = tiles[boardKey];
                return true;
            }
        }

        public bool TryGetPiece(Vector2Int boardKey, out Piece piece)
        {
            if (!IsValidKey(boardKey))
            {
                log.warn(
                    $"Either you are trying to access a piece at " +
                    $"an invalid tile position, or there was an error " +
                    $"during board generation.");

                piece = null;
                return false;
            }
            else if (pieces[boardKey] == null)
            {
                // Empty
                piece = null;
                return false;
            }
            else
            {
                // Occupied
                piece = pieces[boardKey];
                return true;
            }
        }

        public Tile GetTile(Piece piece)
        {
            Assert.IsTrue(
                IsValidKey(piece.BoardKey),
                $"{piece.BoardKey}'s BoardKey is invalid. Did you accidentally " +
                $"pass in a prefab?");

            Assert.IsNotNull(
                pieces[piece.BoardKey],
                $"{piece.BoardKey} is a valid BoardKey, but the piece there was" +
                $"NULL. This probably means that there was an error " +
                $"during board generation. ");

            Assert.IsTrue(
                pieces[piece.BoardKey] == piece,
                $"{pieces[piece.BoardKey]} was found at" +
                $"{piece.name}'s BoardKey ({piece.BoardKey}). Ensure that " +
                $"`Piece.SetBoardIndex(Vector2Int)` or `Piece.BoardKey = n` " +
                $"are only ever called once, during board generation.");

            return tiles[piece.BoardKey];
        }

        public Piece GetOccupant(Tile tile)
        {
            Assert.IsTrue(
                IsValidKey(tile.BoardKey),
                $"{tile.BoardKey}'s BoardKey is invalid. Did you accidentally " +
                $"pass in a prefab?");

            Assert.IsNotNull(
                tiles[tile.BoardKey],
                $"{tile.BoardKey} is a valid BoardKey, but the tile there was" +
                $"NULL. This probably means that there was an error " +
                $"during board generation. ");

            Assert.IsTrue(
                tiles[tile.BoardKey] == tile,
                $"{tiles[tile.BoardKey]} was found at" +
                $"{tile.name}'s BoardKey ({tile.BoardKey}). Ensure that " +
                $"`Tile.SetBoardIndex(Vector2Int)` or `Tile.BoardKey = n` " +
                $"are only ever called once, during board generation.");

            // If the way this operates is that we fill up the
            // pieces dictionary with a key at every position on the board,
            // with the keys where the tile has no occupant are null,
            // we check this way:
            Assert.IsTrue(
                pieces.ContainsKey(tile.BoardKey),
                $"The pieces dictionary is missing a key at {tile.BoardKey}. " +
                $"There should be a piece entry in the pieces dictionary at " +
                $"every position on the board." +
                $"Ensure that none are getting removed at any point.");

            return pieces[tile.BoardKey];

            // It's possible the above way is a lil janky? But idk.
            // If we switch to adding and removing pieces to the dictionary
            // whenever a piece is moved, then use this:
            //if (!pieces.ContainsKey(tile.BoardKey))
            //{
            //    // No occupant
            //    return null;
            //}

            //Assert.IsNotNull(
            //    pieces[tile.BoardKey],
            //    $"The pieces dictionary on {name} contained a null element. " +
            //    $"Ensure pieces are removed at the point that a piece moves" +
            //    $"or is captured.");

            //return pieces[tile.BoardKey];
        }

        public void MovePiece(Piece piece, Vector2Int targetPos)
        {
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
        public List<Vector2Int> GetMovesInDirection(Piece piece, Vector2Int direction)
        {
            List<Vector2Int> moves = new();
            Vector2Int keyToCheck = direction;

            while (IsValidMove(piece, keyToCheck))
            {
                moves.Add(keyToCheck);
                keyToCheck += direction;
            }

            return moves;
        }

        public void OverlaysOn(List<Vector2Int> moves)
        {
            log.print(
                $"Setting overlays ON. " +
                $"Valid moves count: {moves.Count}");

            foreach (Vector2Int pos in moves)
            {
                if (!tiles.TryGetValue(pos, out Tile tile)) continue;

                tile.SetOverlay();
            }
        }

        public void OverlaysOff(List<Vector2Int> moves)
        {
            log.print(
                $"Setting overlays OFF. " +
                $"Valid moves count: {moves.Count}");

            foreach (Vector2Int pos in moves)
            {
                if (!tiles.TryGetValue(pos, out Tile tile)) continue;

                tile.ClearOverlay();
            }
        }

        private void SwitchTurn()
        {
            SelectedTile?.Deselect();
            // TargetedTile?.Deselect();
            CurrentPlayerColor = CurrentPlayerColor == PieceColor.White
                ? PieceColor.Black
                : PieceColor.White;
        }

        public bool IsValidMove(Piece piece, Vector2Int key)
        {
            return IsValidKey(key) && (!TryGetPiece(key, out Piece pieceToCheck)
                || pieceToCheck?.color != piece.color);
        }

        private void HandleCapture(Piece piece)
        {
            SelectedPiece.Captures.Add(piece);

            if (CurrentPlayerColor == PieceColor.White)
            {
                /// TODO:
                /// Go to White's captured pieces
                /// For now,
                DestroyPieceAt(piece.BoardKey);
            }
            else
            {
                /// TODO:
                /// Go to Black's captured pieces
                /// For now,
                DestroyPieceAt(piece.BoardKey);
            }

            MovePiece(SelectedPiece, piece.BoardKey);
            SwitchTurn();
        }
    }
}