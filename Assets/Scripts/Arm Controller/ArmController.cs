using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectSurvival
{

    public class ArmController : MonoBehaviour
    {
        [SerializeField]
        private bool blockInteract = false;
        public bool BlockInteract { get { return blockInteract; } }



    }
}