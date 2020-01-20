using UnityEngine;

namespace Assets.Scripts.Managers.Waves
{
    public class SubWave : MonoBehaviour
    {
        [SerializeField]
        private EnemyDefinition[] _enemies;

        [SerializeField]
        private PowerUp[] _powerUps;

        [SerializeField]
        private float _delayUntilNextWave = 5f;

        public EnemyDefinition[] Enemies => _enemies;

        public float DelayUntilNextWave => _delayUntilNextWave;
    }
}
