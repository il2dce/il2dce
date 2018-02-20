using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IL2DCE
{
    public interface IRandom
    {
        int Next(int maxValue); 
        int Next(int minValue, int maxValue);
    }
}
