using UnityEngine;

namespace Source.Director
{
    public abstract class GameSegment : MonoBehaviour
    {
        public virtual void SegmentBegin() { }
        public abstract bool IsSegmentComplete();
        
        public virtual void SegmentEnd() { }
    }
}