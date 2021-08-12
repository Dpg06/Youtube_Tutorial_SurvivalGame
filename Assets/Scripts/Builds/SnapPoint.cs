using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectSurvival {

    public class SnapPoint : MonoBehaviour
    {
        public enum E_SnapType { None, Foundation, Wall, Roof, }

        [SerializeField]
        private E_SnapType snapType = E_SnapType.None;
        public E_SnapType SnapType { get { return snapType; } }


        [SerializeField]
        private Vector3 snapRotation = new Vector3();
        public Vector3 SnapRotation { get { return snapRotation; } }
    }
}
