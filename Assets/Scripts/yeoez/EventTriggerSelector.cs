/**
 * Triggers an event when poked with a pointing gesture. 
 * Author: Elyssa Yeo
 * Date: 5 Jan 2021
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;

public class EventTriggerSelector : PokeSelector
{
    [System.Serializable]
    public class ButtonEvent : UnityEvent { }

    public ButtonEvent pressedEvent;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.name == "IndexTip" && ColliderHandIsPointing(collision))
        {
            pressedEvent?.Invoke();
        }
    }
}

