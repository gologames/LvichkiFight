using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;

/// <summary>
/// Лучше чтоб проект был в своем неймспейсе.
/// </summary>
//namespace FalangaSim
//{
    /// <summary>
    /// Пример запуска анимации из кода. http://en.esotericsoftware.com/spine-unity#Controlling-Animation
    /// </summary>
    public class ExampleLvichka : MonoBehaviour
    {
        public enum AnimState { IDLE, WALK, HURT, ATTACK, DEATH }
        public AnimState anim_state = AnimState.IDLE;
        List<SkeletonAnimation> skeleton_animations = new List<SkeletonAnimation>();
        int anim_index = 0;

        void Start()
        {
            Debug.Log("Press SPACE to switch to the next anim");
            skeleton_animations.AddRange(FindObjectsOfType<SkeletonAnimation>());
            Debug.Log("Skeletons found in scene: " + skeleton_animations.Count);

            foreach (var skeleton_animation in skeleton_animations)
            {
                SetAnim(skeleton_animation, AnimState.IDLE);
            }
        }

        void Update()
        {
            if (Input.GetKeyDown("space"))
            {
                anim_index++;
                var anim_names = Enum.GetValues(typeof(AnimState));

                if (anim_index >= anim_names.Length)
                {
                    anim_index = 0;
                }
                var anim_name = anim_names.GetValue(anim_index);

                Debug.Log("Set animation " + anim_name);
                foreach (var skeleton_animation in skeleton_animations)
                {
                    SetAnim(skeleton_animation, (AnimState)anim_name);
                }
            }
        }

        void SetAnim(SkeletonAnimation skeleton_animation, AnimState anim_state)
        {
            bool loop = true;
            skeleton_animation.state.SetAnimation(0, anim_state.ToString(), loop);
        }
    }
//}
