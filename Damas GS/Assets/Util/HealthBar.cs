
using UnityEngine;
using UnityEngine.UI;

namespace Damas
{


    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Image healthFill; 
        [SerializeField]private Piece trackedPiece;

        private void Awake()
        {
            if (healthFill == null)
            {
                Debug.LogError("HealthFill is not assigned in the inspector.");
            }
            if (trackedPiece == null)
            {
                Debug.LogError("TrackedPiece is not assigned in the inspector.");
            }
            Initialize();
        }
        private void Initialize()
        {
            UpdateHealthBar();
            if (trackedPiece != null)
            {
                trackedPiece.Health.OnHealthChanged += HandleHealthChanged;
            }
        }

        private void HandleHealthChanged()
        {
            UpdateHealthBar();
        }

        private void UpdateHealthBar()
        {
            if (trackedPiece != null && healthFill != null)
            {
                float healthPercentage = (float)trackedPiece.Health.CurrentValue / trackedPiece.Health.MaxValue;
                healthFill.fillAmount = healthPercentage;
            }
        }

        private void OnDisable()
        {
            
            if (trackedPiece != null)
            {
                trackedPiece.Health.OnHealthChanged -= HandleHealthChanged;
            }
        }
    }
}