using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ProjectSurvival
{
    public enum ItemType
    {
        // Slot accept.
        None = 0, All = 1, 

        // Items.
        Consumable = 2, Weapon = 3, Ammo = 4, Storage = 5, Liquid = 6,
        Tool = 7, Campfire = 8, Furnace = 9
    }

    public class Inventory : MonoBehaviour
    {
        #region Variables.
        public static Inventory instance = null;

        [HideInInspector] public Player player = null;

        [HideInInspector] public bool inventoryOpen = false;

        // Prefabs.
        [SerializeField] public GameObject prf_Slot = null;

        // Panels.
        [HideInInspector] public Panel_Backpack panel_Backpack = null;
        [HideInInspector] public Panel_Options panel_Options = null;
        [HideInInspector] public Panel_Storage panel_Storage = null;
        [HideInInspector] public Panel_Fire panel_Fire = null;

        [HideInInspector] public Panel_Crafting panel_Crafting = null;
        [HideInInspector] public Panel_Description panel_Description = null;

        // Drag & Drop.
        private DragImage dragImage = null;
        private Slot startSlot = null;
        [HideInInspector] public Slot endSlot = null;
        [HideInInspector] public bool dragEnable = false;


        [Header("AudioClip :")]
        public AudioClip clip_StartCook;
        public AudioClip clip_EndCook;

        [HideInInspector] public AudioSource sourceAudio;
        #endregion

        #region Unity Methods.
        private void Awake()
        {
            if (instance == null)
                instance = this;

            if (GetComponent<Canvas>().isActiveAndEnabled)
                GetComponent<Canvas>().enabled = false;
            
        }

        private void Start()
        {
            sourceAudio = GameManager.CreateAudioSource(
                gameObject, string.Empty, null, false,false,0,1);
        }
        private void Update()
        {
            if (dragEnable)
            {
                dragImage.transform.localPosition = (Input.mousePosition)
                    - GetComponent<Canvas>().transform.localPosition;
            }
        }
        #endregion

        #region Init.
        public void Init(Player _player)
        {
            player = _player;

            panel_Backpack = GetComponentInChildren<Panel_Backpack>();
            panel_Backpack.Init();

            panel_Options = GetComponentInChildren<Panel_Options>();
            panel_Options.Init();

            panel_Storage = GetComponentInChildren<Panel_Storage>();
            panel_Storage.Init();

            panel_Fire = GetComponentInChildren<Panel_Fire>();
            panel_Fire.Init();

            panel_Crafting = GetComponentInChildren<Panel_Crafting>();
            panel_Crafting.Init();

            panel_Description = GetComponentInChildren<Panel_Description>();
            panel_Description.HideDescription();
            
            dragImage = transform.Find("DragImage").GetComponent<DragImage>();

        }
        #endregion

        #region Show Hide Inventory.
        public void ShowHide_Inventory()
        {
            inventoryOpen = !inventoryOpen;

            GetComponent<Canvas>().enabled = inventoryOpen;

            player.SetController(!inventoryOpen);
            GameManager.instance.SetCursor(inventoryOpen);

            panel_Options.Hide_Options();
            panel_Description.HideDescription();

            if (!inventoryOpen)
            {
                if (panel_Storage.gameObject.activeSelf)
                    panel_Storage.HidePanel();
                if (panel_Fire.gameObject.activeSelf)
                    panel_Fire.Hide();
                
            }
        }
        #endregion
        
        #region Items.
        public bool AddItem_Backpack(Item item)
        {

            return AddItems(item, panel_Backpack.grid);
        }

        public bool AddItem_Storage(Item item)
        {
            return AddItems(item, panel_Storage.gridSlots);
        }

        public void AddItem_Quickslots(Item item)
        {
            AddItems(item, Quickslot.instance.gridSlots);
        }

        private bool AddItems(Item item, Transform grid)
        {
            if (item == null || item.quantity <= 0)
                return false;


            List<Slot> listSlots = GetSlots(grid);
            if (listSlots.Count <= 0)
                return false;

            Slot slotFound = listSlots.FirstOrDefault(
                p => p.currentItem != null
                && p.currentItem.itemName == item.itemName
                && p.currentItem.Stackable
                && p.currentItem.quantity + item.quantity <= item.quantityMax);

            // slot trouvé, on stack notre item.
            if(slotFound != null)
            {
                slotFound.currentItem.quantity += item.quantity;
                slotFound.Refresh();
            }
            else // Pas de slots trouvés.
            {
                slotFound = listSlots.FirstOrDefault(p => p.currentItem == null);
                if(slotFound == null)
                {
                    //print("Inventory full !");
                    HUD.instance.SetMessage("Inventory Full !", Color.red);
                    player.EjectObject(item);
                    return true;
                }
                slotFound.ChangeItem(item);
            }


            HUD.instance.SetMessage(true, item);

            return true;
        }

        public void AddItemsEmptySlot(Item item)
        {
            if (item == null || item.quantity <= 0)
                return;

            List<Slot> listSlots = GetSlots(panel_Backpack.grid);
            if (listSlots.Count <= 0)
                return;

            Slot slotFound = listSlots.FirstOrDefault(p => p.currentItem == null);
            if(slotFound == null)
            {
                //print("Inventory full !");
                HUD.instance.SetMessage("Inventory Full !", Color.red);
                player.EjectObject(item);
                return;
            }

            slotFound.ChangeItem(item);
        }

        public int ReturnItemsQuantity(string _ItemName)
        {
            if (_ItemName == string.Empty)
                return 0;

            int qtFound = 0;

            // List slots.
            List<Slot> slots = GetAllSlots();
            if(slots.Count > 0)
            {
                for (int i = 0; i < slots.Count; i++)
                {
                    if (slots[i].currentItem != null && slots[i].currentItem.itemName == _ItemName)
                        qtFound += slots[i].currentItem.quantity;
                }
            }

            return qtFound;
        }
        #endregion

        #region Slots.
        public List<Slot> GetSlots(Transform grid)
        {
            if (grid == null || grid.childCount <= 0)
                return null;


            List<Slot> slots = new List<Slot>();

            for (int i = 0; i < grid.childCount; i++)
            {
                slots.Add(grid.GetChild(i).GetComponent<Slot>());
            }

            return slots;
        }

        public List<Slot> GetAllSlots()
        {
            
            List<Slot> slots = new List<Slot>();

            // Backpack.
            if (panel_Backpack.grid.childCount > 0)
            {
                for (int i = 0; i < panel_Backpack.grid.childCount; i++)
                {
                    slots.Add(panel_Backpack.grid.GetChild(i).GetComponent<Slot>());
                }
            }

            // Quickslots.
            Transform t = Quickslot.instance.gridSlots;
            if (t.childCount > 0)
            {
                for (int g = 0; g < t.childCount; g++)
                {
                    slots.Add(t.GetChild(g).GetComponent<Slot>());
                }
            }

            return slots;
        }

        public void DestroyAllObjects(Transform grid)
        {
            if (grid == null || grid.childCount <= 0)
                return;

            for (int i = 0; i < grid.childCount; i++)
            {
                Destroy(grid.GetChild(i).gameObject);
            }
        }

        public Slot CreateSlot(Transform grid)
        {
            if (grid == null)
                return null;

            return Instantiate(prf_Slot, grid).GetComponent<Slot>();
        }

        public void CreateSlots(Transform grid, int number)
        {
            if (grid == null || number <= 0)
                return;

            for (int i = 0; i < number; i++)
            {
                Instantiate(prf_Slot, grid);
            }
        }

        public void CreateSlots(Transform grid, List<Slot> slots)
        {
            if (grid == null || slots.Count <= 0)
                return;

            for (int i = 0; i < slots.Count; i++)
            {
                Slot slot = Instantiate(prf_Slot, grid).GetComponent<Slot>();
                slot.itemAccepted = slots[i].itemAccepted;
                slot.slotType = slots[i].slotType;
                slot.ChangeItem(slots[i].currentItem);
            }
        }
        #endregion

        #region Drag & Drop.
        public void StartDrag(Slot slot)
        {
            if (slot == null || slot.currentItem == null)
                return;

            dragEnable = true;
            dragImage.Refresh(slot.currentItem);

            startSlot = slot;
            startSlot.Set_SelectImage(true);
        }

        public void StopDrag()
        {
            if (startSlot != null)
                startSlot.Set_SelectImage(false);

            dragEnable = false;
            dragImage.Refresh(null);

            if (endSlot != null)
                ChangeItemSlot();
        }

        private void ChangeItemSlot()
        {


            if (startSlot == endSlot || startSlot.currentItem == null)
                return;

            
            ItemType startItemType = startSlot.currentItem.itemType;
            ItemType endSlotType = endSlot.itemAccepted;
            

            if(endSlotType == ItemType.All || 
                (endSlotType != ItemType.All && startItemType == endSlotType))
            {
                Item itemEndSlot = endSlot.currentItem;

                if(CheckItemSlot(endSlot, startSlot.currentItem) && 
                    CheckItemSlot(startSlot, endSlot.currentItem))
                {
                    // Same items.
                    if(itemEndSlot != null &&
                        (itemEndSlot.itemName == startSlot.currentItem.itemName)
                        && itemEndSlot.Stackable)
                    {
                        while (endSlot.currentItem.quantity < endSlot.currentItem.quantityMax)
                        {
                            if (startSlot.currentItem.quantity > 0)
                            {
                                //print("Add items endslots");
                                endSlot.currentItem.quantity++;
                                startSlot.currentItem.quantity--;
                            }
                            else
                                break;
                        }
                        
                        startSlot.Refresh();
                        endSlot.Refresh();

                        startSlot = null;
                        endSlot = null;

                        Quickslot.instance.Selection();
                        return;
                    }

                    endSlot.ChangeItem(startSlot.currentItem);
                    startSlot.ChangeItem(itemEndSlot);

                }
            }
            
            startSlot = null;
            endSlot = null;

            Quickslot.instance.Selection();
        }

        private bool CheckItemSlot(Slot slot, Item item)
        {
            if (item == null) return true;
            if (slot.itemAccepted == ItemType.All && slot.actionType == ActionType.None) return true;

            return (slot.itemAccepted == item.itemType || item.attribute.GetAction(slot.actionType) != null);
        }
        #endregion

        #region Other.
        public float GetPercent(float value, float max)
        {
            return (((value * 100) / max) / 100);
        }
        #endregion

    }
}