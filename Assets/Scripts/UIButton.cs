using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(Button))]
public class UIButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    private Button _button;

    public event Action OnMouseDown;

    public event Action OnMouseUp;



    public void Awake()
    {
        _button = GetComponent<Button>();
    }



    public void OnPointerUp(PointerEventData eventData)
    {
        OnMouseUp.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnMouseDown.Invoke();
    }
}
