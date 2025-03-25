using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectHandler : MonoBehaviour
{
    public Transform currentlySelectedSlotTransform;
    public Transform pickedUpItemStartTransform;
    public GameObject currentlySelectedItem;
    public Item pickedUpItem;
    public bool itemPickedUp = false;
    public bool equipableSlot;
    public bool emptySlot;
    private PlayerInputActions inputActions;
    [SerializeField] private TooltipHandler tooltipHandler;
    private InventoryScript inventoryScript;

    [SerializeField] private Transform ownedItemsParent;
    [SerializeField] private Transform equippedItemsParent;
    [SerializeField] private Transform equippedTrinketsParent;
    private List<Transform> ownedItems;
    public List<Transform> equippedItems;
    private List<Image> equippedSlotBorders;

    [SerializeField] private Color normalSlotColor, highlightSlotColor;

    private GameObject spawnedSelectedItem;
    public RectTransform spawnedRectTransform;
    public Sprite itemSprite;
    [SerializeField] private GameObject dragItem;
    [SerializeField] private Canvas canvas;

    private void Awake()
    {
        inputActions = InputTypeManager.instance.inputActions;
        inventoryScript = GameManager.instance.inventory;
        inventoryScript.selectHandler = this;

        ownedItems = new List<Transform>();
        equippedItems = new List<Transform>();
        equippedSlotBorders = new List<Image>();
        foreach (Transform child in ownedItemsParent) ownedItems.Add(child.Find("Borders").transform);
        foreach (Transform child in equippedItemsParent) equippedItems.Add(child.Find("Borders").transform);
        foreach (Transform child in equippedTrinketsParent) equippedItems.Add(child.Find("Borders").transform);
        foreach (Transform transform in equippedItems) { equippedSlotBorders.Add(transform.gameObject.GetComponent<Image>()); }
    }

    private void Update()
    {
        if(itemPickedUp)
        {
            if(inputActions.UI.Cancel.WasPressedThisFrame() || !InputTypeManager.instance.usingGamepad) //Drop item if changing input type to keyboardMouse
            {
                DropItem();
            }
        }
    }

    public void InventoryButtonClicked()
    {
        if (!itemPickedUp)
        {
            if (emptySlot)
            {
                return;
            }
            else
            {
                PickUpItem();
            }
        }
        else
        {
            if (equipableSlot)
            {
                EquipItem();
            }
            else
            {
                DropItem();
            }
        }
    }


    //Disable tooltip here, Display picked item above selected button
    public void PickUpItem()
    {
        DisableTooltip();
        itemPickedUp = true;
        DisplayItem();

        int index;
        if (ownedItems.Contains(currentlySelectedSlotTransform))
        {
            index = ownedItems.IndexOf(currentlySelectedSlotTransform);
            if (inventoryScript.ownedItems.Count > index) pickedUpItem = inventoryScript.DragHandlerItem(false, index);
        }
        //counts as unequip
        else if (equippedItems.Contains(currentlySelectedSlotTransform))
        {
            pickedUpItemStartTransform = currentlySelectedSlotTransform;
            index = equippedItems.IndexOf(currentlySelectedSlotTransform);
            if (inventoryScript.equippedItems.Count > index) pickedUpItem = inventoryScript.DragHandlerItem(true, index);
            inventoryScript.UnequipItemFromSlot(index);
            emptySlot = true;
            inventoryScript.SortInventory();
        }

        //Highlight proper slots
        if (pickedUpItem.equipAble)
        {
            switch (pickedUpItem.itemType)
            {
                case ItemType.Normal:
                    equippedSlotBorders[0].color = highlightSlotColor;
                    equippedSlotBorders[1].color = highlightSlotColor;
                    break;
                case ItemType.Special:
                    equippedSlotBorders[2].color = highlightSlotColor;
                    break;
                case ItemType.Trinket:
                    equippedSlotBorders[3].color = highlightSlotColor;
                    equippedSlotBorders[4].color = highlightSlotColor;
                    break;
            }
        }

        CheckSlot();
    }

    //Acts as a cancel and is also called when closing inventory or switching a tab, Enable tooltip of the current selected slot, Stop displaying picked up item
    public void DropItem()
    {
        if (itemPickedUp)
        {
            itemPickedUp = false;
            Destroy(spawnedSelectedItem);
            pickedUpItem = null;
            equipableSlot = false;
            pickedUpItemStartTransform = null;
            foreach (Image image in equippedSlotBorders) image.color = normalSlotColor;
            inventoryScript.SortInventory();
            EnableTooltip();
        }
    }

    //Enable Tooltip here of the current selected slot, Equipping acts as dropping item
    private void EquipItem()
    {
        int index = equippedItems.IndexOf(currentlySelectedSlotTransform);

        if (equippedItems.Contains(currentlySelectedSlotTransform) && pickedUpItem != null && equipableSlot) // EquipableSlot is checked OnSelect
        {
            if(SlotIsEmpty()) // If slot is empty -> check for duplicate in other equipped slot and unequip it. If no duplicates are found EQUIP the item into the slot
            {
                if(inventoryScript.equippedItems.Contains(pickedUpItem))
                {
                   int index2 = inventoryScript.equippedItems.IndexOf(pickedUpItem);
                   emptySlot = true;
                   inventoryScript.UnequipItemFromSlot(index2);
                }
                inventoryScript.EquipItemToSlot(pickedUpItem, index);
                emptySlot = false;
            }
            else // If slot is not empty (has an item equipped)
            {
                Item currentSlotItem = inventoryScript.DragHandlerItem(true, equippedItems.IndexOf(currentlySelectedSlotTransform));
                int indexSwap;

                if(currentSlotItem == pickedUpItem) // If item in the slot is the same as the one currently picked up, do nothing
                {
                    DropItem();
                    return;
                }
                else // If item is different than the picked up item
                {
                    if(pickedUpItemStartTransform != null) // Item has been picked up from an equipment slot -> SWAP EQUIPPED SLOTS
                    {
                        indexSwap = equippedItems.IndexOf(pickedUpItemStartTransform);
                        inventoryScript.EquipItemToSlot(currentSlotItem, indexSwap);
                        inventoryScript.EquipItemToSlot(pickedUpItem, index);
                    }
                    else // Item has been picked up from the inventory -> means that the same item might already be equipped in a slot
                    {
                        if(inventoryScript.equippedItems.Contains(pickedUpItem)) // If picked up item is equipped in another slot, get the index of the slot and SWAP EQUIPPED SLOTS
                        {
                            indexSwap = inventoryScript.equippedItems.IndexOf(pickedUpItem);
                            inventoryScript.EquipItemToSlot(currentSlotItem, indexSwap);
                            inventoryScript.EquipItemToSlot(pickedUpItem, index);
                        }
                        else // If picked up item is not equipped in any slot -> equip the picked up one
                        {
                            inventoryScript.EquipItemToSlot(pickedUpItem, index);
                        }
                    }
                }
            }
        }

        DropItem();
    }

    private bool SlotIsEmpty()
    {
        bool slotIsEmpty = true;
        Item swapItem = inventoryScript.DragHandlerItem(true, equippedItems.IndexOf(currentlySelectedSlotTransform));
        if(swapItem != null) {slotIsEmpty = false;}
        return slotIsEmpty;
    }

    public void EnableTooltip()
    {
        tooltipHandler.SetIgnoreState(false);
        tooltipHandler.AttemptTooltipEnable(currentlySelectedItem.GetComponent<TooltipTarget>());
    }

    public void DisableTooltip()
    {
        tooltipHandler.SetIgnoreState(true);
        tooltipHandler.ForceTooltipDisable();
    }

    private void DisplayItem()
    {
        itemSprite = currentlySelectedItem.GetComponent<DragObjectScript>().image.sprite;
        spawnedSelectedItem = Instantiate(dragItem, currentlySelectedSlotTransform.position + new Vector3(0f, 75f, 0f), Quaternion.identity);
        spawnedSelectedItem.transform.SetParent(GameObject.Find("DragItemAnchor").transform);
        spawnedSelectedItem.GetComponent<Image>().sprite = itemSprite;
        spawnedRectTransform = spawnedSelectedItem.GetComponent<RectTransform>();
    }

    public void MoveDisplayedItem()
    {
        if(spawnedSelectedItem != null)
        {
            spawnedRectTransform.position = currentlySelectedItem.GetComponent<RectTransform>().position + new Vector3(0f, 75f, 0f);
        }
    }

    public void DestroyDisplayedItem()
    {
        if(spawnedSelectedItem != null)
        {
            Destroy(spawnedSelectedItem);
        }
    }

    public void CheckSlot()
    {
        if (equippedItems.Contains(currentlySelectedSlotTransform))
        {
            switch (pickedUpItem.itemType)
            {
                case ItemType.Normal:
                    if (currentlySelectedSlotTransform == equippedItems[0] || currentlySelectedSlotTransform == equippedItems[1])
                    {
                        equipableSlot = true;
                    }
                    break;
                case ItemType.Special:
                    if (currentlySelectedSlotTransform == equippedItems[2])
                    {
                        equipableSlot = true;
                    }
                    break;
                case ItemType.Trinket:
                    if (currentlySelectedSlotTransform == equippedItems[3] || currentlySelectedSlotTransform == equippedItems[4])
                    {
                        equipableSlot = true;
                    }
                    break;
            }
        }
    }
    public TooltipVariables GetTooltipVariables()
    {
        TooltipVariables returnVariables = new TooltipVariables(null, null, ItemType.None, ItemRarity.Common);

        int tooltipIndex;
        Item itemInSlot;
        if (equippedItems.Contains(currentlySelectedSlotTransform))
        {
            tooltipIndex = equippedItems.IndexOf(currentlySelectedSlotTransform);
            if (inventoryScript.equippedItems.Count > tooltipIndex)
            {
                itemInSlot = inventoryScript.DragHandlerItem(true, tooltipIndex);
                if (itemInSlot != null)
                {
                    returnVariables.titleText = itemInSlot.itemName;
                    returnVariables.tooltipText = itemInSlot.tooltipText;
                    returnVariables.itemType = itemInSlot.itemType;
                    returnVariables.itemRarity = itemInSlot.itemRarity;

                }
            }
        }
        else if (ownedItems.Contains(currentlySelectedSlotTransform))
        {
            tooltipIndex = ownedItems.IndexOf(currentlySelectedSlotTransform);
            if (inventoryScript.ownedItems.Count > tooltipIndex)
            {
                itemInSlot = inventoryScript.DragHandlerItem(false, tooltipIndex);
                returnVariables.titleText = itemInSlot.itemName;
                returnVariables.tooltipText = itemInSlot.tooltipText;
                returnVariables.itemType = itemInSlot.itemType;
                returnVariables.itemRarity = itemInSlot.itemRarity;
            }
        }

        return returnVariables;
    }
}
