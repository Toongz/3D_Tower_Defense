using UnityEngine;
using UnityEngine.EventSystems;
public class SpellTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private BaseSpell spell;
    [SerializeField] private Vector2 tooltipOffset = new Vector2(20, 20); 

    private bool isHovering = false;

    private void Start()
    {
        if (spell == null)
        {
            spell = GetComponent<BaseSpell>();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        if (spell != null)
        {
            string spellInfo = GetSpellInfo();
            Vector3 tooltipPosition = eventData.position + tooltipOffset;
            UIManager.Instance.ShowTooltip(spellInfo, tooltipPosition);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        UIManager.Instance.HideTooltip();

        CancelInvoke(nameof(CheckTooltipVisibility));
        Invoke(nameof(CheckTooltipVisibility), 1f);//0.05f
    }

    private void CheckTooltipVisibility()
    {
        if (!isHovering && UIManager.Instance.IsTooltipVisible())
        {
            UIManager.Instance.ForceHideTooltip();
        }
    }

    private void Update()
    {
        if (isHovering && UIManager.Instance != null && UIManager.Instance.IsTooltipVisible())
        {
            Vector3 tooltipPosition = Input.mousePosition + (Vector3)tooltipOffset;
            UIManager.Instance.UpdateTooltipPosition(tooltipPosition);
        }
    }

    private string GetSpellInfo()
    {
        string spellType = spell.GetType().Name;
        string baseName = spellType.Replace("Spell", "");

        string spellInfo = $"<b>{baseName} spell</b>\n" +
                           $"Price: {spell.Cost} gold\n" +
                           $"Radius: {spell.Radius}m";

        if (spell is FireSpell fireSpell)
        {
            int damage = 0;
            var damageField = typeof(FireSpell).GetField("damage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (damageField != null)
            {
                damage = (int)damageField.GetValue(fireSpell);
            }

            spellInfo += $"\nDamage: {damage}";
        }
        else if (spell is FreezeSpell freezeSpell)
        {
            var durationField = typeof(FreezeSpell).GetField("duration", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var slowFactorField = typeof(FreezeSpell).GetField("slowFactor", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (durationField != null && slowFactorField != null)
            {
                float duration = (float)durationField.GetValue(freezeSpell);
                float slowFactor = (float)slowFactorField.GetValue(freezeSpell);

                spellInfo += $"\nDuration: {duration}s" +
                             $"\nSlow: {slowFactor * 100}%";
            }
        }
        else if (spell is PoisonSpell poisonSpell)
        {
            // Get private fields using reflection
            var durationField = typeof(PoisonSpell).GetField("duration", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var damagePerSecondField = typeof(PoisonSpell).GetField("damagePerSecond", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (durationField != null && damagePerSecondField != null)
            {
                float duration = (float)durationField.GetValue(poisonSpell);
                int damagePerSecond = (int)damagePerSecondField.GetValue(poisonSpell);

                spellInfo += $"\nDuration: {duration}s" +
                             $"\nDamage/s: {damagePerSecond}";
            }
        }

        return spellInfo;
    }

    private void OnDisable()
    {
        // Đảm bảo tooltip bị ẩn khi component bị vô hiệu hóa
        if (UIManager.Instance != null)
        {
            UIManager.Instance.HideTooltip();
        }
    }
}