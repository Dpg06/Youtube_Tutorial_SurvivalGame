using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectSurvival
{
    [CreateAssetMenu(fileName = "I_",menuName = "Scriptables/Item/DefaultItem")]
    public class Item : ScriptableObject
    {
        public string itemName = string.Empty;
        public string itemDesc = string.Empty;
        public Sprite itemIcon = null;

        [Min(1)]
        public int quantity = 1;
        [Min(1)]
        public int quantityMax = 1;
        public bool Stackable { get { return quantityMax > 1; } }
        public bool stackOnGround = false;
        
        
        public ItemType itemType = ItemType.None;

        [Header("Rarity :")]
        public float rarirtyLoot = 10;          // In percent.

        [Header("Attribute :")]
        public Attribute attribute = new Attribute();
        public string[] interactWith = null;
        public bool InteractWith(string _itemName)
        {
            if (_itemName == string.Empty || interactWith.Length <= 0)
                return false;

            for (int i = 0; i < interactWith.Length; i++)
            {
                if (interactWith[i] == _itemName)
                    return true;
            }

            return false;
        }

        [Header("Prefabs :")]
        public GameObject prf_Ground = null;
        public GameObject prf_Arm = null;

        [Header("Crafting :")]
        public Crafting crafting = new Crafting();

        [Header("Build :")]
        public Build build = new Build();
    }

    [System.Serializable]
    public class Attribute
    {
        // Valeurs.
        public string name = string.Empty;
        public string type = string.Empty;
        public string quality = string.Empty;

        public float value = 0;
        public float max = 0;

        public string changeItemName = string.Empty;
        
        public float GetPercent()
        {
            return Inventory.instance.GetPercent(value, max);
        }
        public void DecreaseValue()
        {
            value--;
            if (value <= 0)
            {
                value = 0;

                if(name == "Water")
                {
                    quality = string.Empty;
                }
                if (name == "Break")
                    quality = "Breaked";
            }
        }
        public void IncreaseValue()
        {
            value++;
            if(value > max)
            {
                value = max;
            }
        }
        public void Reset()
        {
            value = max;
        }

        public bool ActiveBarreHorizontal()
        {
            return name == "Break";
        }
        public bool ActiveBarreVertical()
        {
            return (name == "Water");
        }

        public bool deleteItemAfterAction = false;



        public Action[] actions = null;
        public Action GetAction(ActionType _type)
        {
            if (_type == ActionType.None || actions.Length <= 0)
                return null;

            for (int i = 0; i < actions.Length; i++)
            {
                if (actions[i].type == _type)
                    return actions[i];
            }

            return null;
        }
    }


    public enum ActionType { None, Cook, StartFire, FuelFire , Eat, Drink, Use, Break}

    [System.Serializable]
    public class Action
    {
        public ActionType type = ActionType.None;
        public string fonction = string.Empty;
        public float value = 0;
    }

    [System.Serializable]
    public class Crafting
    {
        public bool isCraftable = false;
        [Min(1)]
        public int resultQuantity = 1;
        [Min(1)]
        public float timeToCraft = 1;
        [HideInInspector] public float timer = 0;

        // Require items.
        public RequireItem[] requireItems = null;
    }

    [System.Serializable]
    public class RequireItem
    {
        public string requireName = string.Empty;
        [Min(1)]
        public int requireQuantity = 1;
    }

    [System.Serializable]
    public class Build
    {
        [SerializeField]
        private GameObject buildObject;
        public GameObject BuildObject { get { return buildObject; } }

        [SerializeField]
        private SnapPoint.E_SnapType snapType = SnapPoint.E_SnapType.None;
        public SnapPoint.E_SnapType SnapType { get { return snapType; } }

        [SerializeField]
        private bool snapRequired;
        public bool SnapRequired { get { return snapRequired; } }

        [Space()]
        [SerializeField]
        private bool canBeRotate;
        public bool CanBeRotate { get { return canBeRotate; } }

        [SerializeField]
        private float angleRotation = 45;
        public float AngleRotation { get { return angleRotation; } }


    }
}