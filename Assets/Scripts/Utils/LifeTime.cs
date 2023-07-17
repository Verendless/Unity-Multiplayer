using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Utils
{
    public class LifeTime : MonoBehaviour
    {

        [SerializeField] private float lifeTime = 1f;

        public void Start()
        {
            Destroy(this.gameObject, lifeTime);
        }
    }
}

