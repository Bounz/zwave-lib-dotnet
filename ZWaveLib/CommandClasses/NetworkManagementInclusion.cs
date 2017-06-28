using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWaveLib.Enums;

namespace ZWaveLib.CommandClasses
{
    public enum NodeAddMode : byte
    {
        /// <summary>
        /// Add any type of node to the network and allow Security 0 bootstrapping
        /// </summary>
        Any = 0x01,

        /// <summary>
        /// Stop adding nodes to the network
        /// </summary>
        Stop = 0x05,

        /// <summary>
        /// Add any type of node to the network and allow Security 0 or Security 2 bootstrapping
        /// </summary>
        AnyS2 = 0x07
    }

    [Flags]
    public enum TransmitOption
    {
        /// <summary>
        /// Transmit at normal power level without any transmit options.
        /// </summary>
        None = 0x0,

        /// <summary>
        /// Transmit at low output power level (1/3 of normal RF range)
        /// </summary>
        LowPower = 0x02,

        /// <summary>
        /// Allow network-wide inclusion
        /// </summary>
        Explore = 0x20
    }

    /// <summary>
    /// The Network Management Inclusion Command Class provides functionality only available in a primary
    /// controller, inclusion controller or SIS.
    /// </summary>
    /// <remarks>SDS13784-2 4.5.9 Network Management Inclusion Command Class, version 2</remarks>
    public class NetworkManagementInclusion : ICommandClass
    {
        public CommandClass GetClassId()
        {
            return CommandClass.CommandClassNetworkManagementInclusion;
        }

        public NodeEvent GetEvent(IZWaveNode node, byte[] message)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This command is used to add nodes to the Z-Wave network.
        /// </summary>
        /// <param name="mode">This field is use to indicate to the receiving node if the Add Mode must be activated or de-activated.</param>
        /// <param name="txOptions">The tx Options field allows a controlling node to specify if transmissions MUST use special properties.</param>
        /// <returns></returns>
        /// <remarks>SDS13784-2 4.5.9.2 Node Add Command</remarks>
        public static ZWaveMessage NodeAdd(NodeAddMode mode, TransmitOption txOptions)
        {
            var message = ZWaveMessage.BuildSendDataRequest(1, new byte[]
            {
                (byte) CommandClass.CommandClassNetworkManagementInclusion,
                Command.NetworkManagementInclusion.NodeAdd,
                0, //seq No
                0, //reserved
                (byte) mode, //mode
                (byte) txOptions // tx options
            });
            return new ZWaveMessage(message, MessageDirection.Outbound, true);
        }
    }
}
