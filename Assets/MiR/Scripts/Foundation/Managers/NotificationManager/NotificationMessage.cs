using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Zenject;

namespace Foundation
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public sealed class NotificationMessage : AbstractBehaviour, IPoolable<string, IMemoryPool>
    {
        public sealed class Factory : PlaceholderFactory<string, NotificationMessage>
        {
        }

        TextMeshProUGUI text;
        Vector3 originalPosition;

        public float MoveHeight;
        public float MoveTime;

        void Awake()
        {
            text = GetComponent<TextMeshProUGUI>();
            originalPosition = transform.localPosition;
        }

        public void OnSpawned(string message, IMemoryPool pool)
        {
            text.text = message;
            transform.localPosition = originalPosition;
            gameObject.SetActive(true);

            text
                .DOFade(0.0f, MoveTime)
                .From(1.0f);

            transform
                .DOMove(transform.position + Vector3.up * MoveHeight, MoveTime)
                .OnComplete(() => pool.Despawn(this));
        }

        public void OnDespawned()
        {
            gameObject.SetActive(false);
        }
    }
}
