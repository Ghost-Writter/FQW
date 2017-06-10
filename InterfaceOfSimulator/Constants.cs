using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InterfaceOfSimulator
{
    public static class Constants
    {
        public static class TitleSignal
        {
            public static string I_VS_ENABLE = "I_VS_ENABLE";
            public static string I_VS_WORKING = "I_VS_WORKING";
            public static string I_VS_AUTOMODE = "I_VS_AUTOMODE";

            public static string Q_VS_WORKING = "Q_VS_WORKING";

            public static string I_PUMP_ENABLE = "I_PUMP_ENABLE";
            public static string I_PUMP_WORKING = "I_PUMP_WORKING";
            public static string I_PUMP_AUTOMODE = "I_PUMP_AUTOMODE";

            public static string Q_PUMP_WORKING = "Q_PUMP_WORKING";

            public static string ID_PVTEMPERATURE = "ID_PVTEMPERATURE";
            public static string ID_PRESSURE = "ID_PRESSURE";
        }

        public static int COUNT_VENT_SECTION = 12;
        public static int COUNT_PUMP = 4;
        public static int BYTE_SIZE = 8;
    }
}
