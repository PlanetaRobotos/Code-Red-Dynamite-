using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Core.UI
{
    public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public event Action OnHold;

        private bool _isPressed;

        private void Update()
        {
            if(!_isPressed || OnHold is null) return;  
            OnHold.Invoke();
        }
        
        public void OnPointerDown(PointerEventData eventData) => 
            _isPressed = true;

        public void OnPointerUp(PointerEventData eventData) => 
            _isPressed = false;
    }
}