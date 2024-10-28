using TMPro;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PikminController : MonoBehaviour
{
    [SerializeField] private PlayerCanonController m_controller;
    [SerializeField] private EnemyController m_enemyController;
    [SerializeField] private Vector2 m_movementDuration;
    [SerializeField] private AnimationCurve m_animationCurve;
    [SerializeField] private AnimationCurve m_animationCurveHorizontal;
    [SerializeField] private Vector2 m_verticalOffsetRange;
    [SerializeField] private Vector2 m_horizontalOffsetRange;
    [SerializeField] private ParticleSystem m_VFX;
    [SerializeField] private ParticleSystem m_VFXDirt;
    [SerializeField] private float m_rangeLaunch = 1f;
    [SerializeField] private float m_rbDrag = 3f;
    [SerializeField] private float m_animationDelay = 3f;
    [SerializeField] private bool m_isShoot;
    [SerializeField] private bool m_isFollow;
    [SerializeField] private bool m_isComingBack;
    
    private NavMeshAgent m_agent; 
    private Rigidbody m_rb;

    public bool IsFollow { get => m_isFollow; set => m_isFollow = value; }

    private void Start()
    {
        m_agent = GetComponent<NavMeshAgent>();
        m_isShoot = false;
        m_isFollow = false;
        m_isComingBack = false;
        m_rb = GetComponent<Rigidbody>();
    }
    public void Shoot(Vector3 raycastHit)
    {
        if(m_isFollow == true)
        {
            StartCoroutine(C_Shoot(raycastHit));
        }
    }
    private IEnumerator C_Shoot(Vector3 m_raycastHit)
    {
        m_enemyController.enabled = false;
        m_isShoot = true;
        m_isFollow = false;
        m_isComingBack = false;
        float timer = 0;
        float duration = Random.Range(m_movementDuration.x, m_movementDuration.y);
        Vector3 startPosition = transform.position;
        float hOffset = Random.Range(m_horizontalOffsetRange.x, m_horizontalOffsetRange.y);
        float vOffset = Random.Range(m_verticalOffsetRange.x, m_verticalOffsetRange.y);

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float xOffset = m_animationCurveHorizontal.Evaluate(timer / duration) * hOffset;
            float yOffset = m_animationCurve.Evaluate(timer / duration) * vOffset;
            Vector3 wantedPos = Vector3.Lerp(startPosition, m_raycastHit, timer / duration);
            transform.position = wantedPos + (Vector3.right * xOffset) + (Vector3.up * yOffset);
            yield return new WaitForEndOfFrame();
            m_isComingBack = true;
        }
        transform.position = m_raycastHit;
        m_isShoot = false;
        m_VFX.Play();
        m_VFXDirt.Play();

        StartCoroutine(Delay());
        
    }

    private IEnumerator Delay()
    {
        m_rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        //jouer l'animation
        yield return new WaitForSeconds(m_animationDelay);
        m_enemyController.enabled = true;
        m_rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    private void Update()
    {
        if(Vector3.Distance(m_enemyController.m_target.position , transform.position) > m_rangeLaunch)
        {
            m_isComingBack = true;
            m_isFollow = false;
            m_rb.linearDamping = 0f;
        }
        else 
        {
            m_isComingBack = false;
            m_isFollow = true;
            m_rb.linearDamping = m_rbDrag;
        }
    }
}
