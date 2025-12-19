using UnityEngine;

public class ZombieAudio : MonoBehaviour
{
    public AudioSource sfxSource;

    [Header("Ambient Groans")]
    public AudioClip[] ambientGroans;

    [Header("Timing (seconds)")]
    public float minDelay = 3f;
    public float maxDelay = 8f;

    [Header("Hearing")]
    public float hearDistance = 25f;

    [Header("Volume")]
    [Range(0f, 1f)] public float volume = 0.8f;

    private Transform player;
    private Coroutine routine;

    void Awake()
    {
        if (sfxSource == null) sfxSource = GetComponent<AudioSource>();
        AcquirePlayer();
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

    private void AcquirePlayer()
    {
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
        {
            player = p.transform;
            return;
        }

        var stats = Object.FindFirstObjectByType<PlayerStats>();
        if (stats != null)
        {
            player = stats.transform;
            return;
        }

        var byName = GameObject.Find("PlayerRoot");
        player = byName != null ? byName.transform : null;
    }


    System.Collections.IEnumerator AmbientLoop()
    {
        while (true)
        {
            float wait = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(wait);

            if (player == null) AcquirePlayer();
            if (player == null) continue;

            if (ambientGroans == null || ambientGroans.Length == 0) continue;
            if (sfxSource == null) continue;

            Debug.Log($"[ZombieAudio] player={(player!=null)} d={(player?Vector3.Distance(transform.position, player.position): -1f)}");
            float d = Vector3.Distance(transform.position, player.position);
            if (d > hearDistance) continue;

            var clip = ambientGroans[Random.Range(0, ambientGroans.Length)];
            sfxSource.PlayOneShot(clip, volume);
        }
    }
}
