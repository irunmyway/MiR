using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;

namespace Foundation
{
    [RequireComponent(typeof(Button))]
    public sealed class DialogButton : AbstractBehaviour, IPoolable<string, IMemoryPool>
    {
        public sealed class Factory : PlaceholderFactory<string, DialogButton>
        {
        }

        Button button;
        public TextMeshProUGUI Text;

        Transform originalParent;
        public IMemoryPool Pool { get; private set; }

        [ReadOnly] public bool Selected;

        void Awake()
        {
            originalParent = transform.parent;
            button = GetComponent<Button>();
        }

        public void OnSpawned(string text, IMemoryPool pool)
        {
            Pool = pool;
            Text.text = text;
            Selected = false;
            gameObject.SetActive(true);
        }

        public void OnDespawned()
        {
            gameObject.SetActive(false);
            gameObject.name = "<inactive button>";
            transform.SetParent(originalParent, false);
        }

        public void Select()
        {
            Selected = true;
        }
    }
}
