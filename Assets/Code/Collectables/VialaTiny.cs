using Assets.Code.Interfaces;
using UnityEngine;

public class VialaTiny : MonoBehaviour, ICollectable
{
    private static XPBarController _XPBarController;
    private static double XP { get; set; } = 0.01;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _XPBarController = GameObject.FindGameObjectWithTag("XPBar").GetComponent<XPBarController>();
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
