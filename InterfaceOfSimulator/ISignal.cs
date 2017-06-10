using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using S7PROSIMLib;

namespace InterfaceOfSimulator
{
    public interface ISignal
    {
        int ByteIndex { get; }
        PointDataTypeConstants DataType { get; }
        byte[] Data { get; }
        Object GetDataObject();
        string Title { get; }
    }

    public interface ISignalDigital : ISignal
    {
        int BitIndex { get; }
    }

    public interface ISignalAnalog : ISignal
    {

    }

    public interface ISignalInput : ISignal
    {
        void Generate();
        void Send();
    }

    public interface ISignalOutput : ISignal
    {
        void Read();
    }

    public interface ISignalCollection : ICollection<ISignal>
    {
        bool Contains(int byteIndex, int bitIndex);
        bool Contains(int byteIndex);
    }
}
