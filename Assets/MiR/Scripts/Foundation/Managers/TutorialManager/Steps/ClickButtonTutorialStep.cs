using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Foundation
{
    public sealed class ClickButtonTutorialStep : TutorialStep
    {
        public Button Button;
        public string ButtonName;

        bool buttonClicked;

        public override RectTransform FingerTarget => Button.GetComponent<RectTransform>();

        public override void OnBegin()
        {
            if (!string.IsNullOrEmpty(ButtonName))//
                Button = GameObject.Find(ButtonName).GetComponent<Button>();

            buttonClicked = false;
            Button.onClick.AddListener(OnButtonClicked);
        }

        public override void OnEnd()
        {
            Button.onClick.RemoveListener(OnButtonClicked);
        }

        void OnButtonClicked()
        {
            buttonClicked = true;
        }

        public override bool IsComplete()
        {
            return buttonClicked;
        }
    }
}
