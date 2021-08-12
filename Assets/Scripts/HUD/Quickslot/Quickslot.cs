using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectSurvival
{

    public class Quickslot : MonoBehaviour
    {
        public static Quickslot instance = null;

        [SerializeField] private int numberSlots = 6;

        private Player player = null;
        [HideInInspector] public Transform gridSlots = null;
        private int slotID = 0;
        private bool initialized = false;

        [SerializeField] private float timer = 0;
        [SerializeField] private float delaySelection = 1;

        private ArmController currentArmController;
        public ArmController CurrentArm { get { return currentArmController; } }

        private void Awake()
        {
            if (instance == null)
                instance = this;

            gridSlots = transform.Find("GridSlots");
        }
        

        private void Update()
        {
            if (initialized && !GameManager.instance.CheckHUDactive)
            {
                if(timer <= 0)
                    Update_Inputs();

                if (timer > 0)
                    timer -= Time.deltaTime;
            }
        }


        public void Init(Player _player)
        {
            player = _player;
            
            StartCoroutine(CreateSlots());
        }

        private void Update_Inputs()
        {
            // Mouse ScrollWheel.
            if (GameManager.instance.options.inverseScrollWheel)
            {
                if (Input.GetAxis("Mouse ScrollWheel") > 0)
                {
                    if (slotID >= gridSlots.childCount - 1)
                        slotID = 0;
                    else slotID++;
                    Selection();

                }

                if (Input.GetAxis("Mouse ScrollWheel") < 0)
                {
                    if (slotID <= 0)
                        slotID = gridSlots.childCount - 1;
                    else slotID--;
                    Selection();
                }
            }
            else
            {
                if (Input.GetAxis("Mouse ScrollWheel") < 0)
                {
                    if (slotID >= gridSlots.childCount - 1)
                        slotID = 0;
                    else slotID++;
                    Selection();

                }

                if (Input.GetAxis("Mouse ScrollWheel") > 0)
                {
                    if (slotID <= 0)
                        slotID = gridSlots.childCount - 1;
                    else slotID--;
                    Selection();
                }
            }

            // Alpha Inputs.
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                slotID = 0;
                Selection();
            }
            if (Input.GetKeyDown(KeyCode.Alpha2) && gridSlots.childCount >= 2)
            {
                slotID = 1;
                Selection();
            }
            if (Input.GetKeyDown(KeyCode.Alpha3) && gridSlots.childCount >= 3)
            {
                slotID = 2;
                Selection();
            }
            if (Input.GetKeyDown(KeyCode.Alpha4) && gridSlots.childCount >= 4)
            {
                slotID = 3;
                Selection();
            }
            if (Input.GetKeyDown(KeyCode.Alpha5) && gridSlots.childCount >= 5)
            {
                slotID = 4;
                Selection();
            }
            if (Input.GetKeyDown(KeyCode.Alpha6) && gridSlots.childCount >= 6)
            {
                slotID = 5;
                Selection();
            }
        }


        public void Selection()
        {

            timer = delaySelection;

            if (gridSlots.childCount <= 0)
                return;

            for (int i = 0; i < gridSlots.childCount; i++)
            {
                if(i == slotID)
                {
                    Slot slot = gridSlots.GetChild(i).GetComponent<Slot>();
                    slot.Set_SelectImage(true);
                    Create_Arm(slot);

                }
                else
                {
                    gridSlots.GetChild(i).GetComponent<Slot>().Set_SelectImage(false);
                }
            }
        }


        private void Create_Arm(Slot slot)
        {
            Destroy_Arm();

            // Debug HUD.
            RaycastHit hit = Inventory.instance.player.fpsCamera.RaycastHit();
            HUD.instance.SceneObjectInfo(
                (hit.collider && hit.collider.GetComponent<Interact>())
                ? hit.collider.GetComponent<Interact>()
                : null);


            if (slot == null || slot.currentItem == null || slot.currentItem.quantity <= 0
                || slot.currentItem.prf_Arm == null)
                return;
            
            

            GameObject arm = Instantiate(slot.currentItem.prf_Arm, player.fpsCamera.armHolder);
            if (arm.GetComponent<WeaponController>())
                arm.GetComponent<WeaponController>().SetItem(slot.currentItem);
            if (arm.GetComponent<ToolController>())
                arm.GetComponent<ToolController>().SetItem(slot.currentItem);
            if (arm.GetComponent<BuildController>())
                arm.GetComponent<BuildController>().SetSlot(slot);

            currentArmController = arm.GetComponent<ArmController>();
            if (!currentArmController)
            {
                Debug.LogWarning($"The {arm.name} dont have 'ArmController script' !");
            }
        }

        private void Destroy_Arm()
        {
            Transform holder = player.fpsCamera.armHolder;
            if (holder.childCount > 0)
            {
                for (int i = 0; i < holder.childCount; i++)
                {
                    Destroy(holder.GetChild(i).gameObject);
                }

            }

            currentArmController = null;

            HUD_Weapon.instance.Refresh_Weapon(null);
            Crosshair.instance.ChangeCrosshair(CrosshairType.Pointer);
        }

        private IEnumerator CreateSlots()
        {
            if (numberSlots <= 0)
                yield return null;

            if(gridSlots.childCount > 0)
            {
                for (int g = 0; g < gridSlots.childCount; g++)
                {
                    Destroy(gridSlots.GetChild(g).gameObject);
                }
            }

            yield return new WaitForSeconds(.2f);

            int _nb = 1;
            for (int i = 0; i < numberSlots; i++)
            {
                Slot s = Instantiate(Inventory.instance.prf_Slot, gridSlots).
                    GetComponent<Slot>();

                s.slotType = SlotType.Quick;
                s.SetID(_nb.ToString());
                _nb++;
            }

            yield return new WaitForSeconds(.2f);

            player.GetComponent<StartItems>().Initialize();

            Selection();
            initialized = true;
        }

        public bool SetItemToSlot(Item item)
        {
            
            if (item == null || item.quantity <= 0)
                return false;

            for (int i = 0; i < gridSlots.childCount; i++)
            {
                if(i == slotID)
                {
                    Slot s = gridSlots.GetChild(i).GetComponent<Slot>();
                    if (s.currentItem != null)
                        Inventory.instance.player.EjectObject(s.currentItem);
                    s.ChangeItem(item);
                    Selection();

                    return true;
                }
            }
            
            return true;
        }
        
        public Slot GetSlotID()
        {
            for (int i = 0; i < gridSlots.childCount; i++)
            {
                if (slotID == gridSlots.GetChild(i).GetSiblingIndex())
                    return gridSlots.GetChild(i).GetComponent<Slot>();
            }

            return null;
        }
    }
}