using System;
using System.Collections.Generic;
using ZWaveLib.CommandClasses;

namespace ZWaveLib
{
    public interface IZWaveNode
    {
        List<NodeCommandClass> CommandClasses { get; set; }
        List<NodeData> Data { get; }
        byte Id { get; set; }
        ManufacturerSpecificInfo ManufacturerSpecific { get; set; }
        byte[] NodeInformationFrame { get; set; }
        NodeCapabilities ProtocolInfo { get; set; }
        byte[] SecuredNodeInformationFrame { get; set; }
        NodeVersion Version { get; set; }

        event Action<object, NodeEvent> NodeUpdated;

        NodeCommandClass GetCommandClass(CommandClass cclass);
        NodeData GetData(string fieldId, object defaultValue = null);
        bool IsSecuredCommandClass(CommandClass commandClass);
        ZWaveMessage SendDataRequest(byte[] request);
        bool SupportCommandClass(CommandClass commandClass);
        void UpdateData(string fieldId, object value);

        ZWaveMessage SendMessage(byte[] message);
        void OnNodeUpdated(NodeEvent zevent);

        void ResendOnWakeUp(byte[] msg);
        void ResendQueuedMessages();
    }
}