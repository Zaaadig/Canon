using TMPro;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PikminController : MonoBehaviour
{
    [SerializeField] private GameObject m_self;
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
    [SerializeField] private float m_animationDelay = 2f;
    [SerializeField] private float m_comingBackSpeed = 7f;
    [SerializeField] private float m_accelerationSpeed = 25f;
    [SerializeField] private bool m_isShoot;
    [SerializeField] private bool m_isFollow;
    [SerializeField] private bool m_isComingBack;
    public Vector3 m_wantedPos;
    
    private NavMeshAgent m_agent; 
    private Rigidbody m_rb;
    private Coroutine m_shootCoroutine;
    //public CapsuleCollider m_capsuleCollider;

    public bool IsFollow { get => m_isFollow; set => m_isFollow = value; }
    public bool IsShoot { get => m_isShoot; set => m_isShoot = value; }
    public bool IsComingBack { get => m_isComingBack; set => m_isComingBack = value; }

    private void Start()
    {
        m_agent = GetComponent<NavMeshAgent>();
        //m_capsuleCollider = GetComponentInChildren<CapsuleCollider>();
        m_isShoot = false;
        m_isFollow = false;
        IsComingBack = false;
        m_rb = GetComponent<Rigidbody>();
    }
    //public void Shoot(Vector3 raycastHit)
    //{
    //    if(m_isFollow == true)
    //    {
    //        StartCoroutine(C_Shoot(raycastHit));
    //    }
    //}
    private IEnumerator C_Shoot(Vector3 raycastHit)
    {
        m_enemyController.enabled = false;
        //m_capsuleCollider.enabled = false;
        m_self.transform.LookAt(raycastHit);
        m_isShoot = true;
        m_isFollow = false;
        IsComingBack = false;
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
            m_wantedPos = Vector3.Lerp(startPosition, raycastHit, timer / duration);
            transform.position = m_wantedPos + (Vector3.right * xOffset) + (Vector3.up * yOffset);
            yield return new WaitForEndOfFrame();
            //m_capsuleCollider.enabled = true;
            IsComingBack = true;
        }
        transform.position = raycastHit;
        m_isShoot = false;
        m_VFX.Play();
        m_VFXDirt.Play();

        StartCoroutine(C_Delay());       
    }

    public IEnumerator C_Delay()
    {
        m_rb.constraints = /*RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ*/ RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        m_self.GetComponentInChildren<Animator>().Play("A_Delay");
        yield return new WaitForSeconds(m_animationDelay);
        m_enemyController.enabled = true;
        m_rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }
    public void StartShootingCoroutine(Vector3 raycastHit)
    {
        if (m_isFollow == true)
            m_shootCoroutine = StartCoroutine(C_Shoot(raycastHit));
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall") && IsShoot == true)
        {
            m_wantedPos = collision.contacts[0].point;
            StopCoroutine(m_shootCoroutine);
            m_shootCoroutine = null;
            transform.position = m_wantedPos;
            StartCoroutine(C_Delay());
            //IsShoot = false;
            //IsComingBack = true;
            //IsFollow = false;
        }
    }

    private void Update()
    {
        if(Vector3.Distance(m_enemyController.m_target.position , transform.position) > m_rangeLaunch)
        {
            IsComingBack = true;
            m_isFollow = false;
            m_rb.linearDamping = 0f;
        }
        else 
        {
            IsComingBack = false;
            m_isFollow = true;
            m_rb.linearDamping = m_rbDrag;
        }

        if (IsComingBack == true)
        {
            m_enemyController.MaxSpeed = m_comingBackSpeed;
            m_enemyController.Acceleration = m_accelerationSpeed;
            //Jouer particules system de courir
        }
        
        if (IsFollow == true)
        {
            m_enemyController.MaxSpeed = 5f;
            m_enemyController.Acceleration = 20f;
        }
    }
}
