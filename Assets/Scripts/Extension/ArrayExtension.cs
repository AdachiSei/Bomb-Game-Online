using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FourthTermPresentation.Extension
{
    /// <summary>
    /// スクリプト
    /// </summary>
    public static class ArrayExtension
    {
        public static void ForEach<T>(this T[] array, Action<T> action)
        {
            foreach (var item in array)
            {
                action(item);
            }
        }
    }
}