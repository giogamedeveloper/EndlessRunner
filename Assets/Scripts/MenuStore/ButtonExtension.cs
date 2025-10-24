using System;
using UnityEngine;
using UnityEngine.UI;

public static class ButtonExtension
{
    /// <summary>
    ///Method that triggers an event when the button is pressed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="button"></param>
    /// <param name="param"></param>
    /// <param name="OnClick"></param>
    public static void AddEventListener<T>(this Button button, T param, Action<T> OnClick)
    {
        button.onClick.AddListener(delegate ()
        {
            OnClick(param);
        });
    }
}
