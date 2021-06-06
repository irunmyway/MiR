using UnityEngine;

namespace Foundation
{
    public interface ICharacterAgent
    {
        void Move(Vector2 dir);
        void NavigateTo(Vector2 dir);
        void Look(Vector2 dir);
        void Stop();
    }
}
