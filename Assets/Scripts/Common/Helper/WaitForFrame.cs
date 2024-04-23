using UnityEngine;

namespace Common.Helper
{
    public class WaitForFrame : CustomYieldInstruction
    {
        private readonly int _currentFrameCount;
        private readonly int _count;
        
        public override bool keepWaiting => Time.frameCount < _currentFrameCount + _count;

        public WaitForFrame(int count)
        {
            _count = count;
            _currentFrameCount = Time.frameCount;
        }
        
    }
}