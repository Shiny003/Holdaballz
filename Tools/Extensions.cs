using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Holdaballz.Tools
{
    public static class Extensions
    {
        public static float Distance(this Vector3 self, Vector3 other)
        {
            return Vector3.Distance(self, other);
        }
    }
}
