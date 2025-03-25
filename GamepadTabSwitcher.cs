using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GamepadTabSwitcher : MonoBehaviour
{
    [SerializeField] private TabGroup tabGroup;
    [Header("Inventory Only")]
    [SerializeField] private SelectHandler selectHandler;

    void Update()
    {
        if(selectHandler != null)
        {
            if (InputTypeManager.instance.inputActions.UI.ChangeTabLeft.WasPressedThisFrame() && !selectHandler.itemPickedUp)
            {
                tabGroup.PreviousTab();
            }

            if (InputTypeManager.instance.inputActions.UI.ChangeTabRight.WasPressedThisFrame() && !selectHandler.itemPickedUp)
            {
                tabGroup.NextTab();
            }
        }
        else
        {
            if (InputTypeManager.instance.inputActions.UI.ChangeTabLeft.WasPressedThisFrame())
            {
                tabGroup.PreviousTab();
            }
            if (InputTypeManager.instance.inputActions.UI.ChangeTabRight.WasPressedThisFrame())
            {
                tabGroup.NextTab();
            }
        }
    }
}
