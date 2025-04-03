using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerVisualSelected : MonoBehaviour
{
    [SerializeField] Tower towerSelected;
    [SerializeField] GameObject visuaTower;

    private void Awake()
    {
        GameEvents.OnTowerSelected += TowerVisualSelectedChanged;
    }
    private void OnDisable()
    {
        GameEvents.OnTowerSelected -= TowerVisualSelectedChanged;
    }
    private void TowerVisualSelectedChanged(Tower tower)
    {
        if (tower != null && towerSelected == tower)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }   


    public void Show()
    {
        if (visuaTower != null)
            visuaTower.SetActive(true);
        else
            Debug.LogError("visuaTower chưa được gán!");
    }
    public void Hide()
    {
        if (visuaTower != null)
        {
            visuaTower.SetActive(false);
        }
        else
        {
            Debug.LogError("visuaTower đã bị mất tham chiếu!");
        }
    }

}
