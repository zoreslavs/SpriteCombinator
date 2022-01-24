using System.Collections.Generic;
using UnityEngine;

public static class ShuffleHelper
{
    public static void Shuffle(string[] array)
    {
        for (int t = 0; t < array.Length; t++)
        {
            string tmp = array[t];
            int r = Random.Range(t, array.Length);
            array[t] = array[r];
            array[r] = tmp;
        }
    }

    public static void Shuffle(List<string[]> list)
    {
        for (int t = 0; t < list.Count; t++)
        {
            string[] tmp = list[t];
            int r = Random.Range(t, list.Count);
            list[t] = list[r];
            list[r] = tmp;
        }
    }
}