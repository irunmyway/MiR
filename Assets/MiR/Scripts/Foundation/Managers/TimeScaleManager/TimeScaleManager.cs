using UnityEngine;
using System.Collections.Generic;

namespace Foundation
{
    public sealed class TimeScaleManager : AbstractService<ITimeScaleManager>, ITimeScaleManager
    {
        List<TimeScaleHandle> handles = new List<TimeScaleHandle>(10);

        void Awake()
        {
            Time.timeScale = 1.0f;
        }

        void UpdateTimeScale()
        {
            float scale = 1.0f;
            foreach (var handle in handles)
                scale *= handle.Scale;
            Time.timeScale = scale;
        }

        public void BeginTimeScale(TimeScaleHandle handle, float scale)
        {
            (handle as ITimeScaleHandleInternal).Init(scale);
            handles.Add(handle);
            UpdateTimeScale();
        }

        public void EndTimeScale(TimeScaleHandle handle)
        {
            handles.Remove(handle);
            (handle as ITimeScaleHandleInternal).Reset();
            UpdateTimeScale();
        }
    }
}
