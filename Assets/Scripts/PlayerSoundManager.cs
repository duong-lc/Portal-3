using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;

public class PlayerSoundManager : MonoBehaviour
{
    public static PlayerSoundManager Instance;
    private AudioSource _audioSource;
    [Header("Sound Library")]
    [Header("Item Sounds")]
    [SerializeField] private AudioClip _soundCubeDestroy;//buttons/spark5
    [SerializeField] private AudioClip _soundCubeRespawn;//buttons/bell1
    [SerializeField] private AudioClip _soundSpawnButtonInteract;//buttons/button3
    [SerializeField] private AudioClip _soundCubeInteract;//buttons/lightswitch2
    [SerializeField] private AudioClip _soundBigButtonActivation;//plats/elevbell1
   
    
    [Header("Player Sounds")]
    [SerializeField] private AudioClip _soundPlayerDeath; //common/bodysplat
    [SerializeField] private AudioClip _soundPlayerDamage; //player/pl_pain6
    [SerializeField] private AudioClip _soundFireLMB; //weapons/pl_gun1
    [SerializeField] private AudioClip _soundFireRMB; //weapons/pl_gun2
    //[SerializeField] private AudioClip _soundTeleport; //spark2
    
    
    [Header("UI Sounds")]
    [SerializeField] private AudioClip _soundHUDButtonSelect; //common/wpn_select
    [SerializeField] private AudioClip _soundPauseScreenEnable; //common/wpn_hudon
    [SerializeField] private AudioClip _soundPauseScreenDisable; //common/wpn_hudoff
    [SerializeField] private AudioClip _soundWinScreen; //scientist/asexpected

    private void Start()
    {
        Instance = this;
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayCubeDestroyAudio(Vector3 pos)
    {
        AudioSource.PlayClipAtPoint(_soundCubeDestroy, pos);
    }

    public void PlayCubeRespawnAudio(Vector3 pos)
    {
        AudioSource.PlayClipAtPoint(_soundCubeRespawn, pos);
    }

    public void PlaySpawnButtonAudio(Vector3 pos)
    {
        AudioSource.PlayClipAtPoint(_soundSpawnButtonInteract, pos);
    }
    
    public void PlayCubeInteractAudio()
    {
        AudioSource.PlayClipAtPoint(_soundCubeInteract, transform.position);
    }
    
    public void PlayBigButtonAudio(Vector3 pos)
    {
        AudioSource.PlayClipAtPoint(_soundBigButtonActivation, pos);
    }

    public void PlayPlayerDeathAudio()
    {
        _audioSource.PlayOneShot(_soundPlayerDeath, 1f);
    }

    public void PlayPlayerDamageAudio()
    {
        _audioSource.PlayOneShot(_soundPlayerDamage, 0.3f);
    }

    public void PlayPlayerFireLmbAudio()
    {
        _audioSource.PlayOneShot(_soundFireLMB, 1f);
    }
    
    public void PlayPlayerFireRmbAudio()
    {
        _audioSource.PlayOneShot(_soundFireRMB, 1f);
    }
    //
    // public void PlayPlayerTeleportAudio(Vector3 pos)
    // {
    //     AudioSource.PlayClipAtPoint(_soundTeleport, pos);
    // }
    
    public void PlayHudButtonSelectAudio()
    {
        _audioSource.PlayOneShot(_soundHUDButtonSelect, 1f);
    }
    public void PlayPauseScreenEnableAudio()
    {
        _audioSource.PlayOneShot(_soundPauseScreenEnable, 1f);
    }
    public void PlayPauseScreenDisableAudio()
    {
        _audioSource.PlayOneShot(_soundPauseScreenDisable, 1f);
    }
    public void PlayPlayerWinAudio()
    {
        _audioSource.PlayOneShot(_soundWinScreen, 1f);
    }
    
    //public void 
}
