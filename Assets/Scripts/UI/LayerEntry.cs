using UnityEngine.UI;
using UnityEngine;
using System;

public class LayerEntry : MonoBehaviour
{
    [SerializeField] private InputField nameInput;
    [SerializeField] private Text pathText;
    [SerializeField] private Button selectButton;
    [SerializeField] private Button removeButton;
    [SerializeField] private Text orderText;

    public event Action onChanged;
    public event Action<LayerEntry> onRemoved;

    private string[] images;
    private bool nameEdited;

    public string[] Images => images;
    public bool HasImages => images != null && images.Length > 0;

    private void Awake()
    {
        selectButton.onClick.AddListener(SelectFolder);
        removeButton.onClick.AddListener(Remove);
        nameInput.onValueChanged.AddListener(OnNameChanged);
    }

    public void SetDefaultName(int order)
    {
        if (!nameEdited)
        {
            nameInput.SetTextWithoutNotify("Layer " + order);
        }
    }

    public void SetOrderLabel(string label)
    {
        orderText.text = label;
    }

    private void OnNameChanged(string value)
    {
        nameEdited = true;
    }

    private void SelectFolder()
    {
        string path = FolderPicker.Open("Select Folder");
        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        images = PathHelper.GetDirectoryItems(path);
        pathText.text = path + " (" + images.Length + ")";
        onChanged?.Invoke();
    }

    private void Remove()
    {
        onRemoved?.Invoke(this);
    }
}