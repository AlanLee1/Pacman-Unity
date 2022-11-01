using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BlinkSprite : MonoBehaviour
{
    public float Interval;
    private SpriteRenderer _spriteRenderer;
    private float _nexStateChange;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.enabled = true;
        _nexStateChange = Time.time + Interval;
    }

    private void Update()
    {
        if (Time.time > _nexStateChange)
        {
            _spriteRenderer.enabled = !_spriteRenderer.enabled;
            _nexStateChange = Time.time + Interval;
        }
    }

}
