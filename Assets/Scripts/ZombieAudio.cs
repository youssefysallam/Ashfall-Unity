using UnityEngine;

public class ZombieAudio : MonoBehaviour
{
    public AudioSource sfxSource;

    [Header("Ambient Groans")]
    public AudioClip[] ambientGroans;

    [Header("Timing (seconds)")]
    public float minDelay = 3f;
    public float maxDelay = 8f;

    [Header("Volume")]
    [Range(0f, 1f)] public float volume = 0.8f;

    Coroutine routine;

    void Awake()
    {
        if (sfxSource == null) sfxSource = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        if (routine == null)
            routine = StartCoroutine(AmbientLoop());
    }

    void OnDisable()
    {
        if (routine != null)
        {
            StopCoroutine(routine);
            routine = null;
        }
    }

    System.Collections.IEnumerator AmbientLoop()
    {
        while (true)
        {
            float wait = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(wait);

            if (ambientGroans == null || ambientGroans.Length == 0 || sfxSource == null)
                continue;

            AudioClip clip = ambientGroans[Random.Range(0, ambientGroans.Length)];
            sfxSource.PlayOneShot(clip, volume);
        }
    }
}

