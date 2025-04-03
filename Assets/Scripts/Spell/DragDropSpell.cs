using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Enum;
using System;

public class DragDropSpell : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public event Action<SpellType> OnSpellDragStarted;
    public event Action<SpellType> OnSpellDragEnded;

    [SerializeField] private SpellManager spellManager;
    [SerializeField] public SpellType spellType;
    [SerializeField] private Image dragImage;

    private Vector3 startPosition;

    private void Start()
    {
        ValidateComponents();
        dragImage.gameObject.SetActive(false);
    }

    private void ValidateComponents()
    {
        if (dragImage == null) Debug.LogError("DragDropSpell: dragImage not assigned!", this);
        if (spellManager == null) Debug.LogError("DragDropSpell: spellManager not assigned!", this);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPosition = transform.position;
        if (dragImage != null)
        {
            dragImage.sprite = GetComponent<Image>().sprite;
            dragImage.gameObject.SetActive(true);
        }

        float radius = spellManager.GetSpellPrefab(spellType).Radius;
        spellManager.StartSpellPreview(radius);
        OnSpellDragStarted?.Invoke(spellType);
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (dragImage == null) return;

        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            dragImage.rectTransform, eventData.position, eventData.pressEventCamera, out Vector3 worldPosition
        );
        dragImage.transform.position = worldPosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragImage != null)
        {
            dragImage.gameObject.SetActive(false);
        }

        spellManager.EndSpellPreview();

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, spellManager.groundLayer))
        {
            int spellCost = spellManager.GetSpellPrefab(spellType).Cost;
            if (GameManager.Instance.SpendGold(spellCost)) 
            {
                spellManager.SpawnSpellAtCursor(spellType);
            }
            else
            {
                Debug.LogWarning("Không đủ vàng để sử dụng spell sau khi kéo thả!");
                transform.position = startPosition; 
            }
        }
        else
        {
            Debug.LogWarning("DragDropSpell: Spell dropped outside valid area!", this);
            transform.position = startPosition;
        }

        OnSpellDragEnded?.Invoke(spellType);
    }

}
