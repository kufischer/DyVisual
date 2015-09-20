using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BVHAnimationPlugin
{
    class BVHFrame
    {
        public string animationName;

        internal BVHFrame(string name, float trans, float quat)
        {
            animationName = name;
            translation = trans;
            quaternion = quat;
        }

        internal float translation;
        internal float quaternion;
    }
}
