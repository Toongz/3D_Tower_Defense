using UnityEngine;
using static Enum;

public class SpellManager : MonoBehaviour
{
    [SerializeField] public LayerMask groundLayer;
    [SerializeField] private GameObject firePotionPrefab;
    [SerializeField] private GameObject freezePotionPrefab;
    [SerializeField] private GameObject poisonPotionPrefab;
    [SerializeField] private GameObject spellPreviewPrefab;
    [SerializeField] private float temp;

    private GameObject currentPreview;
    private Vector3 lastHitPoint;

    private void Update()
    {
        if (!currentPreview) return;

        if (TryGetGroundPoint(out Vector3 hitPoint))
        {
            currentPreview.transform.position = hitPoint;
            lastHitPoint = hitPoint;
        }
    }

    private bool TryGetGroundPoint(out Vector3 point)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool hit = Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, groundLayer);
        point = hit ? hitInfo.point : lastHitPoint;
        return hit;
    }

    public void SpawnSpellAtCursor(SpellType spellType)
    {
        if (TryGetGroundPoint(out Vector3 position))
        {
            SpawnSpell(position, spellType);
        }
    }

    private void SpawnSpell(Vector3 position, SpellType spellType)
    {
        GameObject prefab = spellType switch
        {
            SpellType.Fire => firePotionPrefab,
            SpellType.Freeze => freezePotionPrefab,
            SpellType.Poison => poisonPotionPrefab,
            _ => null
        };

        if (prefab == null)
        {
            Debug.LogError($"SpellManager: No prefab defined for {spellType}");
            return;
        }

        GameObject spellObj = Instantiate(prefab, position, Quaternion.identity);
        if (spellObj.TryGetComponent<BaseSpell>(out var spell))
        {
                spell.Cast(position);
                GameEvents.OnSpellCasted?.Invoke(spellType);
        }
        else
        {
            Debug.LogError($"SpellManager: {spellObj.name} has no BaseSpell component!");
            Destroy(spellObj);
        }
    }

    public void StartSpellPreview(float radius)
    {
        if (spellPreviewPrefab == null)
        {
            Debug.LogError("SpellManager: spellPreviewPrefab is not assigned!");
            return;
        }

        EndSpellPreview();
        currentPreview = Instantiate(spellPreviewPrefab);
        currentPreview.transform.localScale = new Vector3(radius * temp, 1, radius* temp);
    }
    public void EndSpellPreview()
    {
        if (currentPreview != null)
        {
            Destroy(currentPreview);
            currentPreview = null;
        }
    }

    public BaseSpell GetSpellPrefab(SpellType spellType)
    {
        GameObject prefab = spellType switch
        {
            SpellType.Fire => firePotionPrefab,
            SpellType.Freeze => freezePotionPrefab,
            SpellType.Poison => poisonPotionPrefab,
            _ => null
        };

        return prefab != null ? prefab.GetComponent<BaseSpell>() : null;
    }
    private void OnDisable() => EndSpellPreview();
}
