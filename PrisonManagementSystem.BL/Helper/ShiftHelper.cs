using PrisonManagementSystem.DAL.Enums;
using System;

namespace PrisonManagementSystem.Helpers
{
    public static class ShiftHelper
    {
       
        public static (TimeSpan Start, TimeSpan End) GetShiftTimes(ShiftType shiftType)
        {
            switch (shiftType)
            {
                case ShiftType.Morning:
                    return (new TimeSpan(6, 0, 0), new TimeSpan(14, 0, 0)); // 6:00 AM - 2:00 PM
                case ShiftType.Evening:
                    return (new TimeSpan(14, 0, 0), new TimeSpan(22, 0, 0)); // 2:00 PM - 10:00 PM
                case ShiftType.Night:
                    return (new TimeSpan(22, 0, 0), new TimeSpan(6, 0, 0)); // 10:00 PM - 6:00 AM
                default:
                    throw new ArgumentOutOfRangeException(nameof(shiftType), shiftType, null);
            }
        }
    }
}
