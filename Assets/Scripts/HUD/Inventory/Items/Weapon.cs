using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectSurvival
{
    [CreateAssetMenu(fileName = "I_", menuName = "Scriptables/Item/Weapon")]
    public class Weapon : Item
    {

        [Header("Properties :")]
        [Header("WEAPON")]
        public float fireRate = .1f;
        public bool isAutomatic = true;
        public float recoilForce = .2f;
        public float crosshairForce = 10;
        [Header("Ammos :")]
        public int ammo = 20;
        public int ammoMax = 20;


        // Status.
        [HideInInspector] public bool isRealoding = false;
        [HideInInspector] public bool isZooming = false;

        public Weapon()
        {
            this.itemType = ItemType.Weapon;
        }
    }
}
