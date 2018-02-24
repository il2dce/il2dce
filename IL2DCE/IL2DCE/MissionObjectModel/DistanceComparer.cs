using maddox.GP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IL2DCE
{
    public class DistanceComparer : IComparer<AirGroup>
    {
        Point3d _position;

        public DistanceComparer(Point3d position)
        {
            _position = position;
        }

        public int Compare(AirGroup x, AirGroup y)
        {
            return x.Position.distance(ref _position).CompareTo(y.Position.distance(ref _position));
        }
    }
}
