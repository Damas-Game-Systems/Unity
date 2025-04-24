using System.Collections.Generic;
using Damas.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Damas.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class PieceInfoWindow : MonoBehaviour
    {
        [Header("Debugging")]
        [SerializeField] private dbug log = new();

        [Header("Internal Refs")]
        [SerializeField] private Image bkg;
        [SerializeField] private TMP_Text pieceType;
        [SerializeField] private LabeledField pieceColor;
        [SerializeField] private LabeledField health;
        [SerializeField] private LabeledField attack;
        [SerializeField] private Sprite knightEffect, rookEffect, kingEffect, queenEffect;
        [SerializeField] private List<Image> effectImages;
        

        public RectTransform RT { get; private set; }

        [field: SerializeField, ReadOnly] public Piece Piece { get; private set; }
        [field: SerializeField, ReadOnly] public float timeRemaining { get; private set; }

        private void Awake()
        {
            RT = GetComponent<RectTransform>();
        }

        public bool Open(Piece piece, float duration)
        {
            bkg.enabled = false;
            pieceType.enabled = false;
            pieceColor.Off();
            health.Off();
            attack.Off();

            if (piece == null)
            {
                log.error(
                    $"Init failed on {name} because it " +
                    $"was passed a PieceInfoWindowData with a null Piece");
                return false;
            }
            else
            {
                this.Piece = piece;
                this.timeRemaining = duration;
            }

            pieceType.text = Piece.type.ToString();
            pieceColor.Value = Piece.color.ToString();
            health.Value = Piece.Health.CurrentValue.ToString();
            attack.Value = Piece.Attack.CurrentValue.ToString();
            UpdateEffects(piece);
            

            bkg.enabled = true;
            pieceType.enabled = true;
            pieceColor.On();
            health.On();
            attack.On();
            return true;
        }

        private void Update()
        {
            if (timeRemaining > -1 && timeRemaining <= 0)
            {
                UiManager.Instance.CloseWindow(this);
            }
            else if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
            }
        }
        
        public void UpdateEffects(Piece piece)
        {
            
            foreach (Image image in effectImages)
            {
                image.enabled = false;
                image.sprite = null;
            }

          
            int index = 0;
            if (piece.HasKnightBodyguard && index < effectImages.Count)
            {
                effectImages[index].sprite = knightEffect;
                effectImages[index].enabled = true;
                index++;
            }
            
            if (piece.IsLocked && index < effectImages.Count)
            {
                effectImages[index].sprite = queenEffect;
                effectImages[index].enabled = true;
                index++;
            }

            if (piece.HasRookBodyguard && index < effectImages.Count)
            {
                effectImages[index].sprite = rookEffect;
                effectImages[index].enabled = true;
                index++;
            }

            if (piece.HasKingBuff && index < effectImages.Count)
            {
                effectImages[index].sprite = kingEffect;
                effectImages[index].enabled = true;
                index++;
            }

           
        }
    }
}
