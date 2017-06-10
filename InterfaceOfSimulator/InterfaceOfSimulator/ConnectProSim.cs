using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using S7PROSIMLib;

namespace InterfaceOfSimulator
{
    public sealed class ConnectProSim
    {
        protected ConnectProSim() 
        {
            
        }

        public static ConnectProSim Instance
        {
            get
            {
                if (mInstance == null)
                {
                    mInstance = new ConnectProSim();
                }

                return mInstance;
            }
        }

        public void Connect()
        {
            mSimulator = new S7ProSim();
            mSimulator.Connect();
        }

        public void Disconnect()
        {
            if (mSimulator != null)
            {
                mSimulator.Disconnect();
                Dispose();
            }
        }

        public void Dispose()
        {
            System.Runtime.InteropServices.Marshal.ReleaseComObject(mSimulator);
            mSimulator = null;
        }

        public S7ProSim Connection
        {
            get
            {
                return mSimulator;
            }
        }

        private static ConnectProSim mInstance;
        private S7ProSim mSimulator;
    }
}
