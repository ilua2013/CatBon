using System;
using System.Collections.Generic;

public static class ArrayExtensions
{
    public static T Random<T>(this T[] array)
    {
        return array[UnityEngine.Random.Range(0, array.Length)];
    }
    public static T Random<T>(this List<T> array)
    {
        return array[UnityEngine.Random.Range(0, array.Count)];
    }
    public static T[] SubArray<T>(this T[] array, int start, int length)
    {
        T[] result = new T[length];
        for (int i = 0; i < length; i++)
        {
            result[i] = array[start + i];
        }
        return result;
    }
    public static T[] SubArray<T>(this T[] array, int length)
    {
        T[] result = new T[length];
        for (int i = 0; i < length; i++)
        {
            result[i] = array[i];
        }
        return result;
    }
    public static List<T> SubArray<T>(this List<T> array, int start, int length)
    {
        List<T> result = new List<T>(length);
        for (int i = 0; i < length; i++)
        {
            result[i] = array[start + i];
        }
        return result;
    }
    public static List<T> SubArray<T>(this List<T> array, int length)
    {
        List<T> result = new List<T>(length);
        for (int i = 0; i < length; i++)
        {
            result[i] = array[i];
        }
        return result;
    }
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
    public static void Shuffle<T>(this T[] array)
    {
        int n = array.Length;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            T value = array[k];
            array[k] = array[n];
            array[n] = value;
        }
    }

    public static bool IsArrayOf<T>(this Type type)
    {
        return type == typeof(T[]);
    }
}
