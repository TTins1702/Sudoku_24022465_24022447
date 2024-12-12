using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Hint : Selectable, IPointerClickHandler
{

    public void OnPointerClick(PointerEventData eventData)
    {
        GameEvents.OnHintMethod();

    }
}
