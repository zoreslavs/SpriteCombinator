using UnityEngine.UI;
using UnityEngine;

public class ProgressBar : MonoBehaviour
{
    [Header("Settings:")]
    [SerializeField] private Image progressBar;
    [SerializeField] private float progressStep;

    private RectTransform barRT;
    private float barInitWidth;
    private float progress;

    private void Awake()
    {
        barRT = progressBar.GetComponent<RectTransform>();
        barInitWidth = barRT.sizeDelta.x;
        progress = 0;

        UpdateBar();
    }

    public void UpdateProgress(float value)
    {
        progress = value;

        UpdateBar();
    }

    private void UpdateBar()
    {
        if (barRT != null)
            barRT.sizeDelta = new Vector2(barInitWidth * progress, barRT.sizeDelta.y);
    }
}