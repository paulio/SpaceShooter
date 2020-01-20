using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource _explosionAudio;

    public void PlayExplosion(Vector3 position)
    {
        if (_explosionAudio != null)
        {
            AudioSource.PlayClipAtPoint(_explosionAudio.clip, position);
        }
    }
}
