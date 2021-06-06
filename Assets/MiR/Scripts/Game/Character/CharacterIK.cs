using Foundation;
using UnityEngine;

namespace Game
{
    public class CharacterIK : AbstractBehaviour
    {
        Animator animator;
        public Transform leftToeBase;
        public Transform rightToeBase;
        public float MaxDistanceAboveGround;
        public float MaxDistanceBelowGround;
        public LayerMask layerMask;

        void Awake()
        {
            animator = GetComponentInChildren<Animator>();
        }

        void OnAnimatorIK(int layerIndex)
        {
            if (animator == null)
                return;

            FootIK(AvatarIKGoal.LeftFoot, leftToeBase, "IKLeftFootWeight");
            FootIK(AvatarIKGoal.RightFoot, rightToeBase, "IKRightFootWeight");
        }

        void FootIK(AvatarIKGoal goal, Transform toeBase, string weightName)
        {
            float weight = animator.GetFloat(weightName);
            animator.SetIKPositionWeight(goal, 1.0f);//weight);
            animator.SetIKRotationWeight(goal, 1.0f);// weight);

            Vector3 footPosition = animator.GetIKPosition(goal);
            float yOffset = footPosition.y - toeBase.position.y;

            RaycastHit hit;
            Ray ray = new Ray(footPosition + Vector3.up * MaxDistanceAboveGround, Vector3.down);
            if (Physics.Raycast(ray, out hit, MaxDistanceAboveGround + MaxDistanceBelowGround, layerMask)) {
                Debug.Log($"{hit.point.y}");
                Vector3 newFootPosition = footPosition;
                newFootPosition.y = hit.point.y + yOffset;
                animator.SetIKPosition(goal, newFootPosition);
                animator.SetIKRotation(goal, Quaternion.LookRotation(transform.forward, hit.normal));
            }
        }
    }
}
