using maddox.GP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IL2DCE
{
    public class DistanceComparer : IComparer<AirGroup>, IComparer<GroundGroup>
    {
        Point3d _position3d;
        Point2d _position2d;

        public DistanceComparer(Point3d position)
        {
            _position3d = position;
            _position2d = new Point2d(position.x, position.y);
        }

        public DistanceComparer(Point2d position)
        {
            _position3d = new Point3d(position.x, position.y, 0.0);
            _position2d = position;
        }

        public int Compare(AirGroup x, AirGroup y)
        {
            return x.Position.distance(ref _position3d).CompareTo(y.Position.distance(ref _position3d));
        }

        public int Compare(GroundGroup x, GroundGroup y)
        {
            return x.Position.distance(ref _position2d).CompareTo(y.Position.distance(ref _position2d));
        }

        //public int Compare(KeyValuePair<int, Point3d> x, KeyValuePair<int, Point3d> y)
        //{
        //    return x.Value.distance(ref _position3d).CompareTo(y.Value.distance(ref _position3d));
        //}        
    }    
}
