using UnityEngine.UI;
using UnityEngine;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private Image progressBar;

    private RectTransform barRT;
    private float barInitWidth;

    private void Awake()
    {
        barRT = progressBar.GetComponent<RectTransform>();
        barInitWidth = barRT.sizeDelta.x;
        UpdateBar(0);
    }

    public void UpdateProgress(float value)
    {
        UpdateBar(value);
    }

    private void UpdateBar(float progress)
    {
        if (barRT != null)
        {
            barRT.sizeDelta = new Vector2(barInitWidth * progress, barRT.sizeDelta.y);
        }
    }
}