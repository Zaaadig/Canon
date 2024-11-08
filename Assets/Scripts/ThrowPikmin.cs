using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform m_target;
    [SerializeField] private Rigidbody m_rb;

    [Header("Follow Settings")]
    [SerializeField] private float m_acceleration;
    [SerializeField] private float m_maxSpeed;
    [SerializeField] private float m_pathDelay = 0.5f;

    [Header("Projectile Settings")]
    [SerializeField] private Vector2 m_movementDuration;
    [SerializeField] private AnimationCurve m_animationCurve;
    [SerializeField] private AnimationCurve m_animationCurveHorizontal;
    [SerializeField] private Vector2 m_verticalOffsetRange;
    [SerializeField] private Vector2 m_horizontalOffsetRange;
    [SerializeField] private ParticleSystem m_explosionVFX;
    [SerializeField] private float m_returnDelay = 3f;
    [SerializeField] private float m_throwForce = 10f;

    private NavMeshAgent m_agent;
    private NavMeshPath m_currentPath;
    private float m_pathTimer = 0;
    private bool isThrown = false; // Pour savoir si l'ennemi est lanc�

    void Start()
    {
        m_agent = GetComponent<NavMeshAgent>();
        m_agent.enabled = false;
        m_currentPath = new NavMeshPath();
    }

    void Update()
    {
        if (isThrown) return; // Arr�te le suivi si l'ennemi est lanc�

        m_pathTimer += Time.deltaTime;
        if (m_pathTimer >= m_pathDelay)
        {
            m_pathTimer = 0;
            CalculatePath();
        }

        // Visualisation du chemin
        for (int i = 0; i < m_currentPath.corners.Length - 1; i++)
        {
            Debug.DrawLine(m_currentPath.corners[i], m_currentPath.corners[i + 1], Color.red);
        }

        // Condition pour lancer (appui sur espace si proche du joueur)
        if (Vector3.Distance(transform.position, m_target.position) <= 2f && Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }
    }

    private void FixedUpdate()
    {
        if (isThrown) return; // Pas de mouvement si l'ennemi est lanc�

        if (m_currentPath.corners.Length > 0)
        {
            Vector3 dir = m_currentPath.corners[1] - transform.position;
            dir = dir.normalized;

            m_rb.AddForce(dir * m_acceleration, ForceMode.Acceleration);
        }

        m_rb.linearVelocity = Vector3.ClampMagnitude(m_rb.linearVelocity, m_maxSpeed);
    }

    private void CalculatePath()
    {
        m_agent.enabled = true;
        m_agent.CalculatePath(m_target.position, m_currentPath);
        m_agent.enabled = false;
    }

    private void Shoot()
    {
        isThrown = true;
        m_rb.isKinematic = false; // Active le contr�le physique pour le lancement

        // Calcul de la direction vers le joueur et application de la force de lancement
        Vector3 directionToPlayer = (m_target.position - transform.position).normalized;
        m_rb.AddForce(directionToPlayer * m_throwForce, ForceMode.Impulse);

        Invoke(nameof(ReturnToPlayer), m_returnDelay); // Appelle le retour apr�s `m_returnDelay` secondes
    }

    private void DestroyProjectile()
    {
        if (m_explosionVFX != null)
        {
            m_explosionVFX.transform.parent = null;
            m_explosionVFX.Play();
        }
        // Au lieu de d�truire l'objet, on laisse l'effet visuel et on g�re le retour
    }

    private void ReturnToPlayer()
    {
        isThrown = false;
        m_rb.isKinematic = true; // R�active le contr�le physique pour le suivi
    }
}



