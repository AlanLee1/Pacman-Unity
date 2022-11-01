using UnityEngine;

public class PlayWakaWaka : MonoBehaviour
{
    public AudioClip WakaClip1;
    public AudioClip WakaClip2;

    private AudioSource _audioSource;

    private static bool _switchClip;

    private void OnDestroy()
    {
        _audioSource = FindObjectOfType<AudioSource>();
        if (_audioSource != null)
        {
            //metodo que toca o som
            _audioSource.PlayOneShot(_switchClip ? WakaClip1 : WakaClip2);
            //invertendo o valor do clip
            _switchClip = !_switchClip;
        }
    }
}
