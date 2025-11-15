using UnityEngine;

public class StaticProjectile : MonoBehaviour
{
    [SerializeField]
    private ProjectileType projectileType;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Collider2D collider = Physics2D.OverlapCircle(transform.position, transform.localScale.x);

        if (collider != null)
        {
            Debug.Log("Projectile hit target!");
        }
    }
}
