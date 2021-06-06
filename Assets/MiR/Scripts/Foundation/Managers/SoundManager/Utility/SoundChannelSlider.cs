using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Foundation
{
    [RequireComponent(typeof(Slider))]
    public sealed class SoundChannelSlider : MonoBehaviour
    {
        public string ChannelName;

        [Inject] ISoundManager soundManager = default;
        ISoundChannel channel;

        void Awake()
        {
            channel = soundManager.GetChannel(ChannelName);

            var slider = GetComponent<Slider>();
            slider.value = channel.Volume;
            slider.onValueChanged.AddListener((value) => channel.Volume = value);
        }
    }
}
