using System.Collections.Generic;

namespace Foundation
{
    public interface IEnemyManager
    {
        ICollection<IEnemy> AllEnemies { get; }
        ICollection<CoverPoint> AllCoverPoints { get; }

        float DangerousPlayerDistance { get; }

        void AddEnemy(IEnemy enemy);
        void RemoveEnemy(IEnemy enemy);

        void AlertAllEnemies();

        bool EnemyCanAttack(IEnemy enemy);

        CoverPoint AllocCoverPoint(IEnemy enemy);
        void ReleaseCoverPoint(IEnemy enemy, CoverPoint coverPoint);
    }
}
