using Foundation;
using UnityEngine;

namespace Game
{
    [ExecuteAlways]
    public sealed class CharacterVisual : AbstractService<ICharacterVisual>, ICharacterVisual
    {
        public int SelectedHead;
        public int SelectedHair;
        public int SelectedBeard;
        public int SelectedJacket;
        public int SelectedPants;
        public int SelectedBoots;

        public GameObject[] Heads;
        public GameObject[] Hairs;
        public GameObject[] Beards;
        public GameObject[] Jackets;
        public GameObject[] Pants;
        public GameObject[] Boots;

        int currentHead = -1;
        int currentHair = -1;
        int currentBeard = -1;
        int currentJacket = -1;
        int currentPants = -1;
        int currentBoots = -1;

        void Awake()
        {
            UpdateVisual(true);
        }

        public void Randomize()
        {
            RandomizeElement(ref SelectedHead, Heads, false);
            RandomizeElement(ref SelectedHair, Hairs, true);
            RandomizeElement(ref SelectedBeard, Beards, true);
            RandomizeElement(ref SelectedJacket, Jackets, false);
            RandomizeElement(ref SelectedPants, Pants, false);
            RandomizeElement(ref SelectedBoots, Boots, false);
            UpdateVisual(true);
        }

        void RandomizeElement(ref int value, GameObject[] items, bool optional)
        {
            if (items == null || items.Length == 0)
                value = -1;
            else {
                int min = optional ? -1 : 0;
                value = Random.Range(min, items.Length);
            }
        }

        public void UpdateVisual(bool force = false)
        {
            UpdateElement(SelectedHead, ref currentHead, Heads, force);
            UpdateElement(SelectedHair, ref currentHair, Hairs, force);
            UpdateElement(SelectedBeard, ref currentBeard, Beards, force);
            UpdateElement(SelectedJacket, ref currentJacket, Jackets, force);
            UpdateElement(SelectedPants, ref currentPants, Pants, force);
            UpdateElement(SelectedBoots, ref currentBoots, Boots, force);
        }

        void UpdateElement(int value, ref int current, GameObject[] items, bool force)
        {
            if ((current == value && !force) || items == null)
                return;

            current = value;

            int n = items.Length;
            while (n-- > 0)
                items[n].SetActive(n == current);
        }

      #if UNITY_EDITOR
        void Update()
        {
            if (!Application.IsPlaying(this))
                UpdateVisual(false);
        }
      #endif
    }
}
