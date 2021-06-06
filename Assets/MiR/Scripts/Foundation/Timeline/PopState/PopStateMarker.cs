using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Foundation
{
    [CustomStyle("PopStateMarker")]
    public sealed class PopStateMarker : Marker, INotification
    {
        public PropertyName id { get; }
    }
}
