using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TowerBuilder : MonoBehaviour
{
    [SerializeField] public Button buildButton;
    private BuildableArea selectedArea;

    private void Start()
    {
        buildButton.gameObject.SetActive(false);
        buildButton.onClick.AddListener(BuildSelectedTower);
    }

    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (Input.GetMouseButtonDown(0))
        {
            HandleAreaSelection();
            TowerUIManager.Instance.HideInteract();
        }
        else if (Input.GetMouseButtonDown(1) && buildButton.gameObject.activeSelf)
        {
            ClearSelection();
        }
    }

    /// <summary>
    /// Selects a buildable area under the cursor
    /// </summary>
    private void HandleAreaSelection()
    {
        selectedArea = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit)
            ? hit.collider.GetComponent<BuildableArea>()
            : null;

        buildButton.gameObject.SetActive(selectedArea != null && selectedArea.CanBuildTower());
    }

    private void BuildSelectedTower()
    {
        if (selectedArea?.BuildTower() == true)
        {
            ClearSelection();
        }
        
    }


    private void ClearSelection()
    {
        buildButton.gameObject.SetActive(false);
        selectedArea = null;
    }
}
