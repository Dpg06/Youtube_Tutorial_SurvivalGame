using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectSurvival
{
    [RequireComponent(typeof(M_Resources))]
    [RequireComponent(typeof(M_Options))]
    [RequireComponent(typeof(M_Inputs))]
    public class GameManager : MonoBehaviour
    {

        public static GameManager instance = null;

        [HideInInspector] public M_Options options = null;
        [HideInInspector] public M_Inputs inputs = null;
        [HideInInspector] public M_Resources resources = null;

        

        public bool CheckHUDactive
        {
            get
            {
                if (Inventory.instance != null)
                    return Inventory.instance.inventoryOpen;
                else return false;


            }
        }

        private void Awake()
        {
            if(instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);


                options = GetComponent<M_Options>();
                inputs = GetComponent<M_Inputs>();
                resources = GetComponent<M_Resources>();
                resources.Init();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        
        private void Start()
        {
            SetCursor(false);
        }


        public void SetCursor(bool visible)
        {
            Cursor.visible = visible;
            Cursor.lockState = (visible) ? CursorLockMode.None : CursorLockMode.Locked;
        }

        public static AudioSource CreateAudioSource(
            GameObject toParent, string nameSource, AudioClip clip, 
            bool playeOnAwake, bool loop, float spacialBlend, float volume)
        {
            string newName = nameSource == string.Empty ? "AudioSource" : nameSource;
            GameObject go = new GameObject(newName);
            go.transform.parent = toParent.transform;
            go.transform.position = toParent.transform.position;

            AudioSource source = go.AddComponent<AudioSource>();
            source.clip = clip;
            source.playOnAwake = playeOnAwake;
            if (clip && playeOnAwake) source.Play();
            else source.Stop();

            source.loop = loop;
            source.spatialBlend = spacialBlend; // 0 = 2D, 1 = 3D
            source.volume = volume;

            return source;
        }

        public static void SetLayer(GameObject parent, int layer)
        {
            parent.layer = layer;

            Transform[] transforms = parent.GetComponentsInChildren<Transform>();
            if (transforms.Length > 0)
            {
                for (int i = 0; i < transforms.Length; i++)
                {
                    transforms[i].gameObject.layer = layer;
                }
            }
        }
    }
}