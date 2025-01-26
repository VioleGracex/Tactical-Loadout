using UnityEngine;
using UnityEngine.EventSystems;

namespace Inventory
{
    public class DragDropItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public Transform originalParent;

        public void OnBeginDrag(PointerEventData eventData)
        {
            Slot slot = GetComponentInParent<Slot>();

            if (slot != null && slot.CanBeDragged())
            {
                originalParent = transform.parent;
                transform.SetParent(transform.root);
                GetComponent<CanvasGroup>().blocksRaycasts = false;
                Debug.Log($"Started dragging from slot {slot.id}");
            }
            else
            {
                Debug.Log("Cannot drag an empty slot.");
                eventData.pointerDrag = null; // Prevents dragging
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.pointerDrag != null)
            {
                transform.position = eventData.position;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            if (transform.parent == transform.root)
            {
                ResetToOriginalParent();
            }
        }

        public void ResetToOriginalParent()
        {
            transform.SetParent(originalParent);
            transform.localPosition = Vector3.zero;
            Debug.Log($"Returned to original slot");
        }
    }
}