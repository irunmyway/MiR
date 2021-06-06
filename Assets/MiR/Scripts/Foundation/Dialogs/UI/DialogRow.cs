using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;

namespace Foundation
{
    public sealed class DialogRow : AbstractBehaviour, IPoolable<IMemoryPool>
    {
        public sealed class Factory : PlaceholderFactory<DialogRow>
        {
        }

        struct ButtonInfo
        {
            public DialogButton Button;
            public DialogNode Node;
        }

        public TextMeshProUGUI Text;
        public Transform Contents;
        public Transform BalloonImage;

        List<ButtonInfo> buttons = new List<ButtonInfo>();

        Transform originalParent;
        public IMemoryPool Pool { get; private set; }

        [Inject] DialogButton.Factory buttonFactory = default;

        void Awake()
        {
            originalParent = transform.parent;
        }

        public void OnSpawned(IMemoryPool pool)
        {
            Pool = pool;
            gameObject.SetActive(true);
        }

        public void OnDespawned()
        {
            RemoveButtons();
            gameObject.SetActive(false);
            transform.SetParent(originalParent, false);
        }

        public void AddButton(string id, string text, DialogNode node)
        {
            var button = buttonFactory.Create(text);
            button.gameObject.name = id;
            button.transform.SetParent(Contents.transform);

            ButtonInfo info = new ButtonInfo();
            info.Button = button;
            info.Node = node;
            buttons.Add(info);
        }

        public void RemoveButtons()
        {
            foreach (var it in buttons)
                it.Button.Pool.Despawn(it.Button);
            buttons.Clear();
        }

        public bool TryGetSelected(out DialogNode node)
        {
            foreach (var it in buttons) {
                if (it.Button.Selected) {
                    node = it.Node;
                    return true;
                }
            }

            node = null;
            return false;
        }
    }
}
