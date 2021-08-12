using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectSurvival
{

    public class M_Inputs : MonoBehaviour
    {
        public KeyCode key_PrimaryAction = KeyCode.Mouse0;
        public KeyCode key_SecondaryAction = KeyCode.Mouse1;
        public KeyCode key_Reload = KeyCode.R;

        public KeyCode key_Interact1 = KeyCode.F;
        public KeyCode key_Interact2 = KeyCode.E;

        public KeyCode key_MoveObject = KeyCode.Mouse2;
        public KeyCode key_Eject = KeyCode.G;

        public KeyCode key_Inventory = KeyCode.Tab;

        // Build.
        public KeyCode key_BuildRotateRight = KeyCode.E;
        public KeyCode key_BuildRotateLeft = KeyCode.A;
    }
}