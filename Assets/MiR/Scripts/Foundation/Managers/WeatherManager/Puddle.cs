using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Foundation
{
    [RequireComponent(typeof(MeshRenderer))]
    public sealed class Puddle : MonoBehaviour
    {
        public float AppearTime = 15.0f;
        public float DisappearTime = 5.0f;

        Material material;
        float originalAlpha;

        void Awake()
        {
            material = GetComponent<MeshRenderer>().material;
            var c = material.color;
            originalAlpha = c.a;
            c.a = 0.0f;
            material.color = c;
        }

        public void Appear()
        {
            material.DOKill(false);
            material.DOFade(originalAlpha, AppearTime);
        }

        public void Disappear()
        {
            material.DOKill(false);
            material.DOFade(0.0f, DisappearTime);
        }
    }
}
