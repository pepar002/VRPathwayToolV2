using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class EventManager{

    public static event UnityAction PressPalmUpButton;
    public static event UnityAction PressPalmDownButton;
    public static event UnityAction PressPalmScaleUpButton;
    public static event UnityAction PressPalmScaleDownButton;
    public static void OnPressPalmUpButton() => PressPalmUpButton?.Invoke();
    public static void OnPressPalmDownButton() => PressPalmDownButton?.Invoke();
    public static void OnPressPalmScaleUpButton() => PressPalmScaleUpButton?.Invoke();
    public static void OnPressPalmScaleDownButton() => PressPalmScaleDownButton?.Invoke();
}

  
