using UnityEngine;

namespace Source.Director
{
    public class EnemySegment : GameSegment
    {
        public override void SegmentBegin()
        {
        }

        public override bool IsSegmentComplete()
        {
            return false;
        }

        public override void SegmentEnd()
        {
        }
    }
}