using UnityEngine;

public static class ArrayExtensions
{
    /// <summary>
    /// Fisher-Yates shuffle the elements.
    /// </summary>
    public static void Shuffle<T>(this T[] items)
    {
        for (int i = 0; i < items.Length - 1; i++)
        {
            int j = Random.Range(i, items.Length);
            T swap = items[i];
            items[i] = items[j];
            items[j] = swap;
        }
    }
}
