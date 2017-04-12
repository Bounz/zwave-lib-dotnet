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

using System;
using System.Collections.Generic;
using System.Dynamic;
using ZWaveLib.Enums;
using ZWaveLib.Values;

namespace ZWaveLib.CommandClasses
{
    /// <summary>
    /// The Thermostat Setpoint Command Class is used for setpoint handling. Version 2 extends the available
    /// number of setpoint types.
    /// </summary>
    /// <remarks>SDS12652 3.44 Thermostat Setpoint Command Class, version 1-2</remarks>
    public class ThermostatSetPoint : ICommandClass
    {
        [Obsolete("Use SetpointType enum")]
        public enum Value
        {
            Unused = 0x00,
            Heating = 0x01,
            Cooling = 0x02,
            Unused03 = 0x03,
            Unused04 = 0x04,
            Unused05 = 0x05,
            Unused06 = 0x06,
            Furnace = 0x07,
            DryAir = 0x08,
            MoistAir = 0x09,
            AutoChangeover = 0x0A,
            HeatingEconomy = 0x0B,
            CoolingEconomy = 0x0C,
            HeatingAway = 0x0D
        }

        public enum SetpointType
        {
            Unused = 0x00,
            Heating = 0x01,
            Cooling = 0x02,
            Unused03 = 0x03,
            Unused04 = 0x04,
            Unused05 = 0x05,
            Unused06 = 0x06,
            Furnace = 0x07,
            DryAir = 0x08,
            MoistAir = 0x09,
            AutoChangeover = 0x0A,
            HeatingEconomy = 0x0B,
            CoolingEconomy = 0x0C,
            HeatingAway = 0x0D
        }

        public CommandClass GetClassId()
        {
            return CommandClass.ThermostatSetPoint;
        }

        // SDS12652 3.44.4 Thermostat Setpoint Report Command
        // SDS12652 3.44.6 Thermostat Setpoint Supported Report Command - not implemented yet
        public NodeEvent GetEvent(IZWaveNode node, byte[] message)
        {
            NodeEvent nodeEvent = null;
            var cmdType = message[1];
            switch (cmdType)
            {
                case Command.Thermostat.SetPointReport:
                    var zWaveValue = ZWaveValue.ExtractValueFromBytes(message, 4);
                    var setPoint = GetSetPointData(node);
                    setPoint.Precision = zWaveValue.Precision;
                    setPoint.Scale = zWaveValue.Scale;
                    setPoint.Size = zWaveValue.Size;
                    setPoint.Value = zWaveValue.Value;
                    var returnValue = new SetpointValue
                    {
                        Type = (SetpointType) message[2],
                        // convert from Fahrenheit to Celsius if needed
                        Value = zWaveValue.Scale == (int) ZWaveTemperatureScaleType.Fahrenheit
                            ? SensorValue.FahrenheitToCelsius(zWaveValue.Value)
                            : zWaveValue.Value
                    };
                    nodeEvent = new NodeEvent(node, EventParameter.ThermostatSetPoint, returnValue, 0);
                    break;

                case Command.Thermostat.SetPointSupportedReport:
                    break;

                default:
                    throw new Exception(string.Format("Unknown command: {0}", cmdType));
            }

            return nodeEvent;
        }

        /// <summary>
        /// The Thermostat Setpoint Set Command is used to specify the target value for the specified Setpoint Type.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="setpointType">Setpoint Type</param>
        /// <param name="temperature"></param>
        /// <returns></returns>
        /// <remarks>SDS12652 3.44.2 Thermostat Setpoint Set Command</remarks>
        public static ZWaveMessage Set(IZWaveNode node, SetpointType setpointType, double temperature)
        {
            var message = new List<byte>();
            message.AddRange(new[] {
                (byte)CommandClass.ThermostatSetPoint,
                Command.Thermostat.SetPointSet,
                (byte)setpointType
            });
            var setPoint = GetSetPointData(node);
            message.AddRange(ZWaveValue.GetValueBytes(temperature, setPoint.Precision, setPoint.Scale, setPoint.Size));
            return node.SendDataRequest(message.ToArray());
        }

        /// <summary>
        /// The Thermostat Setpoint Get Command is used to query the value of a specified setpoint type.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="setpointType">Setpoint Type</param>
        /// <returns></returns>
        /// <remarks>3.44.3 Thermostat Setpoint Get Command</remarks>
        public static ZWaveMessage Get(IZWaveNode node, SetpointType setpointType)
        {
            return node.SendDataRequest(new[] {
                (byte)CommandClass.ThermostatSetPoint,
                Command.Thermostat.SetPointGet,
                (byte)setpointType
            });
        }

        [Obsolete("Use Get(ZWaveNode node, SetpointType ptype) method instead")]
        public static ZWaveMessage Get(IZWaveNode node, Value ptype)
        {
            return node.SendDataRequest(new[] {
                (byte)CommandClass.ThermostatSetPoint,
                Command.Thermostat.SetPointGet,
                (byte)ptype
            });
        }

        [Obsolete("Use Set(ZWaveNode node, SetpointType ptype, double temperature) method instead.")]
        public static ZWaveMessage Set(IZWaveNode node, Value ptype, double temperature)
        {
            var message = new List<byte>();
            message.AddRange(new[] {
                (byte)CommandClass.ThermostatSetPoint,
                Command.Thermostat.SetPointSet,
                (byte)ptype
            });
            var setPoint = GetSetPointData(node);
            message.AddRange(ZWaveValue.GetValueBytes(temperature, setPoint.Precision, setPoint.Scale, setPoint.Size));
            return node.SendDataRequest(message.ToArray());
        }

        public static ZWaveValue GetSetPointData(IZWaveNode node)
        {
            return (ZWaveValue)node.GetData("SetPoint", new ZWaveValue()).Value;
        }

        /// <summary>
        /// The Thermostat Setpoint Supported Get Command is used to query the supported setpoint types.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <remarks>SDS12652 3.44.5 Thermostat Setpoint Supported Get Command</remarks>
        public static ZWaveMessage SupportedGet(IZWaveNode node)
        {
            throw new NotImplementedException();
        }
    }
}
