using Damas.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PieceInfoPanel : MonoBehaviour
{
    [Header("Debugging")]
    [SerializeField] private dbug log = new();

    [Header("Internal Refs")]
    [SerializeField] private Image bkg;
    [SerializeField] private TMP_Text pieceType;
    [SerializeField] private LabeledField pieceColor;
    [SerializeField] private LabeledField health;
    [SerializeField] private LabeledField attack;
    
    private Piece piece;

    [field: SerializeField, ReadOnly] public bool IsOn { get; private set; }

    public void Initialize(Piece piece)
    {
        if (piece == null)
        {
            log.error($"Init failed on {name}");
            return;
        }

        this.piece = piece;

        pieceType.text = piece.type.ToString();
        pieceColor.Label = "Team";
        health.Label = "Health";
        attack.Label = "Attack Power";
    }

    private void Update()
    {
        if (piece.IsSelected && !IsOn)
        {
            On();
        }
        
        if (!piece.IsSelected && IsOn)
        {
            Off();
        }

        pieceColor.Value = piece.color.ToString();
        health.Value = piece.Health.CurrentValue.ToString();
        attack.Value = piece.Attack.CurrentValue.ToString();
    }

    public void On()
    {
        IsOn = true;

        bkg.enabled = true;
        pieceType.enabled = true;
        pieceColor.On();
        health.On();
        attack.On();
    }

    public void Off()
    {
        IsOn = false;

        bkg.enabled = false;
        pieceType.enabled = false;
        pieceColor.Off();
        health.Off();
        attack.Off();
    }
}
