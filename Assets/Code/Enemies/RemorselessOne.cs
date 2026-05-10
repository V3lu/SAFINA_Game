using UnityEngine;

public class RemorselessOne : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;

    float SortingPrecision = 10f;
    private const int SortingBase = 2000;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LateUpdate()
    {
        spriteRenderer.sortingOrder = SortingBase +
            Mathf.RoundToInt(-transform.position.y * SortingPrecision);
    }
}
