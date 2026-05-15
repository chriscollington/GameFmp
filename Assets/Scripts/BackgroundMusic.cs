using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BackgroundMusic : MonoBehaviour
{
    [Header("Music Tracks")]
    public AudioClip[] musicTracks;

    [Header("Settings")]
    public bool playRandom = true;
    public bool loopPlaylist = true;

    private AudioSource audioSource;
    private int currentTrack = 0;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        PlayMusic();
    }

    void Update()
    {
        if (!audioSource.isPlaying)
        {
            NextTrack();
        }
    }

    void PlayMusic()
    {
        if (musicTracks.Length == 0)
            return;

        if (playRandom)
        {
            currentTrack = Random.Range(0, musicTracks.Length);
        }

        audioSource.clip = musicTracks[currentTrack];
        audioSource.Play();
    }

    void NextTrack()
    {
        if (playRandom)
        {
            currentTrack = Random.Range(0, musicTracks.Length);
        }
        else
        {
            currentTrack++;

            if (currentTrack >= musicTracks.Length)
            {
                if (loopPlaylist)
                    currentTrack = 0;
                else
                    return;
            }
        }

        audioSource.clip = musicTracks[currentTrack];
        audioSource.Play();
    }
}