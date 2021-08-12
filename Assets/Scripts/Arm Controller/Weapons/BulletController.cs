using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectSurvival {

    public class BulletController : MonoBehaviour
    {
        [SerializeField] private float speed = 20;

        private Vector3 lastPos = new Vector3();

        void Start()
        {
            lastPos = transform.position;
            Destroy(gameObject, 100);
        }

        void Update()
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);

            RaycastHit hit;
            if (Physics.Linecast(lastPos, transform.position, out hit))
            {

                // Impact.
                GameManager.instance.resources.CreateImpact(hit, true);

                Destroy(gameObject);
            }

            lastPos = transform.position;
        }
    }
}
