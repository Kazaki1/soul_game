using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;

    [Header("Audio Sources")]
    public AudioSource movementSource; // walk/run
    public AudioSource combatSource;   // attack + hit enemy

    [Header("Audio Clips")]
    public AudioClip[] walkClips;
    public AudioClip[] runClips;
    public AudioClip[] attackClips;    // swing/attack
    public AudioClip[] hitEnemyClips;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // --- Movement SFX ---
    public void PlayWalk()
    {
        PlayRandom(movementSource, walkClips, 0.9f, 1.1f, 0.8f);
    }

    public void PlayRun()
    {
        PlayRandom(movementSource, runClips, 0.95f, 1.1f, 1.0f);
    }

    // --- Combat SFX ---
    public void PlayAttack()
    {
        PlayRandom(combatSource, attackClips, 0.9f, 1.0f, 1f);
    }

    public void PlayHitEnemy()
    {
        PlayRandom(combatSource, hitEnemyClips, 0.9f, 1.0f, 1f);
    }

    // --- Stop movement ---
    public void StopMovementSFX()
    {
        if (movementSource != null && movementSource.isPlaying)
        {
            movementSource.Stop();
        }
    }

    // --- Helper method ---
    private void PlayRandom(AudioSource src, AudioClip[] clips, float minPitch, float maxPitch, float volume = 1f)
    {
        if (src == null || clips.Length == 0) return;

        src.pitch = Random.Range(minPitch, maxPitch);
        int index = Random.Range(0, clips.Length);
        src.PlayOneShot(clips[index], volume);
    }
}
