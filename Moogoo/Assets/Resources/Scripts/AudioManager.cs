using UnityEngine;

public static class AudioManager
{
    public static void Play(params string[] fileNames)
    {
        // Find audio source
        AudioSource audioSource = UnityEngine.Object.FindObjectOfType<GameManager>().soundAS;
        if (audioSource == null)
        {
            Debug.LogError("No AudioSource found in the scene.");
            return;
        }

        // Select and load audio clip to play
        int selection = Random.Range(0, fileNames.Length);
        AudioClip sound = Resources.Load<AudioClip>($"Audio/{fileNames[selection]}");
        if (sound == null)
        {
            Debug.LogError($"AudioClip '{fileNames[selection]}' not found in Resources/Audio.");
            return;
        }

        // Play the audio clip
        audioSource.clip = sound;
        audioSource.loop = false;
        audioSource.Play();
    }

    public static void PlayMusic(params string[] fileNames)
    {
        // Find audio source
        AudioSource audioSource = UnityEngine.Object.FindObjectOfType<GameManager>().musicAS;
        if (audioSource == null)
        {
            Debug.LogError("No AudioSource found in the scene.");
            return;
        }

        // Select and load audio clip to play
        int selection = Random.Range(0, fileNames.Length);
        AudioClip sound = Resources.Load<AudioClip>($"Audio/{fileNames[selection]}");
        if (sound == null)
        {
            Debug.LogError($"AudioClip '{fileNames[selection]}' not found in Resources/Audio.");
            return;
        }

        // Play the audio clip
        audioSource.clip = sound;
        audioSource.loop = true;
        audioSource.Play();
    }
}
