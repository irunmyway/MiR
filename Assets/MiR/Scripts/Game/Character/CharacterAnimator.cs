using Foundation;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Game
{
    [RequireComponent(typeof(Animator))]
    public sealed class CharacterAnimator : AbstractBehaviour, IOnUpdate
    {
        enum BodyAnim
        {
            IdleOrRunning_NoWeapon = 1000,
            IdleOrRunning_Pistol = 1001,
            IdleOrRunning_Baseball = 1004,
            IdleOrRunning_Rifle = 1007,
            Crouching_NoWeapon = 1002,
            Crouching_Pistol = 1003,
            Crouching_Baseball = 1005,
            Crouching_Rifle = 1008,
            Attack_Baseball = 1006,
            Attack_Rifle = 1009,
            Dying_NoWeapon1 = 1010,
            Dying_NoWeapon2 = 1011,
            Dying_Rifle = 1012,
            Dying_Crouching_Rifle = 1013,
            Car_Entering = 1014,
            Car_Exiting = 1015,
            Car_Sitting = 1016,
            Car_Driving = 1017,
        }

        enum LegsAnim
        {
            IdleOrRunning = 2000,
            Crouching = 2001,
            Dying_NoWeapon1 = 2002,
            Dying_NoWeapon2 = 2003,
            Dying_Rifle = 2004,
            Dying_Crouching_Rifle = 2005,
            Car_Entering = 2006,
            Car_Exiting = 2007,
            Car_Sitting = 2008,
            Car_Driving = 2009,
        }

        [Header("Debug")]
        [HideInInspector] [ReadOnly] BodyAnim bodyAnim = BodyAnim.IdleOrRunning_NoWeapon;
        [HideInInspector] [ReadOnly] LegsAnim legsAnim = LegsAnim.IdleOrRunning;

        [Header("Weapons")]
        public List<AbstractWeapon> BaseballBats;
        public List<AbstractWeapon> Pistols;
        public List<AbstractWeapon> Rifles;
        public List<AbstractWeapon> RocketLaunchers;

        Animator animator;
        Vector3 prevPosition;
        int bodyAnimID;
        int legsAnimID;
        int velocityXID;
        int velocityYID;
        bool dead;
        BodyAnim deadBodyAnim;
        LegsAnim deadLegsAnim;

        [Inject] ISceneState state = default;
        [InjectOptional] ICharacterWeapon characterWeapon = default;
        [InjectOptional] ICharacterCrouchInput characterCrouch = default;
        [InjectOptional] ICharacterHealth characterHealth = default;
        [InjectOptional] ICharacterVehicle characterVehicle = default;

        void Awake()
        {
            animator = GetComponent<Animator>();
            bodyAnimID = Animator.StringToHash("BodyAnim");
            legsAnimID = Animator.StringToHash("LegsAnim");
            velocityXID = Animator.StringToHash("VelocityX");
            velocityYID = Animator.StringToHash("VelocityY");
            animator.SetInteger(bodyAnimID, (int)bodyAnim);
            animator.SetInteger(legsAnimID, (int)legsAnim);
            prevPosition = transform.position;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Observe(state.OnUpdate);
        }

        void IOnUpdate.Do(float deltaTime)
        {
            var (wantedBodyAnim, wantedLegsAnim) = ChooseAnimation();

            if (bodyAnim != wantedBodyAnim) {
                bodyAnim = wantedBodyAnim;
                animator.SetInteger(bodyAnimID, (int)bodyAnim);
            }

            if (legsAnim != wantedLegsAnim) {
                legsAnim = wantedLegsAnim;
                animator.SetInteger(legsAnimID, (int)legsAnim);
            }

            var position = transform.position;
            var delta = position - prevPosition;
            prevPosition = position;

            delta = transform.InverseTransformDirection(delta);

            var velocity = new Vector2(delta.x, delta.z).normalized;
            animator.SetFloat(velocityXID, velocity.x);
            animator.SetFloat(velocityYID, velocity.y);
        }

        (BodyAnim body, LegsAnim legs) ChooseAnimation()
        {
            AbstractWeapon weapon = null;
            bool attacking = false;
            if (characterWeapon != null) {
                weapon = characterWeapon.CurrentWeapon;
                attacking = characterWeapon.AttackingWeapon != null;
            }

            if (characterVehicle != null) {
                switch (characterVehicle.State) {
                    case CharacterVehicleState.NotInVehicle:
                        break;

                    case CharacterVehicleState.Entering:
                        return (BodyAnim.Car_Entering, LegsAnim.Car_Entering);

                    case CharacterVehicleState.Exiting:
                        return (BodyAnim.Car_Exiting, LegsAnim.Car_Exiting);

                    case CharacterVehicleState.Driving:
                        return (BodyAnim.Car_Driving, LegsAnim.Car_Driving);

                    case CharacterVehicleState.Sitting:
                        return (BodyAnim.Car_Sitting, LegsAnim.Car_Sitting);
                }
            }

            if (characterHealth != null && characterHealth.IsDead) {
                if (!dead) {
                    dead = true;
                    if (characterCrouch != null && characterCrouch.Crouching) {
                        deadBodyAnim = BodyAnim.Dying_Crouching_Rifle;
                        deadLegsAnim = LegsAnim.Dying_Crouching_Rifle;
                    } else {
                        if (Rifles != null && weapon != null && Rifles.Contains(weapon)) {
                            deadBodyAnim = BodyAnim.Dying_Rifle;
                            deadLegsAnim = LegsAnim.Dying_Rifle;
                        } else {
                            switch (Random.Range(0, 2)) {
                                case 0:
                                    deadBodyAnim = BodyAnim.Dying_NoWeapon1;
                                    deadLegsAnim = LegsAnim.Dying_NoWeapon1;
                                    break;
                                case 1:
                                    deadBodyAnim = BodyAnim.Dying_NoWeapon2;
                                    deadLegsAnim = LegsAnim.Dying_NoWeapon2;
                                    break;
                            }
                        }
                    }
                }

                return (deadBodyAnim, deadLegsAnim);
            }

            if (characterCrouch != null && characterCrouch.Crouching) {
                if (BaseballBats != null && weapon != null && BaseballBats.Contains(weapon)) {
                    if (attacking)
                        return (BodyAnim.Attack_Baseball, LegsAnim.Crouching);
                    else
                        return (BodyAnim.Crouching_Baseball, LegsAnim.Crouching);
                }

                if (Pistols != null && weapon != null && Pistols.Contains(weapon)) {
                    if (attacking)
                        characterWeapon.EndAttack(applyCooldown: true);
                    return (BodyAnim.Crouching_Pistol, LegsAnim.Crouching);
                }

                if (Rifles != null && weapon != null && Rifles.Contains(weapon)) {
                    if (attacking)
                        return (BodyAnim.Attack_Rifle, LegsAnim.Crouching);
                    else
                        return (BodyAnim.Crouching_Rifle, LegsAnim.Crouching);
                }

                if (RocketLaunchers != null && weapon != null && RocketLaunchers.Contains(weapon)) {
                    if (attacking)
                        characterWeapon.EndAttack(applyCooldown: true);
                    return (BodyAnim.Crouching_NoWeapon, LegsAnim.Crouching);
                }

                return (BodyAnim.Crouching_NoWeapon, LegsAnim.Crouching);
            }

            if (BaseballBats != null && weapon != null && BaseballBats.Contains(weapon)) {
                if (attacking)
                    return (BodyAnim.Attack_Baseball, LegsAnim.IdleOrRunning);
                else
                    return (BodyAnim.IdleOrRunning_Baseball, LegsAnim.IdleOrRunning);
            }

            if (Pistols != null && weapon != null && Pistols.Contains(weapon)) {
                if (attacking)
                    characterWeapon.EndAttack(applyCooldown: true);
                return (BodyAnim.IdleOrRunning_Pistol, LegsAnim.IdleOrRunning);
            }

            if (Rifles != null && weapon != null && Rifles.Contains(weapon)) {
                if (attacking)
                    return (BodyAnim.Attack_Rifle, LegsAnim.IdleOrRunning);
                else
                    return (BodyAnim.IdleOrRunning_Rifle, LegsAnim.IdleOrRunning);
            }

            if (RocketLaunchers != null && weapon != null && RocketLaunchers.Contains(weapon)) {
                if (attacking)
                    characterWeapon.EndAttack(applyCooldown: true);
                return (BodyAnim.IdleOrRunning_NoWeapon, LegsAnim.IdleOrRunning);
            }

            return (BodyAnim.IdleOrRunning_NoWeapon, LegsAnim.IdleOrRunning);
        }
    }
}
