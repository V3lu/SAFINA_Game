using Assets.Code.Interfaces;
using System;
using UnityEngine;

public class VialaTiny : MonoBehaviour, ICollectable
{
    static long XP { get; set; } = 1;

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ObjectPoolManager.ReturnObjectToPool(gameObject);
            GameManager.Player.GainXP(XP);
        }
    }
}
