using Damas.Utils;
using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Damas.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class PieceInfoWindow : Window
    {
        [Header("Debugging")]
        [SerializeField] private dbug log = new();

        [Header("Internal Refs")]
        [SerializeField] private Image bkg;
        [SerializeField] private TMP_Text pieceType;
        [SerializeField] private LabeledField pieceColor;
        [SerializeField] private LabeledField health;
        [SerializeField] private LabeledField attack;

        public RectTransform RT { get; private set; }

        [field: SerializeField, ReadOnly] public Piece Piece { get; private set; }

        private void Awake()
        {
            RT = GetComponent<RectTransform>();
        }
        private void OnEnable()
        {
            Hide();
        }

        public override bool Initialize(WindowData baseData)
        {
            Hide();
            if (baseData is not PieceInfoWindowData data)
            {
                log.error(
                    $"Init failed on {name} because it " +
                    $"was passed the wrong data");
                return false;

            }
            else if (data.piece == null)
            {
                log.error(
                    $"Init failed on {name} because it " +
                    $"was passed a PieceInfoWindowData with a null Piece");
                return false;
            }
            else
            {
                this.Piece = data.piece;
                RT.position = RectTransformUtility.WorldToScreenPoint(data.cam, data.worldSpaceOpenPos);
            }

            pieceType.text = Piece.type.ToString();
            pieceColor.Label = "Team";
            health.Label = "Health";
            attack.Label = "Attack Power";
            return true;
        }

        private void Update()
        {
            if (Piece == null)
            {
                log.error($"{name}'s Piece became null.");
                // Request Close
                return;
            }

            pieceColor.Value = Piece.color.ToString();
            health.Value = Piece.Health.CurrentValue.ToString();
            attack.Value = Piece.Attack.CurrentValue.ToString();
        }

        public void Show()
        {
            bkg.enabled = true;
            pieceType.enabled = true;
            pieceColor.On();
            health.On();
            attack.On();
        }

        public void Hide()
        {
            bkg.enabled = false;
            pieceType.enabled = false;
            pieceColor.Off();
            health.Off();
            attack.Off();
        }
    }

    public class PieceInfoWindowData : WindowData
    {
        public readonly Piece piece;

        public PieceInfoWindowData(
            Camera cam,
            Vector3 worldSpaceOpenPos,
            Piece piece)
                : base (cam, worldSpaceOpenPos)
        {
            this.piece = piece;
        }
    }
}