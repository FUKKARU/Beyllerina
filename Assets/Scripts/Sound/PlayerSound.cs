using System.Collections;
using System.Collections.Generic;
using BaseSystem;
using UnityEngine;

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
            var state = pm.GetState();
            var opponentState = pm.GetOpponentState();

            switch (state)
            {
                case PlayerMove.PlayerState.IDLE:
                    break;
                case PlayerMove.PlayerState.PUSH:
                    break;
            }
        }

        if (collision.gameObject.CompareTag("Stage"))
        {

        }
    }
}