using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaseSystem
{
    public class PlayerSound : MonoBehaviour
    {
        PlayerMove pm;
        Rigidbody rb;

        private void Start()
        {
            pm = GetComponent<PlayerMove>();
            rb = GetComponent<Rigidbody>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                SoundManager.Instance.HitSE();
            }
        }
    }
}