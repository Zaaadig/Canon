using System.Collections;
using UnityEngine;

public class PlayerCanonController : MonoBehaviour
{
    [SerializeField] private PikminController m_pikminController;
    
    private RaycastHit m_raycastHit;


    void Update()
    {
        if (Input.GetMouseButtonDown(0) && m_pikminController.IsFollow == true)
        {
            Vector3 mouse = Input.mousePosition;
            Ray castPoint = Camera.main.ScreenPointToRay(mouse);
            RaycastHit raycastHit;
            if (Physics.Raycast(castPoint, out raycastHit, Mathf.Infinity))
            {
                m_pikminController.Shoot(raycastHit.point);
            }

        }

    }
    
}
