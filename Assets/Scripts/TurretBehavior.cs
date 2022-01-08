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
    private AudioSource _audioSource;
    [SerializeField] private AudioClip _turretDetect;
    [SerializeField] private AudioClip _turretDeath;
    [SerializeField] private AudioClip _turretShoot;
    
    // Start is called before the first frame update
    void Start()
    {
        _fovBehavior = GetComponent<TurretFOVBehavior>();
        _audioSource = GetComponent<AudioSource>();
        _lineRenderer = GetComponentInChildren<LineRenderer>();
        
        _audioSource.Stop();
        _lineRenderer.positionCount = 2;
        StartCoroutine(LaserLineUpdateRoutine());

    }

    private IEnumerator LaserLineUpdateRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.01f);
        while (true)
        {
            _laserArray[0] = _laserStartPoint.position;
            _laserArray[1] = _laserStartPoint.position + _laserStartPoint.forward * _laserLength;
            _lineRenderer.SetPositions(_laserArray);
            yield return wait;
        }
    }
    
    // Update is called once per frame
    private void Update()
    {
        if (_fovBehavior.canSeePlayer)
        {
            print($"fire");
        }
    }

    public void PlayAudioDetect()
    {
        //AudioSource.PlayClipAtPoint(_turretDetect, transform.position);
        _audioSource.Stop();
        _audioSource.clip = _turretDetect;
        _audioSource.loop = false;
        AudioSource.PlayClipAtPoint(_turretDetect, transform.position);
        
    }
    public void PlayAudioDeath()
    {
       // AudioSource.PlayClipAtPoint(_turretDeath, transform.position);
       StopCoroutine(GetComponent<TurretFOVBehavior>().FOVRoutine());
       _audioSource.Stop();
       _audioSource.clip = _turretDeath;
       _audioSource.loop = false;
       AudioSource.PlayClipAtPoint(_turretDeath, transform.position);
    }
    public void PlayAudioShoot()
    {
       // AudioSource.PlayClipAtPoint(_turretShoot, transform.position);
       _audioSource.Stop();
       _audioSource.clip = _turretShoot;
       _audioSource.loop = true;
       _audioSource.Play();
    }
}
