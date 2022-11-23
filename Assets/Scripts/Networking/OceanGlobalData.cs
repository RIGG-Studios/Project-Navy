using Photon.Pun;
using UnityEngine;

public class OceanGlobalData : MonoBehaviour, IPunObservable
{
    [SerializeField] private float maaxDifferenceBeforeAdjust = 0.5f;
    
    public static  OceanGlobalData Instance;
    
    public float Time { get; private set; }

    private float _currentTime;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Time = UnityEngine.Time.time;
        }
        else
        {
            Time = Mathf.Lerp(Time, _currentTime, UnityEngine.Time.time * 3.5f);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //write 
        if (stream.IsWriting)
        {
            stream.SendNext(Time);
        }
        else //read
        {
            _currentTime = (float)stream.ReceiveNext();
        }
    }
}
