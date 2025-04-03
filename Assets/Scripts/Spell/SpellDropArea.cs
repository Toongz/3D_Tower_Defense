using UnityEngine;
using UnityEngine.EventSystems;
using System;
using static Enum;

public class SpellDropArea : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public event Action<SpellType> OnSpellDropped;
    private bool isPointerOver;

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;

        if (eventData.pointerDrag.TryGetComponent<DragDropSpell>(out var draggedSpell))
        {
            OnSpellDropped?.Invoke(draggedSpell.spellType);
            Debug.Log($"Spell dropped: {draggedSpell.spellType}");
        }
    }

    public void OnPointerEnter(PointerEventData eventData) => isPointerOver = true;
    public void OnPointerExit(PointerEventData eventData) => isPointerOver = false;
    public bool IsPointerOver() => isPointerOver;
}
