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
    [SerializeField] private float m_pathDelay = 0.5f;
    [SerializeField] private float m_closeDistance = 1;


    private NavMeshAgent m_agent;
    private NavMeshPath m_currentPath;
    private float m_pathTimer = 0;
    private float m_distanceToTarget;
    private float m_baseDrag;
    private bool m_isMoving = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_baseDrag = m_rb.linearDamping;
        m_agent = GetComponent<NavMeshAgent>();
        m_agent.enabled = false;
        m_currentPath = new NavMeshPath();
    }

    // Update is called once per frame
    void Update()
    {
        CalculateDistance();
        SetDrag();

        m_pathTimer += Time.deltaTime;

        if(m_pathTimer >= m_pathDelay)
        {
            m_pathTimer = 0;
            CalculatePath();
        }
    
        if(m_currentPath.corners.Length > 0)
        {
            for (int i = 0; i < m_currentPath.corners.Length - 1; i++)
            {
                Debug.DrawLine(m_currentPath.corners[i], m_currentPath.corners[i + 1], Color.red);
            }
        }
    }

    void CalculateDistance()
    {
        m_distanceToTarget = Vector3.Distance(transform.position, m_target.position);

        if (m_distanceToTarget <= m_closeDistance)
        {
            m_isMoving = false;
        }
        else
        {
            m_isMoving = true;
        }
    }

    void SetDrag()
    {
        if (m_isMoving)
            m_rb.linearDamping = 0;
        else
            m_rb.linearDamping = m_baseDrag;
    }

    private void OnDrawGizmosSelected()
    {
        if(m_currentPath.corners.Length > 0)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(m_currentPath.corners[1], 0.6f);
        }
    }

    private void FixedUpdate()
    {
        SetMaxSpeed();

        if (m_currentPath.corners.Length > 0)
        {
            Vector3 wantedPos = m_currentPath.corners[1];

            Vector3 dir = wantedPos - transform.position;
            dir = dir.normalized;

            m_rb.AddForce(dir * m_acceleration, ForceMode.Acceleration);            
        }

        if(!m_isMoving)
        {
            m_rb.linearVelocity *= 0.8f;
        }
    }

    private void SetMaxSpeed()
    {
        m_rb.linearVelocity = Vector3.ClampMagnitude(m_rb.linearVelocity, m_maxSpeed);
    }

    private void CalculatePath()
    {
        m_agent.enabled = true;
        m_agent.CalculatePath(m_target.position, m_currentPath);
        m_agent.enabled = false;

    }
}
