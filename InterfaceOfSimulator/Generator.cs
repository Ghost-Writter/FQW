using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace InterfaceOfSimulator
{
    public interface IGenerationMethod
    {
        byte[] Generate();
    }

    public sealed class Generator
    {
        public sealed class ConstantBit : IGenerationMethod
        {
            public ConstantBit(bool bit)
            {
                mBit = bit;
            }

            public byte[] Generate()
            {
                return BitConverter.GetBytes(mBit);
            }

            private bool mBit;
        }

        public sealed class RandomBit : IGenerationMethod
        {
            public byte[] Generate()
            {
                bool result = rnd.Next(2) > 0 ? true : false;

                return BitConverter.GetBytes(result);
            }
        }

        public sealed class OutputValue : IGenerationMethod
        {
            public OutputValue(ISignalOutput signalOutput)
            {
                mSignalOutput = signalOutput;
            }

            public byte[] Generate()
            {
                return mSignalOutput.Data;
            }

            private ISignalOutput mSignalOutput;
        }

        public sealed class CurrentTemperature : IGenerationMethod
        {
            public CurrentTemperature(IModuleOutput moduleOutput)
            {
                mModuleOutput = moduleOutput;
            }

            public byte[] Generate()
            {
                int countVentSection = GetCountVentSection();

                pvTemperature += (countVentSection - 6) * (-0.01f) + 0.0001f;

                return BitConverter.GetBytes(pvTemperature);
            }

            private int GetCountVentSection()
            {
                int countVentSection = 0;

                foreach (ISignalOutput item in mModuleOutput.GetSignalCollection())
                {
                    if (item.Title == Constants.TitleSignal.Q_VS_WORKING)
                    {
                        bool isWorking = false;

                        isWorking = BitConverter.ToBoolean(item.Data, 0);

                        if (isWorking)
                        {
                            countVentSection++;
                        }
                    }
                }

                ventSection = countVentSection;
                return countVentSection;
            }

            private IModuleOutput mModuleOutput;
        }

        public sealed class CurrentPressure : IGenerationMethod
        {
            public CurrentPressure(IModuleOutput moduleOutput)
            {
                mModuleOutput = moduleOutput;
            }

            public byte[] Generate()
            {
                int countPump = GetCountPump();

                pressure = countPump * 10.0f;

                return BitConverter.GetBytes(pressure);
            }

            private int GetCountPump()
            {
                int countPump = 0;

                foreach (ISignalOutput item in mModuleOutput.GetSignalCollection())
                {
                    if (item.Title == Constants.TitleSignal.Q_PUMP_WORKING)
                    {
                        bool isWorking = false;

                        isWorking = BitConverter.ToBoolean(item.Data, 0);

                        if (isWorking)
                        {
                            countPump++;
                        }
                    }
                }

                pump = countPump;
                return countPump;
            }

            private IModuleOutput mModuleOutput;
        }

        public static float GetTemperature()
        {
            return pvTemperature;
        }

        public static float GetPressure()
        {
            return pressure;
        }

        public static int GetVentSection()
        {
            return ventSection;
        }

        public static int GetPump()
        {
            return pump;
        }

        private static Random rnd = new Random(DateTime.Now.Millisecond);
        private static float pvTemperature = 0.0f;
        private static float pressure = 0.0f;
        private static int ventSection = 0;
        private static int pump = 0;
    }
}
