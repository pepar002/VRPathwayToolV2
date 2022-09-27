using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Xml;

public static class VRigeEventManager{


    //this class is no currently being used, but was created for early testing with events
    public static event UnityAction<float> SliderMoveX;

    public static event UnityAction NodeMove;



    public static void OnSliderMoveX(float f) => SliderMoveX?.Invoke(f);

    public static void OnNodeMove() => NodeMove?.Invoke();
}

  
