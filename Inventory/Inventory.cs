using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public ItemSO woodItem;
    public ItemSO axeItem;

    public GameObject hotbarObj;
    public GameObject inventorySlotParent;
    public GameObject container;

    public Image dragIcon;

    public float pickupRange = 3f;
    public Material highlightMaterial;
    private Material originalMaterial;
    private Renderer LookedAtRenderer = null;

    private int equippedHotbarIndex = 0; // 0-4
    public float equippedOpacity = 0.9f;
    public float normalOpacity = 0.58f;
    public Transform hand;
    private GameObject currentHandItem;

    private List<Slot> inventorySlots = new List<Slot>();
    private List<Slot> hotbarSlots = new List<Slot>();
    private List<Slot> allSlots = new List<Slot>();

    private Slot draggedSlot = null;
    private bool isDragging = false;

    private void Awake()
    {
        inventorySlots.AddRange(inventorySlotParent.GetComponentsInChildren<Slot>());
        hotbarSlots.AddRange(hotbarObj.GetComponentsInChildren<Slot>());

        allSlots.AddRange(inventorySlots);
        allSlots.AddRange(hotbarSlots);
    }

    private void Start()
    {
        // Start with inventory CLOSED
        container.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // ✅ FIXED inventory toggle logic
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            bool isOpen = !container.activeInHierarchy;
            container.SetActive(isOpen);

            if (isOpen)
            {
                // Inventory OPEN
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                // Inventory CLOSED
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        DetectLookedAtItem();
        pickup();

        StartDrag();
        UpdateDragItemPosition();
        EndDrag();

        HandleHotbarSelection();
        HandleDropEquippedItem();
        UpdateHotbarOpacity();
    }

    public void AddItem(ItemSO itemToAdd, int amount)
    {
        int remaining = amount;

        foreach (Slot slot in allSlots)
        {
            if (slot.HasItem() && slot.GetItem() == itemToAdd)
            {
                int currentAmount = slot.GetAmount();
                int maxStack = itemToAdd.maxStackSize;

                if (currentAmount < maxStack)
                {
                    int spaceLeft = maxStack - currentAmount;
                    int amountToAdd = Mathf.Min(spaceLeft, remaining);

                    slot.SetItem(itemToAdd, currentAmount + amountToAdd);
                    remaining -= amountToAdd;

                    if (remaining <= 0)
                        return;
                }
            }
        }

        foreach (Slot slot in allSlots)
        {
            if (!slot.HasItem())
            {
                int amountToPlace = Mathf.Min(itemToAdd.maxStackSize, remaining);

                slot.SetItem(itemToAdd, amountToPlace);
                remaining -= amountToPlace;

                if (remaining <= 0)
                    return;
            }
        }

        if (remaining > 0)
        {
            Debug.Log("Inventory Is full, could not add "
                      + remaining + " of " + itemToAdd.itemName);
        }
    }

    private void StartDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Slot hovered = GetHoverSlot();
            if (hovered != null && hovered.HasItem())
            {
                draggedSlot = hovered;
                isDragging = true;

                dragIcon.sprite = hovered.GetItem().icon;
                dragIcon.color = new Color(1, 1, 1, 0.5f);
                dragIcon.enabled = true;
            }
        }
    }

    private void EndDrag()
    {
        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            Slot hovered = GetHoverSlot();

            if (hovered != null)
            {
                HandleDrop(draggedSlot, hovered);

                dragIcon.enabled = false;
                draggedSlot = null;
                isDragging = false;
            }
        }
    }

    private Slot GetHoverSlot()
    {
        foreach (Slot s in allSlots)
        {
            if (s.hovering)
                return s;
        }
        return null;
    }

    private void HandleDrop(Slot from, Slot to)
    {
        if (from == to) return;

        if (to.HasItem() && to.GetItem() == from.GetItem())
        {
            int max = to.GetItem().maxStackSize;
            int space = max - to.GetAmount();

            if (space > 0)
            {
                int move = Mathf.Min(space, from.GetAmount());

                to.SetItem(to.GetItem(), to.GetAmount() + move);
                from.SetItem(from.GetItem(), from.GetAmount() - move);

                if (from.GetAmount() <= 0)
                    from.ClearSlot();

                return;
            }
        }

        if (to.HasItem())
        {
            ItemSO tempItem = to.GetItem();
            int tempAmount = to.GetAmount();

            to.SetItem(from.GetItem(), from.GetAmount());
            from.SetItem(tempItem, tempAmount);
            return;
        }

        to.SetItem(from.GetItem(), from.GetAmount());
        from.ClearSlot();
    }

    private void UpdateDragItemPosition()
    {
        if (isDragging)
        {
            dragIcon.transform.position = Input.mousePosition;
        }
    }

    private void pickup()
    {
        if (LookedAtRenderer != null && Input.GetKeyDown(KeyCode.E))
        {
            Item item = LookedAtRenderer.GetComponent<Item>();
            if (item != null)
            {
                AddItem(item.item, item.amount);

                //Quest line for collecting items
                QuestManager.instance.ItemCollected(item.item.itemName);
                Destroy(item.gameObject);
                EquipHandItem();
            }
        }
    }

    private void DetectLookedAtItem()
    {
        if (LookedAtRenderer != null && originalMaterial != null)
        {
            LookedAtRenderer.material = originalMaterial;
        }

        LookedAtRenderer = null;
        originalMaterial = null;

        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange))
        {
            Item item = hit.collider.GetComponentInParent<Item>();

            if (item != null)
            {
                Renderer rend = item.GetComponentInChildren<Renderer>();

                if (rend != null)
                {
                    originalMaterial = rend.material;
                    rend.material = highlightMaterial;
                    LookedAtRenderer = rend;
                }
            }
        }
    }

    private void UpdateHotbarOpacity()
    {
        for (int i = 0 ; i <hotbarSlots.Count; i++)
        {
            Image icon = hotbarSlots[i].GetComponent<Image>();
            if(icon != null)
            {
                icon.color = (i == equippedHotbarIndex)? new Color(1,1,1,equippedOpacity) : new Color(1,1,1,normalOpacity);
            }
        }
    }
    
    private void HandleHotbarSelection()
    {
        for(int i = 0; i<6 ; i++)
        {
            if(Input.GetKeyDown(( i+1 ).ToString()))
            {
                equippedHotbarIndex = i;
                UpdateHotbarOpacity();
                EquipHandItem();
            }
        }
    }

    private void HandleDropEquippedItem()
    {
        if (!Input.GetKeyDown(KeyCode.Q)) return;

        Slot equippedSlot =  hotbarSlots[equippedHotbarIndex]; 

        if(!equippedSlot.HasItem()) return;

        ItemSO itemSO = equippedSlot.GetItem();
        GameObject prefab = itemSO.itemPrefab;

        if(prefab == null) return; 

        GameObject dropped =Instantiate(prefab , Camera.main.transform.position + Camera.main.transform.forward, Quaternion.identity);

        Item item = dropped.GetComponent<Item>();
        item.item = itemSO;
        item.amount = equippedSlot.GetAmount();

        equippedSlot.ClearSlot();

        EquipHandItem();
    }

    private void EquipHandItem()
{
    if(currentHandItem != null) Destroy(currentHandItem);

    Slot equippedSlot = hotbarSlots[equippedHotbarIndex];

    if(!equippedSlot.HasItem()) return; // ✅ FIXED

    ItemSO item = equippedSlot.GetItem();

    if(item == null || item.handItemPrefab == null) return;

    currentHandItem = Instantiate(item.handItemPrefab, hand);
    currentHandItem.transform.localPosition = Vector3.zero;
    currentHandItem.transform.localRotation = Quaternion.identity;
}
}