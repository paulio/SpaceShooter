using UnityEngine;

namespace Assets.Scripts.Managers.Waves
{
    public class SubWave : MonoBehaviour
    {
        public const int DelayUntilAllEnemiesAreDead = -1;

        [SerializeField]
        private EnemyDefinition[] _enemies;

        [SerializeField]
        private PowerUp[] _powerUps;

        [SerializeField]
        private float _delayUntilNextWave = 5f;

        public EnemyDefinition[] Enemies => _enemies;

        public PowerUp[] PowerUps => _powerUps;

        public float DelayUntilNextWave 
        {
            get
            {
                return _delayUntilNextWave;
            }
            set
            {
                _delayUntilNextWave = value;
            }
        }

    }
}
