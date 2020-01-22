using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource _explosionAudio;

    [SerializeField]
    private AudioSource _laserBeam;

    public void PlayExplosion(Vector3 position)
    {
        if (_explosionAudio != null)
        {
            AudioSource.PlayClipAtPoint(_explosionAudio.clip, position);
        }
    }

    public void PlayLaserBeam(Vector3 position)
    {
        if (_explosionAudio != null)
        {
            AudioSource.PlayClipAtPoint(_laserBeam.clip, position);
        }
    }
}
