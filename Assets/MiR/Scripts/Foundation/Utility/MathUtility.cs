using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Foundation
{
    public static class MathUtility
    {
        // https://stackoverflow.com/questions/51905268/how-to-find-closest-point-on-line
        public static float FindNearestPointOnLine(Vector3 start, Vector3 end, Vector3 point)
        {
            //Get heading
            Vector3 heading = (end - start);
            float magnitudeMax = heading.magnitude;
            heading.Normalize();

            //Do projection from the point but clamp it
            Vector3 lhs = point - start;
            float dotP = Vector3.Dot(lhs, heading);
            dotP = Mathf.Clamp(dotP, 0f, magnitudeMax);
            return dotP / magnitudeMax;
        }
    }
}
