using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private Transform m_target;

    private NavMeshAgent m_agent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        m_agent.SetDestination(m_target.position);
    }
}
