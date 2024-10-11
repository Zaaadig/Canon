using System.Collections;
using UnityEngine;

public class CanonController : MonoBehaviour
{
    [SerializeField] private ProjectileController m_projectilePrefab;
    [SerializeField] private Vector2 m_cooldownRange;
    [SerializeField] private Transform m_shootPosition;
    [SerializeField] private BoxCollider m_boxCollider;

    private void Start()
    {
        StartCoroutine(C_Shoot());
    }

    private IEnumerator C_Shoot()
    {
        float cooldown = Random.Range(m_cooldownRange.x, m_cooldownRange.y);
        yield return new WaitForSeconds(cooldown);

        ShootProjectile();
        StartCoroutine(C_Shoot());
    }

    private void ShootProjectile()
    {
        Vector3 targetPos = RandomPointInBounds(m_boxCollider.bounds);
        ProjectileController newProjectile = CreateProjectile();
        newProjectile.Shoot(targetPos);
    }

    private ProjectileController CreateProjectile()
    {
        ProjectileController newProjectile = Instantiate(m_projectilePrefab);
        newProjectile.transform.position = m_shootPosition.position;
        return newProjectile;
    }

    public Vector3 RandomPointInBounds(Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
            );
    }
}
