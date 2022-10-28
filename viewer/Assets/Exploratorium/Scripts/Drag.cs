using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Exploratorium
{
    public class Drag : MonoBehaviour, IDragHandler
    {
        [SerializeField] private RectTransform rt;
        [SerializeField] private RectTransform target;

        public void OnDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, eventData.position, eventData.pressEventCamera,
                out var pos);
            target.anchoredPosition = new Vector3(pos.x, pos.y, 0);
        }
    }
}
