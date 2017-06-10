using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace InterfaceOfSimulator
{
    public sealed class PLC
    {
        public static PLC Instance
        {
            get
            {
                if (mController == null)
                {
                    mController = new PLC();
                }

                return mController;
            }
        }

        public void Dispose()
        {
        }

        public PLC Controller
        {
            private get { return mController; }
            set { mController = value; }
        }

        public void Load()
        {
            isWork = false;
            mThead = new Thread(Update);

            mModuleInput = new ModuleInput();
            mModuleOutput = new ModuleOutput();
        }

        public void Start()
        {
            isWork = true;

            mThead.Start();
        }

        private void Update()
        {
            while (isWork)
            {
                mModuleOutput.Read();

                mModuleInput.Generate();
                mModuleInput.Send();

                Thread.Sleep(100);
            }
        }

        public void Stop()
        {
            isWork = false;
            mThead.Join();
        }

        public void AddModuleInput(IModuleInput moduleInput)
        {
            mModuleInput = moduleInput;
        }

        public void AddModuleOutput(IModuleOutput moduleOutput)
        {
            mModuleOutput = moduleOutput;
        }

        public ISignal GetSignalInput(int byteIndex, int bitIndex)
        {
            return mModuleInput.Get(byteIndex, bitIndex);
        }

        public ISignal GetSignalInput(int byteIndex)
        {
            return mModuleInput.Get(byteIndex);
        }

        public ISignal GetSignalOutput(int byteIndex, int bitIndex)
        {
            return mModuleOutput.Get(byteIndex, bitIndex);
        }

        public ISignal GetSignalOutput(int byteIndex)
        {
            return mModuleOutput.Get(byteIndex);
        }

        private volatile bool isWork;
        private Thread mThead;

        private IModuleInput mModuleInput;
        private IModuleOutput mModuleOutput;

        private static PLC mController;
    }
}
