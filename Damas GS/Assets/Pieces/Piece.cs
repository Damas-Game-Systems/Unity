using Damas.Combat;
using Damas.UI;
using Damas.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Damas
{
    public enum PieceType { Pawn, Rook, Knight, Bishop, Queen, King }
    public enum PieceColor { White, Black }

    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Collider2D))]
    public abstract class Piece : MonoBehaviour, ISelectable
    {
        [SerializeField] private dbug log = new();

        [field: Space(10)]
        public PieceType type;
        public PieceColor color;

        [field: Space(10)]
        [SerializeField] private int maxHealth;
        [SerializeField] private int defaultAttack;

        private SpriteRenderer sRenderer;

        [field: Space(10)]
        public HealthStat Health { get; private set; }
        public AttackStat Attack { get; private set; }
        public List<Piece> Captures { get; private set; } = new();
        public bool IsLocked { get; set; } = false;

        // Current position on the board
        private int boardX;
        private int boardY;

        public int X => boardX;
        public int Y => boardY;
        public Vector2Int BoardKey => new(X, Y);

        [field: Space(10)]
        [field: ReadOnly] public bool IsRegistered { get; private set; }
        [field: ReadOnly] public bool HasMoved { get; private set; }
        [field: ReadOnly] public bool IsSelected { get; private set; }
        
        [field: Space(10)]
        [field: SerializeField] public bool canMovePastPieces { get; protected set; } = false;

        [field: Space(10)]
        [field: Header("Info Window")]
        [SerializeField] private float attackWindowPopupDuration;
        [field: ReadOnly] public PieceInfoWindow InfoWindow { get; private set; }

        public float OffsetY => sRenderer.sprite.bounds.extents.y;

        private void Awake()
        {
            sRenderer = GetComponent<SpriteRenderer>();
        }

        private void OnEnable()
        {
            Subscribe();
        }


        private void Update()
        {
            log.print(
                $"{gameObject.name}" +
                $"| Health: {Health.CurrentValue} Attack: {Attack.CurrentValue}");

            if (Health.CurrentValue <= 0)
            {
                sRenderer.color = Color.black;
            }

            if (IsLocked)
            {
                sRenderer.color = Color.cyan;
            }
        }

        public virtual void OnSpawn(Vector2Int spawnCoords)
        {
            MoveTo(spawnCoords);

            Health = new(maxHealth);
            Attack = new(defaultAttack);
            Debug.Log($"OnSpawn Called for {this.name} at {spawnCoords}");
            HasMoved = false;
        }

        public Vector2Int GetPositionData()
        {
            return new(boardX, boardY);
        }

        public void SetBoardIndex(Vector2Int pos)
        {
            boardX = pos.x; boardY = pos.y;
        }

        public void AttackPiece(Piece target)
        {
            if (target.GetAttacked(this))
            {
                MoveTo(BoardKey);
            }
        }

        public bool GetAttacked(Piece attacker)
        {
            int rawDamage = attacker.Attack.CurrentValue;
            int damageAfterRook =
                Rook.ApplyRookShieldIfInRange(this, rawDamage);
            int damageAfterKnight =
                Knight.ApplyKnightBodyguardIfInRange(this, damageAfterRook);

            Health.ReceiveDamage(damageAfterKnight);

            if (Health.CurrentValue <= 0)
            {
                OnKilledBy(attacker);
                return true;
            }

            UiManager.Instance.OpenWindow(this, attackWindowPopupDuration);

            return false;
        }

        public void ReceiveDeathRattle(int amount)
        {
            Health.ReceiveDamage(amount);
            if (Health.CurrentValue <= 0)
            {
                Destroy(gameObject);
            }
        }

        public void MoveTo(Vector2Int newPos)
        {
            string errorMsg = "";

            if (IsRegistered)
            {
                if (!BoardManager.Instance.DeregisterPiece(this, out errorMsg))
                {
                    log.error(errorMsg);
                    return;
                }
                HasMoved = true;
            }

            SetBoardIndex(newPos);

            if (!BoardManager.Instance.RegisterPiece(this, out errorMsg))
            {
                log.error(errorMsg);
                return;
            }

            IsRegistered = true;
            transform.position = new Vector2(newPos.x, newPos.y - OffsetY);
            if (BoardManager.Instance.Initialized)
            {
                OnAfterMove();
            }
        }

        /// <summary>
        /// for the bishop and king
        /// </summary>
        protected virtual void OnAfterMove()
        {
        }

        public virtual void OnKilledBy(Piece killer)
        {
            Destroy(gameObject);
        }

        public virtual List<Vector2Int> GetValidMoves()
        {
            // If locked, no moves
            if (IsLocked) return new List<Vector2Int>();

            //else just do normal moves
            return GetValidMovesInternal();
        }

        public abstract List<Vector2Int> GetValidMovesInternal();

        public void Select()
        {
            IsSelected = true;

            InfoWindow = UiManager.Instance.OpenWindow(this);
        }

        public void Deselect()
        {
            IsSelected = false;

            UiManager.Instance.CloseWindow(InfoWindow);
        }

        private void Subscribe()
        {
            // to anything in here
        }

        private void Unsubscribe()
        {
            // from anything in here
        }

        private void HandlePieceWindowOpened(PieceInfoWindow window)
        {
            if (window.Piece == this)
            {
                InfoWindow = window;
            }
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        protected virtual void OnDestroy()
        {
            BoardManager.Instance.OnPieceDestroyed(this);
        }
    } 
}
