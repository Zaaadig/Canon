using System.Collections;
using UnityEngine;

public class PlayerCanonController : MonoBehaviour
{
    [SerializeField] private PikminController m_pikminController;
    [SerializeField] private float m_Yoffset;
    //[SerializeField] private LayerMask m_layerGround;
    //[SerializeField] private LayerMask m_layerWall;

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
                //Vector3 shootPosition = raycastHit.point + Vector3.up * m_Yoffset;
                // Décaler le pikmin vers le haut pour qu'il ne rentre pas dans le sol
                //m_pikminController.Shoot(raycastHit.point);
                m_pikminController.StartShootingCoroutine(raycastHit.point);
                //if (m_layerGround == (m_layerGround | (1 << raycastHit.transform.gameObject.layer)))
                //{
                //    print("ground hit");

                //}

                //else if (m_layerWall == (m_layerWall | (1 << raycastHit.transform.gameObject.layer)))
                //{
                //    print("Wall hit");
                //}
            }
        }
    }   
}
