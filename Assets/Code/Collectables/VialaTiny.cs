using Assets.Code.Interfaces;
using System;
using UnityEngine;

public class VialaTiny : MonoBehaviour, ICollectable
{
    static XPBarController _XPBarController;
    static long XP { get; set; } = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _XPBarController = GameObject.FindGameObjectWithTag("XPBar").GetComponent<XPBarController>();
    }

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
