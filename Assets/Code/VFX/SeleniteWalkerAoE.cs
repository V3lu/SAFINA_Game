using Assets.Code.Interfaces;
using UnityEngine;

public class SeleniteWalkerAoE : MonoBehaviour, IVFX
{
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
        if(collision.gameObject.TryGetComponent<PlayerCtrl>(out var player))
        {
            var playerCtrl = player as PlayerCtrl;
            playerCtrl.LooseHP(20);
        }
    }
}
