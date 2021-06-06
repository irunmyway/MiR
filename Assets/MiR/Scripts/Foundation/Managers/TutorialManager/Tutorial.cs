using UnityEngine;

namespace Foundation
{
    public class Tutorial : AbstractBehaviour
    {
        [ReadOnly] [SerializeField] int currentStep;
        [ReadOnly] [SerializeField] int activeStep = -1;

        public bool Once = true;
        public TutorialStep[] Steps;
        public Tutorial Next;

        public bool WasShown { get; private set; }

        void Awake()
        {
            if (Steps == null)
                Steps = new TutorialStep[0];

            WasShown = false;//PlayerPrefs.GetInt($"Tutorial_{gameObject.name}", 0) != 0;
        }

        void EndActiveStep()
        {
            if (activeStep >= 0 && activeStep < Steps.Length) {
                Steps[activeStep].OnEnd();
                activeStep = -1;
            }
        }

        public void Restart()
        {
            EndActiveStep();
            currentStep = 0;
        }

        public bool UpdateCurrentStep(TutorialOverlay overlay)
        {
            if (!WasShown) {
                WasShown = true;
                PlayerPrefs.SetInt($"Tutorial_{gameObject.name}", 1);
            }

            for (;;) {
                if (currentStep >= Steps.Length) {
                    overlay.DisableHole();
                    overlay.DisableMessage();
                    EndActiveStep();
                    return false;
                }

                if (activeStep != currentStep) {
                    overlay.DisableHole();
                    overlay.DisableMessage();
                    EndActiveStep();

                    activeStep = currentStep;

                    if (activeStep >= 0 && activeStep < Steps.Length) {
                        var step = Steps[activeStep];

                        step.OnBegin();

                        if (step.FingerTarget != null)
                            overlay.EnableHole(step.FingerTarget, true);
                        if (step.Message != null)
                            overlay.EnableMessage(step.Message);
                    }
                }

                if (!Steps[currentStep].IsComplete())
                    return true;

                ++currentStep;
            }
        }
    }
}
