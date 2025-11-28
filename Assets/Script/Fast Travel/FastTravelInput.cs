using UnityEngine;

public class FastTravelInput : MonoBehaviour
{
    public FastTravelMenu fastTravelMenu; // gán trong inspector

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (fastTravelMenu.menuUI.activeSelf) fastTravelMenu.CloseMenu();
            else fastTravelMenu.OpenMenu();
        }
    }
}
