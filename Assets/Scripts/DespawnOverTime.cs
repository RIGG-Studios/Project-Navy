using Photon.Pun;
using UnityEngine;

public class DespawnOverTime : MonoBehaviour
{
    [SerializeField] private float destroyTimer = 4.5f;

    private float _timer;

    private void Update()
    {
        _timer += Time.deltaTime;

        if (_timer >= destroyTimer)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
