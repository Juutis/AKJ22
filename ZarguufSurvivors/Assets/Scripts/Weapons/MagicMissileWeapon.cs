using UnityEngine;

public class MagicMissileWeapon : MonoBehaviour
{
    [SerializeField]
    private float cooldown;
    [SerializeField]
    private float projectileSpeed;
    [SerializeField]
    private GameObject projectilePrefab;

    private float lastShoot;
    private PlayerMovement player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lastShoot = 0;
        player = transform.parent.GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastShoot >= cooldown)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        GameObject newProjectile = Instantiate(projectilePrefab);
        newProjectile.GetComponent<StraightFlyingProjectile>().Init(player.MoveDir, projectileSpeed);

        Vector2 randomPos2 = Random.insideUnitCircle.normalized * 0.2f;
        Vector3 randomPos = new Vector3(randomPos2.x, randomPos2.y, 0);
        Vector3 offsetPos = new Vector3(player.MoveDir.x, player.MoveDir.y, 0) * 0.5f;

        newProjectile.transform.position = transform.position + offsetPos + randomPos;
        lastShoot = Time.time;
    }
}
