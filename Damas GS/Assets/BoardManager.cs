using Damas.Combat;
using Damas.Utils;
using Damas.UI;
using System.Collections.Generic;
using UnityEngine;

namespace Damas
{
    public class BoardManager : Singleton<BoardManager>
    {
        [SerializeField] dbug log;

        //Keep track of whose turn it is 
        [SerializeField] public PieceColor currentPlayerColor = PieceColor.White;

        [Header("Scoring")]
        [SerializeField, ReadOnly] private List<Sprite> whiteCaptures = new();
        public int whiteScore { get { return whiteCaptures?.Count ?? 0; } }
        [SerializeField, ReadOnly] private List<Sprite> blackCaptures = new();
        public int blackScore { get { return blackCaptures?.Count ?? 0; } }

        [SerializeField] private int width;
        [SerializeField] private int height;

        public Dictionary<Vector2Int, Piece> pieces = new();
        public Dictionary<Vector2Int, Tile> tiles = new();

        // Currently selected piece
        [SerializeField]private Piece selectedPiece = null;
        
        public bool Initialized = false;

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
            Initialized = true;
        }

        public bool RegisterPiece(Piece piece, out string error)
        {
            error = "";

            Vector2Int key = piece.GetPositionData();

            if (!pieces.ContainsKey(key))
            {
                error =
                    $"Initializing key at  {key}.";

                pieces[key] = piece;
                return true;
            }
            else if (pieces[key] != null
                    && pieces[key] != piece)
            {
                if (pieces[key].Health.CurrentValue <= 0)
                {
                    DeregisterPiece(pieces[key], out string err);
                }
                else
                {
                    error =
                        $"Couldn't register {piece.name} at {key}." +
                        $"{pieces[key].name} was already registered here.";
                    return false;
                }
            }

            pieces[key] = piece;
            return true;
        }

        public bool DeregisterPiece(Piece piece, out string error)
        {
            error = "";

            Vector2Int key = piece.GetPositionData();

            if (!pieces.ContainsKey(key))
            {
                error =
                    $"Couldn't deregister {piece.name} at {key}." +
                    $"no key at {key}";
                return false;
            }
            else if (pieces[key] != piece)
            {
                error =
                    $"Couldn't deregister {piece.name} at {key}." +
                    $"{piece.name} is not registered to {key}";
                return false;
            }

            // Deregister the new piece from the pieces dictionary
            pieces[key] = null;
            return true;
        }

        public void OnPieceDestroyed(Piece piece)
        {
            if (!DeregisterPiece(piece, out string error))
            {
                log.error(error);
                return;
            }

            Sprite sprite = piece.GetComponent<SpriteRenderer>().sprite;
            if (piece.color == PieceColor.Black)
            {
                whiteCaptures.Add(sprite);
            }
            else if (piece.color == PieceColor.White)
            {
                blackCaptures.Add(sprite);
            }
            else
            {
                log.error("Piece was neither color");
            }
        }

        public void OnPieceClicked(Piece clickedPiece)
        {
            // SetOverlaysOnList(false);

            if (selectedPiece == null)
            {
                if (clickedPiece.color == currentPlayerColor)
                {
                    SelectPiece(clickedPiece);
                }
                else
                {
                    UiManager.Instance.OpenWindow(clickedPiece);
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
                List<Vector2Int> validMoves = selectedPiece.GetValidMoves();
                Vector2Int enemyPos = clickedPiece.GetPositionData();

                if (!validMoves.Contains(enemyPos))
                {
                    float windowDuration = 1.5f;
                    UiManager.Instance.OpenWindow(clickedPiece, windowDuration);
                    log.warn($"Invalid capture: Enemy is not in range. Opening window for {windowDuration}");
                    return;
                }

                selectedPiece.AttackPiece(clickedPiece);
                if (selectedPiece is Queen queen)
                {
                    queen.LockEnemy(clickedPiece);
                }

                // // If tile is valid, perform the attack
                // AttackCommand command = new(selectedPiece, clickedPiece);
                // command.Execute();
                SwitchTurn();
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
                selectedPiece.MoveTo(tilePos);
                SwitchTurn();
            }
            else
            {
                /// TODO:
                /// Indicate that it was an invalid move attempt?
            }
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
        }

        private void DeselectPiece()
        {
            log.print($"Deselecting piece: {selectedPiece}");

            selectedPiece?.Deselect();

            tiles[selectedPiece.BoardKey].ClearOverlay();
            selectedPiece = null;
        }

        private void DisplayPieceInfo(Piece piece)
        {
            /// TODO
        }

        /// <summary>
        /// Moves in a straight line until blocked or off-board.
        /// For Rooks/Bishops/Queens. kept this here for now might get moved to piece class - lee
        /// </summary>
        public List<Vector2Int> GetMovesInDirection(Piece piece, int dx, int dy)
        {
            if (piece == null)
            {
                Debug.LogError("GetMovesInDirection: The piece parameter is null.");
                return new List<Vector2Int>();
            }

            var moves = new List<Vector2Int>();
            int x = piece.X;
            int y = piece.Y;

            while (true)
            {
                x += dx;
                y += dy;

                // Stop if out of bounds
                if (!IsInBounds(x, y)) break;

                Vector2Int position = new Vector2Int(x, y);

                if (pieces.TryGetValue(position, out Piece occupant))
                {
                    if (occupant != null)
                    {
                        // Add as a valid move if it's an opponent piece
                        if (occupant.color != piece.color)
                        {
                            moves.Add(position);
                        }
                        // Stop further movement if the piece cannot move past others
                        break;
                    }
                }

                // Add empty square as a valid move
                moves.Add(position);
            }

            return moves;
        }

        public void SetOverlaysOnList(List<Vector2Int> positions, bool turnOn)
        {
            log.print(
                $"Setting overlays to {(turnOn ? "on" : "off")}.");

            foreach (Vector2Int pos in positions ?? new List<Vector2Int>())
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

        public bool IsInBounds(Vector2Int pos)
        {
            return IsInBounds(pos.x, pos.y);
        }

        private bool IsInBounds(int x, int y)
        {
            return x >= 0 && x < 8 && y >= 0 && y < 8;
        }

        private void SwitchTurn()
        {
            UiManager.Instance.CloseAllWindows();
            DeselectPiece();
            if (selectedPiece is King)
            {
                King king = selectedPiece as King;
                king.RemoveBuffFromAllies();
            }
            if (selectedPiece is Knight)
            {
                Knight knight = selectedPiece as Knight;
                knight.RemoveBuffFromAllies();
            }
            if (selectedPiece is Rook)
            {
                Rook rook = selectedPiece as Rook;
                rook.RemoveBuffFromAllies();
            }
            
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
    }
}
