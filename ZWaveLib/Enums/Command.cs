/*
    This file is part of ZWaveLib Project source code.

    ZWaveLib is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    ZWaveLib is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with ZWaveLib.  If not, see <http://www.gnu.org/licenses/>.  
*/

/*
 *     Author: Generoso Martello <gene@homegenie.it>
 *     Project Homepage: https://github.com/genielabs/zwave-lib-dotnet
 */

namespace ZWaveLib.Enums
{
    internal static class Command
    {
        internal class Basic
        {
            public const byte Set = 0x01;
            public const byte Get = 0x02;
            public const byte Report = 0x03;
        }

        internal class SwitchBinary
        {
            public const byte Set = 0x01;
            public const byte Get = 0x02;
            public const byte Report = 0x03;
        }

        internal class SwitchMultilevel
        {
            public const byte Set = 0x01;
            public const byte Get = 0x02;
            public const byte Report = 0x03;
            public const byte StartLevelChange = 0x04;
            public const byte StopLevelChange = 0x05;
            public const byte SupportedGet = 0x06;
            public const byte SupportedReport = 0x07;
        }

        internal class Battery
        {
            public const byte Get = 0x02;
            public const byte Report = 0x03;
        }

        internal class Meter
        {
            public const byte Get = 0x01;
            public const byte Report = 0x02;
            public const byte SupportedGet = 0x03;
            public const byte SupportedReport = 0x04;
            public const byte Reset = 0x05;
        }

        internal class SensorBinary
        {
            public const byte Get = 0x02;
            public const byte Report = 0x03;
            public const byte SupportedGet = 0x01;
            public const byte SupportedReport = 0x04;
        }

        internal class SensorMultilevel
        {
            public const byte Get = 0x04;
            public const byte Report = 0x05;
        }

        internal class SensorAlarm
        {
            public const byte Get = 0x01;
            public const byte Report = 0x02;
            public const byte SupportedGet = 0x03;
            public const byte SupportedReport = 0x04;
        }

        internal class Alarm
        {
            public const byte Get = 0x04;
            public const byte Report = 0x05;
        }

        internal class MultiInstance
        {
            public const byte Set = 0x01;
            public const byte Get = 0x02;
            public const byte CountGet = 0x04;
            public const byte CountReport = 0x05;
            public const byte Encapsulated = 0x06;
        }

        internal class MultiChannel
        {
            public const byte Encapsulated = 0x0D;
        }

        internal class Association
        {
            public const byte Set = 0x01;
            public const byte Get = 0x02;
            public const byte Report = 0x03;
            public const byte Remove = 0x04;

        }

        internal class Configuration
        {
            public const byte Set = 0x04;
            public const byte Get = 0x05;
            public const byte Report = 0x06;
        }

        internal class ManufacturerSpecific
        {
            public const byte Get = 0x04;
        }

        internal class WakeUp
        {
            public const byte IntervalSet = 0x04;
            public const byte IntervalGet = 0x05;
            public const byte IntervalReport = 0x06;
            public const byte Notification = 0x07;
            public const byte NoMoreInfo = 0x08;
            public const byte IntervalCapabilitiesGet = 0x09;
            public const byte IntervalCapabilitiesReport = 0x0A;
        }

        internal class Version
        {
            public const byte Get = 0x11;
            public const byte Report = 0x12;
            public const byte CommandClassGet = 0x13;
            public const byte CommandClassReport = 0x14;
        }

        internal class Thermostat
        {
            public const byte FanModeGet = 0x02;
            public const byte FanModeReport = 0x03;
            public const byte FanModeSet = 0x01;
            public const byte FanModeSupportedGet = 0x04;
            public const byte FanModeSupportedReport = 0x05;

            public const byte FanStateGet = 0x02;
            public const byte FanStateReport = 0x03;

            public const byte ModeSet = 0x01;
            public const byte ModeGet = 0x02;
            public const byte ModeReport = 0x03;
            public const byte ModeSupportedGet = 0x04;
            public const byte ModeSupportedReport = 0x05;

            public const byte OperatingStateGet = 0x02;
            public const byte OperatingStateReport = 0x03;
            public const byte OperatingStateLoggingSupportedGet = 0x01;
            public const byte OperatingLoggingSupportedReport = 0x04;
            public const byte OperatingStateLoggingGet = 0x05;
            public const byte OperatingStateLoggingReport = 0x06;

            public const byte SetbackGet = 0x02;
            public const byte SetbackReport = 0x03;
            public const byte SetbackSet = 0x01;            

            public const byte SetPointSet = 0x01;
            public const byte SetPointGet = 0x02;
            public const byte SetPointReport = 0x03;
            public const byte SetPointSupportedGet = 0x04;
            public const byte SetPointSupportedReport = 0x05;
        }

        internal class Scene
        {
            public const byte ActivationSet = 0x01;
        }

        internal class UserCode
        {
            public const byte Report = 0x03;
            public const byte Set = 0x01;
        }

        internal class DoorLock
        {
            public const byte Set = 0x01;
            public const byte Get = 0x02;
            public const byte Report = 0x03;
            public const byte ConfigurationSet = 0x04;
            public const byte ConfigurationGet = 0x05;
            public const byte ConfigurationReport = 0x06;
        }

        internal class Clock
        {
            public const byte Version = 0x01;
            public const byte Get = 0x05;
            public const byte Set = 0x04;
            public const byte Report = 0x06;
        }

        internal class Irrigation
        {
            public const byte SystemInfoGet = 0x01; // IRRIGATION_SYSTEM_INFO_GET
            public const byte SystemInfoReport = 0x02; // IRRIGATION_SYSTEM_INFO_REPORT
            public const byte SystemStatusGet = 0x03; // IRRIGATION_SYSTEM_STATUS_GET
            public const byte SystemStatusReport = 0x04; // IRRIGATION_SYSTEM_STATUS_REPORT
            public const byte SystemConfigSet = 0x05; // IRRIGATION_SYSTEM_CONFIG_SET
            public const byte SystemConfigGet = 0x06; // IRRIGATION_SYSTEM_CONFIG_GET
            public const byte SystemConfigReport = 0x07; // IRRIGATION_SYSTEM_CONFIG_REPORT
            public const byte ValveInfoGet = 0x08; // IRRIGATION_VALVE_INFO_GET
            public const byte ValveInfoReport = 0x09; // IRRIGATION_VALVE_INFO_REPORT
            public const byte ValveConfigSet = 0x0A; // IRRIGATION_VALVE_CONFIG_SET
            public const byte ValveConfigGet = 0x0B; // IRRIGATION_VALVE_CONFIG_GET
            public const byte ValveRun = 0x0D; // IRRIGATION_VALVE_RUN
            public const byte ValveConfigReport = 0x0C; // IRRIGATION_VALVE_CONFIG_REPORT
            public const byte ValveTableSet = 0x0E; // IRRIGATION_VALVE_TABLE_SET
            public const byte ValveTableGet = 0x0F; // IRRIGATION_VALVE_TABLE_GET
            public const byte ValveTableReport = 0x10; // IRRIGATION_VALVE_TABLE_REPORT
            public const byte ValveTableRun = 0x11; // IRRIGATION_VALVE_TABLE_RUN
            public const byte SystemShutoff = 0x12; // IRRIGATION_SYSTEM_SHUTOFF
        }

        internal class Schedule
        {
            public const byte Set = 0x01;
            public const byte Get = 0x02;
            public const byte Report = 0x03;
            public const byte ChangedGet = 0x04;
            public const byte ChangedReport = 0x05;
            public const byte OverrideSet = 0x06;
            public const byte OverrideGet = 0x07;
            public const byte OverrideReport = 0x08;
        }

        internal class CentralScene
        {
            public const byte SupportedGet = 0x01;
            public const byte SupportedReport = 0x02;
            public const byte Notification = 0x03;
        }
    }
}
