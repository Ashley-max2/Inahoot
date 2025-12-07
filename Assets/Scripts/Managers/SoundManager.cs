using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Clips")]
    [SerializeField] private AudioClip correctAnswerSound;
    [SerializeField] private AudioClip incorrectAnswerSound;
    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private AudioClip timeWarningSound;
    [SerializeField] private AudioClip gameOverSound;

    [Header("Configuración")]
    [SerializeField] private float masterVolume = 1f;

    private AudioSource audioSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null); // Asegurar que sea objeto raíz
            DontDestroyOnLoad(gameObject);
            
        }
        else
        {
            Destroy(gameObject);
        }

        audioSource = GetComponent<AudioSource>();
    }

    public void PlayCorrectAnswer()
    {
        PlaySound(correctAnswerSound);
    }

    public void PlayIncorrectAnswer()
    {
        PlaySound(incorrectAnswerSound);
    }

    public void PlayButtonClick()
    {
        PlaySound(buttonClickSound);
    }

    public void PlayTimeWarning()
    {
        PlaySound(timeWarningSound);
    }

    public void PlayGameOver()
    {
        PlaySound(gameOverSound);
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip, masterVolume);
        }
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
    }
}

