using Assets.Scripts.Managers.Waves;
using UnityEngine;

public class Wave : MonoBehaviour
{
    [SerializeField]
    private SubWave[] _subWaves;

    public SubWave[] Waves => _subWaves;
}

