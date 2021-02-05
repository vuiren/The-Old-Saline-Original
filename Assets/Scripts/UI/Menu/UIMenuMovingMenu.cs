using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMenuMovingMenu : MonoBehaviour
{
    [SerializeField] private GameObject paper_1, paper_2, paper_3, mainMenu;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float newMovingMenuPosition;
    private bool isMove = true;


    void Start()
    {
        var movingMenuRectTrans = GetComponent<RectTransform>();
        newMovingMenuPosition = movingMenuRectTrans.localPosition.y - movingMenuRectTrans.sizeDelta.y;
    }

    void Update() 
    {
        if(isMove)
        {
            var movingMenuRectTrans = GetComponent<RectTransform>();
            var movingMenuRectTransPosition = movingMenuRectTrans.localPosition;
            movingMenuRectTransPosition.y = Mathf.MoveTowards(movingMenuRectTrans.localPosition.y, newMovingMenuPosition, moveSpeed);
            movingMenuRectTrans.localPosition = movingMenuRectTransPosition;
            isMove = movingMenuRectTrans.localPosition.y > newMovingMenuPosition;
        }
    }

    public void StartMove()
    {
        var movingMenuRectTrans = GetComponent<RectTransform>();
        newMovingMenuPosition = movingMenuRectTrans.localPosition.y - movingMenuRectTrans.sizeDelta.y;
        isMove = true;
    }
}
