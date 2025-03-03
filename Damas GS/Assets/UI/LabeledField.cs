using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LabeledField : MonoBehaviour
{
    [Header("Internal Refs")]
    [SerializeField] private Image labelBKG;
    [SerializeField] private TMP_Text label;
    [SerializeField] private Image valueBKG;
    [SerializeField] private TMP_Text value;

    public string Label
    {
        get
        {
            return this.label.text;
        }
        set
        {
            this.label.text = value;
        }
    }

    public string Value
    {
        get
        {
            return this.value.text;
        }
        set
        {
            this.value.text = value;
        }
    }

    public void On()
    {
        labelBKG.enabled = true;
        label.enabled = true;
        valueBKG.enabled = true;
        value.enabled = true;
    }

    public void Off()
    {
        labelBKG.enabled = false;
        label.enabled = false;
        valueBKG.enabled = false;
        value.enabled = false;
    }
}
