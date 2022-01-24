using UnityEngine.UI;
using UnityEngine;
using System;
using SFB;

public class PathButton : MonoBehaviour
{
    [SerializeField] private ImageType.Type type;
    [SerializeField] private Text labelText;
    [SerializeField] private Text pathText;

    public event Action<ImageType.Type, string, string[]> onPathSetEvent;

    private string initLabelText;
    private string path = "";

    private void Awake()
    {
        initLabelText = labelText.text;
    }

    public void OnPress()
    {
        string[] paths = StandaloneFileBrowser.OpenFolderPanel("Select Folder", "", true);
        pathText.text = path = paths[0];

        labelText.text = initLabelText;

        string[] items = null;
        if (type != ImageType.Type.NONE)
        {
            items = PathHelper.GetDirectoryItems(path);
            labelText.text += " (" + items.Length + ")";
        }

        onPathSetEvent.Invoke(type, path, items);
    }

    public string GetFolderPath() { return path; }
    public ImageType.Type GetImageType() { return type; }
}