using UnityEngine;

public class ColliderController : MonoBehaviour
{
    private TagHandle _playerTag;
    public void OnEnable()
    {
        _playerTag = TagHandle.GetExistingTag("Player");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(_playerTag))
        {
            Debug.Log("boo");
        }
    }
}
