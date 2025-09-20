using Assets.Code.Interfaces;
using UnityEngine;

public class VialaTiny : MonoBehaviour, ICollectable
{
    private static float XP { get; set; } = 10f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ObjectPoolManager.ReturnObjectToPool(gameObject);
            GameManager.Player.GainXP(XP);
        }
    }
}
