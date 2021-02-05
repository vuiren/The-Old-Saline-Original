using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UIMenuUIMove : MonoBehaviour
{
    [SerializeField] private int steps;
    void Update() 
    {
        var rectTrans = GetComponent<RectTransform>();
        var newPosition = rectTrans.position;
        if(newPosition.y < transform.parent.parent.GetComponent<RectTransform>().sizeDelta.y * -0.5f)
        {
            newPosition.y = rectTrans.position.y + steps * rectTrans.sizeDelta.y;
            rectTrans.position = newPosition;
        }
    }
}
