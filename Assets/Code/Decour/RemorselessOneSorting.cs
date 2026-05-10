using UnityEngine;

public class RemorselessOneSorting : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    private const float SortingPrecision = 10f;
    private const int SortingBase = 1000;


    void Start()
    {
        spriteRenderer.sortingOrder = SortingBase + 
            Mathf.RoundToInt(-transform.position.y * SortingPrecision);
    }
}
