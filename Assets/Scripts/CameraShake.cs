using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{  
   [HideInInspector] public static CameraShake instance;

    private Vector3 _originalPos;
    private float _timeAtCurrentFrame;
    private float _timeAtLastFrame;
    private float _fakeDelta;
    private bool _isShaking = false;

    private void Awake() {
        instance = this;
    }

    private void Update() {
        // Calculate a fake delta time, so we can Shake while game is paused.
        _timeAtCurrentFrame = Time.realtimeSinceStartup;
        _fakeDelta = _timeAtCurrentFrame - _timeAtLastFrame;
        _timeAtLastFrame = _timeAtCurrentFrame; 
    }

    public void Shake (float duration, float amount, bool isTakingDamage)
    {
        if (_isShaking || !PlayerController.Instance.canMove) return;
        instance._originalPos = instance.gameObject.transform.localPosition;
        if(isTakingDamage)
            PlayerSoundManager.Instance.PlayPlayerDamageAudio();
        instance.StopAllCoroutines();
        instance.StartCoroutine(instance.CamShake(duration, amount));
    }

    public IEnumerator CamShake (float duration, float amount) {
        var endTime = Time.time + duration;
        _isShaking = true;
        while (duration > 0) {
            transform.localPosition = _originalPos + Random.insideUnitSphere * amount;

            duration -= _fakeDelta;

            yield return null;
        }

        _isShaking = false;
        transform.localPosition = _originalPos;
    }
}
