using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectSurvival
{
    [CreateAssetMenu(fileName = "I_", menuName = "Scriptables/Item/Storage")]
    public class Storage : Item
    {
        [Header("Storage :")]
        public int capacity = 12;
        public List<Slot> slotList = new List<Slot>();

        public bool useRandomLoot = true;                   // If can be create a random loot item list.
        public bool randomListCreated = false;
        public string lootType = "Military";
        public List<Item> lootItems = new List<Item>();

        public Storage()
        {
            this.itemType = ItemType.Storage;
            this.quantity = 1;
            this.quantityMax = 1;
            this.stackOnGround = false;
        }

        public void CreateRandomLoot()
        {
            if (useRandomLoot && !randomListCreated)
            {

                lootItems = GameManager.instance.resources.GetRandomLoot(lootType, capacity);
                //Debug.Log("Items looted = " + lootItems.Count);

                randomListCreated = true;
            }
        }
    }
}
