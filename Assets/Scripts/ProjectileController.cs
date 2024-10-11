using System.Collections;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [SerializeField] private Vector2 m_movementDuration;
    [SerializeField] private AnimationCurve m_animationCurve;
    [SerializeField] private AnimationCurve m_animationCurveHorizontal;
    [SerializeField] private Vector2 m_verticalOffsetRange;
    [SerializeField] private Vector2 m_horizontalOffsetRange;
    [SerializeField] private ParticleSystem m_explosionVFX;

    public void Shoot(Vector3 targetPosition)
    {
        StartCoroutine(C_Shoot(targetPosition));
    }

    private IEnumerator C_Shoot(Vector3 targetPosition)
    {
        float timer = 0;
        float duration = Random.Range(m_movementDuration.x, m_movementDuration.y);
        Vector3 startPosition = transform.position;
        float hOffset = Random.Range(m_horizontalOffsetRange.x, m_horizontalOffsetRange.y);
        float vOffset = Random.Range(m_verticalOffsetRange.x, m_verticalOffsetRange.y);

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float xOffset = m_animationCurveHorizontal.Evaluate(timer/duration) * hOffset;
            float yOffset = m_animationCurve.Evaluate(timer/duration) * vOffset;
            Vector3 wantedPos = Vector3.Lerp(startPosition, targetPosition, timer / duration);
            transform.position = wantedPos + (Vector3.right * xOffset) + (Vector3.up * yOffset);
            yield return new WaitForEndOfFrame();
        }
        transform.position = targetPosition;
        DestroyProjectile();
    }

    private void DestroyProjectile()
    {
        m_explosionVFX.transform.parent = null;
        m_explosionVFX.Play();
        Destroy(gameObject);
    }
}
