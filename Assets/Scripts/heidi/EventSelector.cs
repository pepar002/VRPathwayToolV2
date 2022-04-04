using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;

public class EventSelector : MonoBehaviour
{
    [System.Serializable]
    public class ButtonEvent : UnityEvent { }

    public ButtonEvent pressedEvent;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.name == "Cone")
        {
            pressedEvent?.Invoke();
        }
    }
}
