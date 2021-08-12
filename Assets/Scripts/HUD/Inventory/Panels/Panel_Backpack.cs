using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectSurvival
{

    public class Panel_Backpack : MonoBehaviour
    {

        [HideInInspector] public Transform grid = null;


        public void Init()
        {
            grid = transform.Find("_GridSlots");
        }


    }
}
