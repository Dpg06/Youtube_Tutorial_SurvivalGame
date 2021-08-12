using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectSurvival
{
    [RequireComponent(typeof(Rigidbody))]
    public class Interact : MonoBehaviour
    {
        public enum TypeAction { None, Pick, Interact, Resource }

        [SerializeField] private string itemName = string.Empty;
        [SerializeField] public TypeAction typeAction = TypeAction.Pick;
        [SerializeField] public bool movable = false;

        private Player player = null;
        private Rigidbody rb = null;

        private Outline[] outlines = null;
        private bool moving = false;
        [HideInInspector] public bool canHitObject = true;

        [HideInInspector] public Item refItem = null;
        [HideInInspector] public AudioSource audioS = null;

        private void Start()
        {
            Init();
        }

        private void OnDestroy()
        {
            if(HUD.instance != null)
                HUD.instance.SceneObjectInfo(null);
        }


        #region Initialize.
        private void Init()
        {
            GameManager.SetLayer(gameObject,13);


            audioS = GameManager.CreateAudioSource(
                gameObject, string.Empty, null, false, false, 1, 1) ;

            player = FindObjectOfType<Player>();
            rb = GetComponent<Rigidbody>();

            outlines = GetComponentsInChildren<Outline>();

            if (outlines.Length > 0)
            {
                for (int i = 0; i < outlines.Length; i++)
                {
                    outlines[i].OutlineMode = GameManager.instance.options.outline_Mode;
                    outlines[i].OutlineColor = GameManager.instance.options.outline_Color;
                    outlines[i].OutlineWidth = GameManager.instance.options.outline_Width;
                }
            }


            OutlinesEnable(false);
            GetItem();

            if (GetComponent<ResourceObject>())
                GetComponent<ResourceObject>().Initialize(this);
            if (GetComponent<FireController>())
                GetComponent<FireController>().Init(this);
        }
        
        private void OutlinesEnable(bool active)
        {


            if (outlines.Length > 0)
            {
                for (int i = 0; i < outlines.Length; i++)
                {
                    outlines[i].enabled = active;
                }
            }
        }

        
        #endregion


        #region Interaction.
        public void EventInteraction(bool enable)
        {
            //print("Tick Interaction");

            OutlinesEnable(enable);

            HUD.instance.SceneObjectInfo(enable ? this : null);
        }

        public void Update_Inputs()
        {
            if (moving)
            {
                if (Input.GetKeyDown(GameManager.instance.inputs.key_Eject))
                    Eject_Object();
                if (Input.GetKeyUp(GameManager.instance.inputs.key_MoveObject))
                    Drop_Object();
            }
            else
            {
                if (Input.GetKeyDown(GameManager.instance.inputs.key_MoveObject) && movable)
                    Move_Object();

                if(typeAction == TypeAction.Pick)
                {
                    if (Input.GetKeyDown(GameManager.instance.inputs.key_Interact1))
                        Action_PickItem();
                    if (Input.GetKeyDown(GameManager.instance.inputs.key_Interact2))
                        Action_EquipItem();
                }
                if(typeAction == TypeAction.Interact)
                {
                    if (Input.GetKeyDown(GameManager.instance.inputs.key_Interact1))
                        Action_Interact();
                }
            }
        }
        #endregion

        #region Actions.
        private void Action_PickItem()
        {
            HUD.instance.SceneObjectInfo(null);

            bool result = Inventory.instance.AddItem_Backpack(GetItem());
            if (result)
                Destroy(gameObject);


        }

        private void Action_EquipItem()
        {
            HUD.instance.SceneObjectInfo(null);

            bool result = Quickslot.instance.SetItemToSlot(GetItem());
            if (result)
                Destroy(gameObject);
        }

        private void Action_Interact()
        {
            HUD.instance.SceneObjectInfo(null);

            // Animations.
            if (GetComponentInChildren<Animator>())
                GetComponentInChildren<Animator>().SetBool("open", true);
            

            // Random loot.
            if (refItem != null && refItem.itemType == ItemType.Storage)
            {
                Storage storage = refItem as Storage;
                storage.CreateRandomLoot();

                // Show inventory.
                Inventory.instance.panel_Storage.ShowPanel(gameObject, refItem);
            }

            // Other Controller.
            if (GetComponent<FireController>())
                Inventory.instance.panel_Fire.Show(this);


        }

        /// <summary>
        /// FX Close the box
        /// </summary>
        public void Action_CloseBox()
        {
            // Animations.
            if (GetComponentInChildren<Animator>())
                GetComponentInChildren<Animator>().SetBool("open", false);
            
        }
        #endregion


        #region Move Object.
        // Bouge l'objet.
        private void Move_Object()
        {
            rb.isKinematic = true;
            transform.parent = player.fpsCamera.transform;
            moving = true;
        }

        // Lache l'objet.
        private void Drop_Object()
        {
            rb.isKinematic = false;
            transform.parent = null;
            moving = false;
        }

        // Eject l'objet.
        private void Eject_Object()
        {
            rb.isKinematic = false;
            transform.parent = null;
            moving = false;

            rb.AddForce(player.fpsCamera.transform.forward
                * (player.stats.ejectForce / rb.mass));
        }
        #endregion

        #region Items.
        private Item GetItem()
        {
            if (refItem == null)
            {
                refItem = GameManager.instance.resources.GetItemByName(itemName);
            }
            return refItem;
        }

        public void SetItem(Item item)
        {
            if (item == null)
                return;

            refItem = item;
        }
        #endregion

    }
}