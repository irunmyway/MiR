using System;
using UnityEngine;
using Zenject;
using DG.Tweening;

namespace Foundation
{
    [RequireComponent(typeof(LineRenderer))]
    public sealed class ShootTrace : MonoBehaviour, IPoolable<Vector3, Vector3, IMemoryPool>
    {
        public sealed class Factory : PlaceholderFactory<Vector3, Vector3, ShootTrace>
        {
        }

        IMemoryPool pool;
        LineRenderer lineRenderer;
        Vector3[] points = new Vector3[2];

        public float ShowTime;

        void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        public void OnSpawned(Vector3 from, Vector3 to, IMemoryPool pool)
        {
            this.pool = pool;
            points[0] = from;
            points[1] = to;
            lineRenderer.SetPositions(points);
            gameObject.SetActive(true);

            var seq = DOTween.Sequence();
            seq.AppendInterval(ShowTime);
            seq.AppendCallback(() => { pool.Despawn(this); });
            seq.Play();
        }

        public void OnDespawned()
        {
            pool = null;
            gameObject.SetActive(false);
        }
    }
}
