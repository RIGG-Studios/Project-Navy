using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviourPun, IDamagable, IPunObservable
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private Slider healthSlider;

    [SerializeField] private bool useHitMarker = true;
    [SerializeField] private GameObject hitMarker;

    [SerializeField] private bool useShipDestroyedNotifier;
    [SerializeField] private Animator shipDestroyedAnim;

    [SerializeField] private bool useInteractHelper;
    [SerializeField] private GameObject interactHelper;

    [SerializeField] private bool useShipBoardingNotification;
    [SerializeField] private float shipBoardingTimer = 5f;
    [SerializeField] private Text shipBoardingText;

    [SerializeField] private bool useShipControlUI;
    [SerializeField] private GameObject shipControlUI;

    [SerializeField] private bool useCannonUI;
    [SerializeField] private GameObject cannonUI;

    [SerializeField] private GameObject outOfMapUI;
    
    public string PlayerName { get; private set; }
    public int PlayerActorNumber { get; private set; }
    public Ship PlayerShip { get; private set; }
    
    private float _currentHealth;
    private bool _canRecieveDamage;
    private Transform _spawnPoint;
    private bool _hasNoShip;
    private Ship _shipToBoard;
    private PlayerController _playerController;
    private MusketController _musketController;

    private bool _boardingShip;
    private float _shipBoardingCooldown;


    public int ActorID => PlayerActorNumber;

    private void Awake()
    {
        _currentHealth = maxHealth;
        healthSlider.minValue = 0;
        healthSlider.maxValue = 100;
        healthSlider.value = maxHealth;
        _shipBoardingCooldown = shipBoardingTimer;

        _playerController = GetComponent<PlayerController>();
        _musketController = GetComponent<MusketController>();
    }

    private void Update()
    {
        if (_boardingShip)
        {
            _shipBoardingCooldown -= Time.deltaTime;
            
            if (useShipBoardingNotification)
            {
                shipBoardingText.gameObject.SetActive(true);
                shipBoardingText.text = "BOARDING SHIP IN: " + _shipBoardingCooldown;
            }
            
            if (_shipBoardingCooldown <= 0)
            {
                BoardShip();
            }
        }
    }

    public void SetupNetworkPlayer(Ship ship)
    {
        if (!photonView.IsMine)
        {
            return;
        }

        photonView.RPC("SetInitialPlayerInfo", RpcTarget.AllBuffered, 
            PhotonNetwork.LocalPlayer.NickName, PhotonNetwork.LocalPlayer.ActorNumber);

        _spawnPoint = ship.playerSpawnPoint;
        PlayerShip = ship;
        
        //initialize package info
        object[] package =
        {
            PlayerName,
            PlayerActorNumber,
            photonView.ViewID
        };
        
        //send event with the package we created
        PhotonEventsManager.Instance.RaiseEvent(EventCodes.AddPlayer, ReceiverGroup.MasterClient, package);
    }
    
    [PunRPC]
    public void SetInitialPlayerInfo(string playerName, int actorID)
    {
        PlayerName = playerName;
        PlayerActorNumber = actorID;
        gameObject.name = PlayerName + (photonView.IsMine ? " (Local)" : " (Remote)");
    }

    [PunRPC]
    public void OnPlayerShipSink()
    {
        _hasNoShip = true;

        if (useShipDestroyedNotifier)
        {
            shipDestroyedAnim.SetTrigger("Show");
        }
    }

    private void BoardShip()
    {
        _shipBoardingCooldown = shipBoardingTimer;

        if (useShipBoardingNotification)
        {
            shipBoardingText.gameObject.SetActive(false);
            shipBoardingText.text = string.Empty;
        }

        if (photonView.IsMine)
        {
            _musketController.enabled = false;
            _playerController.enabled = false;
        }

        transform.position = _shipToBoard.boardingSpawnPoint.position;
        transform.rotation = _shipToBoard.boardingSpawnPoint.rotation;

        if (photonView.IsMine)
        {
            _musketController.enabled = true;
            _playerController.enabled = true;
        }

        _shipToBoard = null;
        _boardingShip = false;
    }

    public void ToggleShipControlUI(bool state)
    {
        if (useShipControlUI)
        {
            shipControlUI.SetActive(state);
        }
    }

    public void ToggleCannonUI(bool state)
    {
        if (useCannonUI)
        {
            cannonUI.SetActive(state);
        }
    }
    
    private void Die()
    {
        if (photonView.IsMine)
        {
            Respawn();
        }
    }

    private void Respawn()
    {
        if (_hasNoShip)
        {
            EndGameUI.Instance.ShowGameOverScreen();
            PlayerSpawner.Instance.ToggleSceneCamera(true);
            PhotonNetwork.Destroy(gameObject);

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            if (photonView.IsMine)
            {
                _musketController.enabled = false;
                _playerController.enabled = false;
            }
            
            transform.position = _spawnPoint.position + new Vector3(0, 1, 0);
            transform.rotation = _spawnPoint.rotation;

            if (photonView.IsMine)
            {
                _musketController.enabled = true;
                _playerController.enabled = true;
                
                _playerController.canDoAnything = true;
                _playerController.canRecieveInput = true;
                _musketController.canDoAnything = true;
            }
            
            _currentHealth = maxHealth;
            healthSlider.value = _currentHealth;
            _canRecieveDamage = true;
        }
    }

    [PunRPC]
    public void OnPlayerDamagedOther(int otherActorID)
    {
        if (!useHitMarker) return;
        
        hitMarker.SetActive(true);
        Invoke(nameof(ResetHitMarker), 0.25f);
    }
    
    [PunRPC]
    public void OnPlayerDamagedShip()
    {
        if (!useHitMarker || !photonView.IsMine) return;
        
        hitMarker.SetActive(true);
        Invoke(nameof(ResetHitMarker), 0.25f);
    }
    
    private void ResetHitMarker()
    {
        hitMarker.SetActive(false);
    }

    public void ToggleInteractHelper(bool state)
    {
        if (!useInteractHelper)
            return;
        
        interactHelper.SetActive(state);
    }
    
    public void Damage(int attackerID, float damageAmount)
    {
        _currentHealth -= damageAmount;
        healthSlider.value = _currentHealth;
        
        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine)
            return;
        
        if (other.TryGetComponent(out ShipBoardingCollider shipBoardingCollider))
        {
            _boardingShip = true;
            _shipToBoard = shipBoardingCollider.Ship;
            _shipBoardingCooldown = shipBoardingTimer;
        }
        
        
        if (other.CompareTag("OutOfMapTrigger"))
        {
            outOfMapUI.SetActive(true);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (!photonView.IsMine)
            return;

        if (other.TryGetComponent(out ShipBoardingCollider shipBoardingCollider))
        {
            _boardingShip = false;
            _shipToBoard = null;
            shipBoardingText.gameObject.SetActive(false);
        }

        if (other.CompareTag("OutOfMapTrigger"))
        {
            outOfMapUI.SetActive(false);
        }
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_canRecieveDamage);
            stream.SendNext(_currentHealth);
            stream.SendNext(PlayerName);
            stream.SendNext(PlayerActorNumber);
        }
        else
        {
            _canRecieveDamage = (bool)stream.ReceiveNext();
            _currentHealth = (float)stream.ReceiveNext();
            PlayerName = (string)stream.ReceiveNext();
            PlayerActorNumber = (int)stream.ReceiveNext();
        }
    }
}
