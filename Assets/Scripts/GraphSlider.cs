using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GraphSlider : MonoBehaviour
{
    public Slider slider;

    // Start is called before the first frame update
    void Start()
    {
        slider.onValueChanged.AddListener((v) =>
        {
            //VRigeEventManager.OnSliderMoveX(v);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
