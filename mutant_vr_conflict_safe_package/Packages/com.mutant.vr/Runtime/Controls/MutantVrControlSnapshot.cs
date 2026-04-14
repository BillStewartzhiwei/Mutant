using System;
using UnityEngine;

namespace Mutant.VR.Controls
{
    [Serializable]
    public struct MutantVrControlSnapshot
    {
        public int FrameIndex;
        public float RealtimeSinceStartup;

        public bool LeftTriggerPressed;
        public bool RightTriggerPressed;

        public bool LeftGripPressed;
        public bool RightGripPressed;

        public bool LeftPrimaryPressed;
        public bool RightPrimaryPressed;

        public Vector2 LeftStickAxis;
        public Vector2 RightStickAxis;
    }
}
