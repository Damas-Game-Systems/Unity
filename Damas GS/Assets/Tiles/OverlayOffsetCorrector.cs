using UnityEngine;

public class OverlayOffsetCorrector : MonoBehaviour
{
    // [Header("Offsets")]
    // [SerializeField] Vector2 offset;

    void Start()
    {
        Vector3 tileBorderOffset = (Vector3.one - transform.localScale) * .25f;

        transform.localPosition = transform.localPosition + tileBorderOffset;
    }
}
