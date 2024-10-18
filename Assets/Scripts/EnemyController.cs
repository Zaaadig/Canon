using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private Transform m_target;
    [SerializeField] private Rigidbody m_rb;

    [Header("Values")]
    [SerializeField] private float m_acceleration;
    [SerializeField] private float m_maxSpeed;

    private NavMeshAgent m_agent;
    private NavMeshPath m_currentPath;
    private float m_pathTimer = 0;
    [SerializeField] private float m_pathDelay = 0.5f;
    void Start()
    {
        m_agent = GetComponent<NavMeshAgent>();
        m_agent.enabled = false;
        m_currentPath = new NavMeshPath();
    }

    void Update()
    {
        m_pathTimer += Time.deltaTime;

        if(m_pathTimer >= m_pathDelay)
        {
            m_pathTimer = 0;
            CalculatePath();
        }

        for (int i = 0; i < m_currentPath.corners.Length - 1; i++)
        {
            Debug.DrawLine(m_currentPath.corners[i], m_currentPath.corners[i + 1], Color.red);
        }
    }

    private void FixedUpdate()
    {
        if(m_currentPath.corners.Length > 0)
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
}
