using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectSurvival
{

    public class M_Resources : MonoBehaviour
    {
        #region Init.
        public void Init()
        {
            itemsDataBase = Resources.LoadAll<Item>("Scriptables/Items");
        }
        #endregion

        #region Items.
        private Item[] itemsDataBase = null;
        public Item GetItemByName(string _name)
        {
            if (itemsDataBase.Length <= 0)
                return null;

            for (int i = 0; i < itemsDataBase.Length; i++)
            {
                if(itemsDataBase[i].itemName == _name)
                {
                    return Instantiate(itemsDataBase[i]);
                }
            }

            return null;
        }
        #endregion

        #region Surfaces.
        [SerializeField] private Surface[] surfacesDataBase = null;
        public GameObject GetSurface(string _name)
        {
            if (_name == string.Empty || surfacesDataBase.Length <= 0)
                return null;

            for (int i = 0; i < surfacesDataBase.Length; i++)
            {
                if(surfacesDataBase[i].name == _name)
                {
                    return surfacesDataBase[i].bulletHole;
                }
            }

            return null;
        }


        public void CreateImpact(RaycastHit hit, bool addForceRigidbody)
        {
            if (hit.collider && hit.collider.sharedMaterial)
            {
                GameObject g = GameManager.instance.resources.GetSurface(
                    hit.collider.sharedMaterial.name);

                if (g != null)
                {
                    GameObject impact = Instantiate(g, hit.point + hit.normal * 0.001f,
                        Quaternion.LookRotation(hit.normal));

                    impact.transform.parent = hit.collider.transform;
                }
            }

            // Rigibody.
            if (addForceRigidbody && hit.rigidbody && hit.rigidbody.useGravity)
                hit.rigidbody.AddForce(-hit.normal * 100);
        }


        public void DeleteImpact(GameObject _parent)
        {
            if(_parent != null && _parent.transform.childCount > 0)
            {
                for (int i = 0; i < _parent.transform.childCount; i++)
                {
                    if (_parent.transform.GetChild(i).gameObject.tag == "Impact")
                        Destroy(_parent.transform.GetChild(i).gameObject);
                }
            }
        }
        #endregion

        #region Loots.
        [SerializeField] private LootTable[] lootTables = null;


        public List<Item> GetRandomLoot(string _lootType, int capacityMax)
        {
            
            // Get table by _lootType.
            LootTable table = new LootTable();

            if(lootTables.Length > 0)
            {
                for (int i = 0; i < lootTables.Length; i++)
                {
                    if (lootTables[i].lootType == _lootType)
                        table = lootTables[i];
                }
            }

            if(table.lootType == string.Empty)
            {
                return null;
            }


            List<Item> lootItems = new List<Item>();

            // Get items.
            if(table.itemNames.Length > 0)
            {
                for (int i = 0; i < table.itemNames.Length; i++)
                {
                    if (lootItems.Count < capacityMax)
                    {
                        Item item = GetItemByName(table.itemNames[i]);
                        if (item != null)
                        {
                            float randomPickItem = Random.Range(0, 100);

                            if (randomPickItem <= item.rarirtyLoot)
                            {
                                // Random stack.
                                if (item.Stackable)
                                {
                                    int randomStack = Random.Range(1, Mathf.RoundToInt(item.quantityMax / 2));
                                    Item itemStack = Instantiate(item);
                                    itemStack.quantity = randomStack;
                                    lootItems.Add(itemStack);
                                    //Debug.Log("Quantity as changed ! == " + randomStack);
                                }
                                else
                                {

                                    // The item pick for loot !
                                    lootItems.Add(item);
                                }
                            }

                        }
                    }
                }
            }

            return lootItems;
        }
        #endregion
    }

    [System.Serializable]
    public class Surface
    {
        public string name = string.Empty;
        public GameObject bulletHole = null;
    }


    [System.Serializable]
    public class LootTable
    {
        public string lootType = string.Empty;
        public string[] itemNames = null;
    }
}