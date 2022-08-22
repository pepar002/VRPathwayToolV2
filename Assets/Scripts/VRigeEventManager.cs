using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Xml;

public static class VRigeEventManager{


    //this class is no currently being used, but was created for early testing with events
    public static event UnityAction<float> SliderMoveX;

    public static event UnityAction PressPalmUpButton;
    public static event UnityAction PressPalmDownButton;
    public static event UnityAction PressPalmScaleUpButton;
    public static event UnityAction PressPalmScaleDownButton;
    public static event UnityAction<XmlDocument, string> PressPalmPyruvate;
    public static event UnityAction<XmlDocument, string> PressPalmGlycolysis;
    public static void OnPressPalmUpButton() => PressPalmUpButton?.Invoke();
    public static void OnPressPalmDownButton() => PressPalmDownButton?.Invoke();
    public static void OnPressPalmScaleUpButton() => PressPalmScaleUpButton?.Invoke();
    public static void OnPressPalmScaleDownButton() => PressPalmScaleDownButton?.Invoke();
    public static void OnPressPalmPyruvate(XmlDocument xml, string key) => PressPalmPyruvate?.Invoke(xml, key);
    public static void OnPressPalmGlycolysis(XmlDocument xml, string key) => PressPalmGlycolysis?.Invoke(xml, key);



    public static void OnSliderMoveX(float f) => SliderMoveX?.Invoke(f);
}

  
