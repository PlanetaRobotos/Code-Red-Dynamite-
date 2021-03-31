using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Core.UI
{
    public class HoldButtonImpulse : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public event Action OnHoldEvent;
        public event Action OnPointerUpEvent;
        public event Action OnPointerDownEvent;

        private bool _isPressed;

        private void Update()
        {
            if (!_isPressed || OnHoldEvent is null) return;
            OnHoldEvent?.Invoke();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _isPressed = true;
            OnPointerDownEvent?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isPressed = false;
            OnPointerUpEvent?.Invoke();
        }
    }
}