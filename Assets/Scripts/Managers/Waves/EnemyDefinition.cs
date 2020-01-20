using Assets.Scripts.Enemy;
using UnityEngine;

namespace Assets.Scripts.Managers.Waves
{
    public class EnemyDefinition : MonoBehaviour
    {
        [SerializeField]
        private EnemyType _enemyType = EnemyType.Basic;

        [SerializeField]
        private int _count;

        public EnemyType EnemyType => _enemyType;

        public int Count => _count;
    }
}
