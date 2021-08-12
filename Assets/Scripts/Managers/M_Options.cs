using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectSurvival
{

    public class M_Options : MonoBehaviour
    {

        [Tooltip("Gravity Earth = -9.81")]
        public float gravityEarth = -9.81f;

        [Header("Mouse :")]
        // User modifier.
        [SerializeField] private float userMouseSensi= 100;

        // Internal modifier.
        [HideInInspector] public float multiMouseSensi = 1;
        [HideInInspector] public float mouseSpeed_Default = 1;
        [HideInInspector] public float mouseSpeed_Zoom = .5f;

        public float MouseSensitivity => userMouseSensi * multiMouseSensi;

        [Header("Controls :")]
        public bool inverseScrollWheel = false;


        [Header("Outlines :")]
        public Outline.Mode outline_Mode = Outline.Mode.OutlineAll;
        public Color32 outline_Color = new Color32(255,0,0,255);
        public float outline_Width = 2f;

        

    }
}