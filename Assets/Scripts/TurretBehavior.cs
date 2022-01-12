using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class TurretBehavior : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    [SerializeField] private float _laserLength = 2.0f;
    private Vector3[] _laserArray = new Vector3[2];
    [SerializeField] private Transform _laserStartPoint;
    private TurretFOVBehavior _fovBehavior;

    [Header("Sound Effects")] 
    public AudioSource audioSource;
    [SerializeField] private AudioClip _turretDetect;
    [SerializeField] private AudioClip _turretDeath;
    public AudioClip _turretShoot;

    [Header("Combat Properties")]
    public GameObject bulletSpawner;
    [SerializeField] private GameObject _muzzleFlash;
    [SerializeField] private float _fireRate = 0.02f; 
    [HideInInspector] public bool isAlive = true;

    
    // Start is called before the first frame update
    void Start()
    {
        _fovBehavior = GetComponent<TurretFOVBehavior>();
        audioSource = GetComponent<AudioSource>();
        _lineRenderer = GetComponentInChildren<LineRenderer>();
        
        audioSource.Stop();
        _lineRenderer.positionCount = 2;
        StartCoroutine(LaserLineUpdateRoutine());

    }

    private IEnumerator LaserLineUpdateRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.01f);
        while (true)
        {
            _laserArray[0] = _laserStartPoint.position;
            _laserArray[1] = _fovBehavior.canSeePlayer ? _laserArray[1] =_laserStartPoint.position +  _fovBehavior.playerDir : _laserStartPoint.position + _laserStartPoint.forward * _laserLength;
            _lineRenderer.SetPositions(_laserArray);
            yield return wait;
        }
    }

    private IEnumerator MuzzleFlashRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(_fireRate);
        WaitForSeconds waitFire = new WaitForSeconds(0.01f);
        while (_fovBehavior.canSeePlayer)
        {
            _muzzleFlash.SetActive(true);
            yield return waitFire;
            _muzzleFlash.SetActive(false);
            yield return wait;
        }
    }
    
    public void PlayAudioDetect()
    {
        if(!isAlive) return;
        audioSource.clip = _turretDetect;
        audioSource.loop = false;
        AudioSource.PlayClipAtPoint(_turretDetect, transform.position);
        
    }
    public void PlayAudioDeath()
    {
        isAlive = false;
        StopCoroutine(GetComponent<TurretFOVBehavior>().FOVRoutine());
        audioSource.Stop();
        audioSource.clip = _turretDeath;
        audioSource.loop = false;
        AudioSource.PlayClipAtPoint(_turretDeath, transform.position);
    }
    public void PlayAudioShoot()
    {
        if(!isAlive) return;
        audioSource.Stop();
        StartCoroutine(MuzzleFlashRoutine());
        audioSource.clip = _turretShoot;
        audioSource.loop = true;
        audioSource.Play();
    }

    public void StopAudioShoot()
    {
        audioSource.Stop();
        StopCoroutine(MuzzleFlashRoutine());
    }
}
