using Foundation;
using UnityEngine;
using Zenject;

namespace Game
{
    public class MinimapState : AbstractBehaviour, IOnStateActivate
    {//
        static readonly Vector2 topLeft = new Vector2(-50, -50);
        static readonly Vector2 bottomRight = new Vector2(50, 50);

        public RectTransform fullmapImage;
        public RectTransform playerIndicator;

        public int Player;

        [Inject] IPlayerManager playerManager = default;
        [Inject] ISceneState state = default;

        protected override void OnEnable()
        {
            base.OnEnable();
            Observe(state.OnActivate);
        }

        void IOnStateActivate.Do()
        {
            var player = playerManager.GetPlayer(Player);
            if (player != null) {
                Vector3 pos3d = player.Position;
                Vector2 pos2d = new Vector3(pos3d.x, pos3d.z);

                pos2d -= topLeft;
                pos2d /= (bottomRight - topLeft);

                Vector2 size = fullmapImage.rect.size;
                pos2d *= size;

                playerIndicator.anchoredPosition = pos2d;
            }
        }
    }
}
