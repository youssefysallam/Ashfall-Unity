using UnityEngine;

public class AmbientMusic : MonoBehaviour
{
    public static AmbientMusic Instance { get; private set; }

    [SerializeField] private AudioSource source;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (source == null) source = GetComponent<AudioSource>();
        if (source != null && !source.isPlaying)
            source.Play();
    }
}