using System.IO;

public static class PathHelper
{
    public static string[] GetDirectoryItems(string path)
    {
        DirectoryInfo dir = new DirectoryInfo(path);
        FileInfo[] info = dir.GetFiles("*.png");
        string[] items = new string[info.Length];

        for (int i = 0; i < items.Length; i++)
        {
            items[i] = info[i].ToString();
        }
        return items;
    }
}