namespace BreakingStorm.Common.Helpers
{
    #region

    using System;

    #endregion

    public static class HelperDateTime
    {
        #region Public Methods and Operators

        public static DateTime GetDateTimeFromBytes(byte[] bytes, byte fromIndex)
        {
            double utcMilliseconds = BitConverter.ToDouble(bytes, fromIndex);
            return new DateTime((long)(utcMilliseconds * 10000d) + 621355968000000000, DateTimeKind.Local);
        }

        public static DateTime GetDateTimeFromUtcMilliseconds(double utcMilliseconds)
        {
            return new DateTime((long)(utcMilliseconds * 10000d) + 621355968000000000, DateTimeKind.Local);
        }


        public static double GetUtcMilliseconds(this DateTime date)
        {
            return (date.ToUniversalTime().Ticks - 621355968000000000) / 10000d;
        }

        #endregion
    }
}