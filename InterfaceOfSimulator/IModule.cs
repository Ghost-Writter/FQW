using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InterfaceOfSimulator
{
    public interface IModule
    {
        void Add(ISignal signal);
        ISignal Get(int byteIndex, int bitIndex);
        ISignal Get(int byteIndex);
        bool Contains(int byteIndex, int bitIndex);
        bool Contains(int byteIndex);
        ISignalCollection GetSignalCollection();
    }

    public interface IModuleInput : IModule
    {
        void Add(ISignalInput item);
        void Send();
        void Generate();
    }

    public interface IModuleOutput : IModule
    {
        void Add(ISignalOutput item);
        void Read();
    }

    public interface IModuleCollection : ICollection<IModule>
    {

    }
}
