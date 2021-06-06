using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Foundation
{
    [RequireComponent(typeof(Toggle))]
    public sealed class SoundChannelToggle : MonoBehaviour
    {
        public string ChannelName;

        [Inject] ISoundManager soundManager = default;
        ISoundChannel channel;

        void Awake()
        {
            channel = soundManager.GetChannel(ChannelName);

            var toggle = GetComponent<Toggle>();
            toggle.isOn = channel.Enabled;
            toggle.onValueChanged.AddListener((value) => channel.Enabled = value);
        }
    }
}
