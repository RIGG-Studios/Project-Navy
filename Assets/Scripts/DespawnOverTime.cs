using Photon.Pun;
using UnityEngine;

public class DespawnOverTime : MonoBehaviour
{
    [SerializeField] private float destroyTimer = 4.5f;
    [SerializeField] private bool destroyNetwork = true;
    private float _timer;

    private void Update()
    {
        _timer += Time.deltaTime;

        if (!(_timer >= destroyTimer)) return;
        
        
        if (destroyNetwork)
        {
            PhotonNetwork.Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
