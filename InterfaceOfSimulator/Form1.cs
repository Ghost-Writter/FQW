using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Reflection;
using S7PROSIMLib;

namespace InterfaceOfSimulator
{
    public partial class Form1 : Form
    {
        delegate void SetTextCallback(string text);

        public Form1()
        {
            InitializeComponent();
        }
        
        private void UpdateUI()
        {
            while (mIsWorkUI)
            {
                this.labelTemperature.BeginInvoke((MethodInvoker)(() => this.labelTemperature.Text = Generator.GetTemperature() + ""));
                this.labelPressure.BeginInvoke((MethodInvoker)(() => this.labelPressure.Text = Generator.GetPressure() + ""));
                this.labelCountVentSection.BeginInvoke((MethodInvoker)(() => this.labelCountVentSection.Text = Generator.GetVentSection() + "/" + Constants.COUNT_VENT_SECTION));
                this.labelCountPump.BeginInvoke((MethodInvoker)(() => this.labelCountPump.Text = Generator.GetPump() + "/" + Constants.COUNT_PUMP));

                int index = 1;

                for (int curByte = 24; curByte < 26; curByte++)
                {
                    for (int curBit = 0; curBit < Constants.BYTE_SIZE; curBit++)
                    {
                        ISignal signalDO = PLC.Instance.GetSignalOutput(curByte, curBit);
                        bool isWork = BitConverter.ToBoolean(signalDO.Data, 0);

                        Label currentLabel = new Label();
                        currentLabel = (Label)(this.Controls.Find("labelSignalDO" + index, true).FirstOrDefault());

                        if (!isWork)
                        {
                            currentLabel.BeginInvoke((MethodInvoker)(() => currentLabel.Text = "X"));
                            currentLabel.BackColor = System.Drawing.Color.Red;
                        }
                        else
                        {
                            currentLabel.BeginInvoke((MethodInvoker)(() => currentLabel.Text = "O"));
                            currentLabel.BackColor = System.Drawing.Color.GreenYellow;
                        }

                        index++;
                    }
                }

                Thread.Sleep(200);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ConnectProSim.Instance.Connect();
            ConnectProSim.Instance.Connection.SetScanMode(ScanModeConstants.ContinuousScan);

            //выходные статусы секций вентиляторов
            SignalDigitalOutput signalDO_24_0 = new SignalDigitalOutput(24, 0, PointDataTypeConstants.S7_Bit, Constants.TitleSignal.Q_VS_WORKING);
            SignalDigitalOutput signalDO_24_1 = new SignalDigitalOutput(24, 1, PointDataTypeConstants.S7_Bit, Constants.TitleSignal.Q_VS_WORKING);
            SignalDigitalOutput signalDO_24_2 = new SignalDigitalOutput(24, 2, PointDataTypeConstants.S7_Bit, Constants.TitleSignal.Q_VS_WORKING);
            SignalDigitalOutput signalDO_24_3 = new SignalDigitalOutput(24, 3, PointDataTypeConstants.S7_Bit, Constants.TitleSignal.Q_VS_WORKING);
            SignalDigitalOutput signalDO_24_4 = new SignalDigitalOutput(24, 4, PointDataTypeConstants.S7_Bit, Constants.TitleSignal.Q_VS_WORKING);
            SignalDigitalOutput signalDO_24_5 = new SignalDigitalOutput(24, 5, PointDataTypeConstants.S7_Bit, Constants.TitleSignal.Q_VS_WORKING);
            SignalDigitalOutput signalDO_24_6 = new SignalDigitalOutput(24, 6, PointDataTypeConstants.S7_Bit, Constants.TitleSignal.Q_VS_WORKING);
            SignalDigitalOutput signalDO_24_7 = new SignalDigitalOutput(24, 7, PointDataTypeConstants.S7_Bit, Constants.TitleSignal.Q_VS_WORKING);
            SignalDigitalOutput signalDO_25_0 = new SignalDigitalOutput(25, 0, PointDataTypeConstants.S7_Bit, Constants.TitleSignal.Q_VS_WORKING);
            SignalDigitalOutput signalDO_25_1 = new SignalDigitalOutput(25, 1, PointDataTypeConstants.S7_Bit, Constants.TitleSignal.Q_VS_WORKING);
            SignalDigitalOutput signalDO_25_2 = new SignalDigitalOutput(25, 2, PointDataTypeConstants.S7_Bit, Constants.TitleSignal.Q_VS_WORKING);
            SignalDigitalOutput signalDO_25_3 = new SignalDigitalOutput(25, 3, PointDataTypeConstants.S7_Bit, Constants.TitleSignal.Q_VS_WORKING);
            //выходные статусы насосов
            SignalDigitalOutput signalDO_25_4 = new SignalDigitalOutput(25, 4, PointDataTypeConstants.S7_Bit, Constants.TitleSignal.Q_PUMP_WORKING);
            SignalDigitalOutput signalDO_25_5 = new SignalDigitalOutput(25, 5, PointDataTypeConstants.S7_Bit, Constants.TitleSignal.Q_PUMP_WORKING);
            SignalDigitalOutput signalDO_25_6 = new SignalDigitalOutput(25, 6, PointDataTypeConstants.S7_Bit, Constants.TitleSignal.Q_PUMP_WORKING);
            SignalDigitalOutput signalDO_25_7 = new SignalDigitalOutput(25, 7, PointDataTypeConstants.S7_Bit, Constants.TitleSignal.Q_PUMP_WORKING);

            ModuleOutput moduleOutput = new ModuleOutput();
            moduleOutput.Add(signalDO_24_0);
            moduleOutput.Add(signalDO_24_1);
            moduleOutput.Add(signalDO_24_2);
            moduleOutput.Add(signalDO_24_3);
            moduleOutput.Add(signalDO_24_4);
            moduleOutput.Add(signalDO_24_5);
            moduleOutput.Add(signalDO_24_6);
            moduleOutput.Add(signalDO_24_7);
            moduleOutput.Add(signalDO_25_0);
            moduleOutput.Add(signalDO_25_1);
            moduleOutput.Add(signalDO_25_2);
            moduleOutput.Add(signalDO_25_3);

            moduleOutput.Add(signalDO_25_4);
            moduleOutput.Add(signalDO_25_5);
            moduleOutput.Add(signalDO_25_6);
            moduleOutput.Add(signalDO_25_7);

            //секция №1
            SignalDigitalInput signalDI_16_0 = new SignalDigitalInput(16, 0, PointDataTypeConstants.S7_Bit, Constants.TitleSignal.I_VS_ENABLE, new Generator.ConstantBit(true));
            SignalDigitalInput signalDI_16_1 = new SignalDigitalInput(16, 1, PointDataTypeConstants.S7_Bit, Constants.TitleSignal.I_VS_WORKING, new Generator.OutputValue(signalDO_24_0));
            SignalDigitalInput signalDI_16_2 = new SignalDigitalInput(16, 2, PointDataTypeConstants.S7_Bit, Constants.TitleSignal.I_VS_AUTOMODE, new Generator.ConstantBit(true));
            //секция №2
            SignalDigitalInput signalDI_16_3 = new SignalDigitalInput(16, 3, PointDataTypeConstants.S7_Bit, Constants.TitleSignal.I_VS_ENABLE, new Generator.ConstantBit(true));
            SignalDigitalInput signalDI_16_4 = new SignalDigitalInput(16, 4, PointDataTypeConstants.S7_Bit, Constants.TitleSignal.I_VS_WORKING, new Generator.OutputValue(signalDO_24_1));
            SignalDigitalInput signalDI_16_5 = new SignalDigitalInput(16, 5, PointDataTypeConstants.S7_Bit, Constants.TitleSignal.I_VS_AUTOMODE, new Generator.ConstantBit(true));
            //секция №3
            SignalDigitalInput signalDI_16_6 = new SignalDigitalInput(16, 6, PointDataTypeConstants.S7_Bit, Constants.TitleSignal.I_VS_ENABLE, new Generator.ConstantBit(true));
            SignalDigitalInput signalDI_16_7 = new SignalDigitalInput(16, 7, PointDataTypeConstants.S7_Bit, Constants.TitleSignal.I_VS_WORKING, new Generator.OutputValue(signalDO_24_2));
            SignalDigitalInput signalDI_17_0 = new SignalDigitalInput(17, 0, PointDataTypeConstants.S7_Bit, Constants.TitleSignal.I_VS_AUTOMODE, new Generator.ConstantBit(true));
            //секция №4
            SignalDigitalInput signalDI_17_1 = new SignalDigitalInput(17, 1, PointDataTypeConstants.S7_Bit, Constants.TitleSignal.I_VS_ENABLE, new Generator.ConstantBit(true));
            SignalDigitalInput signalDI_17_2 = new SignalDigitalInput(17, 2, PointDataTypeConstants.S7_Bit, Constants.TitleSignal.I_VS_WORKING, new Generator.OutputValue(signalDO_24_3));
            SignalDigitalInput signalDI_17_3 = new SignalDigitalInput(17, 3, PointDataTypeConstants.S7_Bit, Constants.TitleSignal.I_VS_AUTOMODE, new Generator.ConstantBit(true));
            //секция №5
            SignalDigitalInput signalDI_17_4 = new SignalDigitalInput(17, 4, PointDataTypeConstants.S7_Bit, Constants.TitleSignal.I_VS_ENABLE, new Generator.ConstantBit(true));
            SignalDigitalInput signalDI_17_5 = new SignalDigitalInput(17, 5, PointDataTypeConstants.S7_Bit, Constants.TitleSignal.I_VS_WORKING, new Generator.OutputValue(signalDO_24_4));
            SignalDigitalInput signalDI_17_6 = new SignalDigitalInput(17, 6, PointDataTypeConstants.S7_Bit, Constants.TitleSignal.I_VS_AUTOMODE, new Generator.ConstantBit(true));
            //секция №6
            SignalDigitalInput signalDI_17_7 = new SignalDigitalInput(17, 7, PointDataTypeConstants.S7_Bit, Constants.TitleSignal.I_VS_ENABLE, new Generator.ConstantBit(true));
            SignalDigitalInput signalDI_18_0 = new SignalDigitalInput(18, 0, PointDataTypeConstants.S7_Bit, Constants.TitleSignal.I_VS_WORKING, new Generator.OutputValue(signalDO_24_5));
            SignalDigitalInput signalDI_18_1 = new SignalDigitalInput(18, 1, PointDataTypeConstants.S7_Bit, Constants.TitleSignal.I_VS_AUTOMODE, new Generator.ConstantBit(true));
            //секция №7
            SignalDigitalInput signalDI_18_2 = new SignalDigitalInput(18, 2, PointDataTypeConstants.S7_Bit, Constants.TitleSignal.I_VS_ENABLE, new Generator.ConstantBit(true));
            SignalDigitalInput signalDI_18_3 = new SignalDigitalInput(18, 3, PointDataTypeConstants.S7_Bit, Constants.TitleSignal.I_VS_WORKING, new Generator.OutputValue(signalDO_24_6));
            SignalDigitalInput signalDI_18_4 = new SignalDigitalInput(18, 4, PointDataTypeConstants.S7_Bit, Constants.TitleSignal.I_VS_AUTOMODE, new Generator.ConstantBit(true));
            //секция №8
            SignalDigitalInput signalDI_18_5 = new SignalDigitalInput(18, 5, PointDataTypeConstants.S7_Bit, Constants.TitleSignal.I_VS_ENABLE, new Generator.ConstantBit(true));
            SignalDigitalInput signalDI_18_6 = new SignalDigitalInput(18, 6, PointDataTypeConstants.S7_Bit, Constants.TitleSignal.I_VS_WORKING, new Generator.OutputValue(signalDO_24_7));
            SignalDigitalInput signalDI_18_7 = new SignalDigitalInput(18, 7, PointDataTypeConstants.S7_Bit, Constants.TitleSignal.I_VS_AUTOMODE, new Generator.ConstantBit(true));
            //секция №9
            SignalDigitalInput signalDI_19_0 = new SignalDigitalInput(19, 0, PointDataTypeConstants.S7_Bit, Constants.TitleSignal.I_VS_ENABLE, new Generator.ConstantBit(true));
            SignalDigitalInput signalDI_19_1 = new SignalDigitalInput(19, 1, PointDataTypeConstants.S7_Bit, Constants.TitleSignal.I_VS_WORKING, new Generator.OutputValue(signalDO_25_0));
            SignalDigitalInput signalDI_19_2 = new SignalDigitalInput(19, 2, PointDataTypeConstants.S7_Bit, Constants.TitleSignal.I_VS_AUTOMODE, new Generator.ConstantBit(true));
            //секция №10
            SignalDigitalInput signalDI_19_3 = new SignalDigitalInput(19, 3, PointDataTypeConstants.S7_Bit, Constants.TitleSignal.I_VS_ENABLE, new Generator.ConstantBit(true));
            SignalDigitalInput signalDI_19_4 = new SignalDigitalInput(19, 4, PointDataTypeConstants.S7_Bit, Constants.TitleSignal.I_VS_WORKING, new Generator.OutputValue(signalDO_25_1));
            SignalDigitalInput signalDI_19_5 = new SignalDigitalInput(19, 5, PointDataTypeConstants.S7_Bit, Constants.TitleSignal.I_VS_AUTOMODE, new Generator.ConstantBit(true));
            //секция №11
            SignalDigitalInput signalDI_19_6 = new SignalDigitalInput(19, 6, PointDataTypeConstants.S7_Bit, Constants.TitleSignal.I_VS_ENABLE, new Generator.ConstantBit(true));
            SignalDigitalInput signalDI_19_7 = new SignalDigitalInput(19, 7, PointDataTypeConstants.S7_Bit, Constants.TitleSignal.I_VS_WORKING, new Generator.OutputValue(signalDO_25_2));
            SignalDigitalInput signalDI_20_0 = new SignalDigitalInput(20, 0, PointDataTypeConstants.S7_Bit, Constants.TitleSignal.I_VS_AUTOMODE, new Generator.ConstantBit(true));
            //секция №12
            SignalDigitalInput signalDI_20_1 = new SignalDigitalInput(20, 1, PointDataTypeConstants.S7_Bit, Constants.TitleSignal.I_VS_ENABLE, new Generator.ConstantBit(true));
            SignalDigitalInput signalDI_20_2 = new SignalDigitalInput(20, 2, PointDataTypeConstants.S7_Bit, Constants.TitleSignal.I_VS_WORKING, new Generator.OutputValue(signalDO_25_3));
            SignalDigitalInput signalDI_20_3 = new SignalDigitalInput(20, 3, PointDataTypeConstants.S7_Bit, Constants.TitleSignal.I_VS_AUTOMODE, new Generator.ConstantBit(true));

            //насос №1
            SignalDigitalInput signalDI_20_4 = new SignalDigitalInput(20, 4, PointDataTypeConstants.S7_Bit, Constants.TitleSignal.I_PUMP_ENABLE, new Generator.ConstantBit(true));
            SignalDigitalInput signalDI_20_5 = new SignalDigitalInput(20, 5, PointDataTypeConstants.S7_Bit, Constants.TitleSignal.I_PUMP_WORKING, new Generator.OutputValue(signalDO_25_4));
            SignalDigitalInput signalDI_20_6 = new SignalDigitalInput(20, 6, PointDataTypeConstants.S7_Bit, Constants.TitleSignal.I_PUMP_AUTOMODE, new Generator.ConstantBit(true));
            //насос №2
            SignalDigitalInput signalDI_20_7 = new SignalDigitalInput(20, 7, PointDataTypeConstants.S7_Bit, Constants.TitleSignal.I_PUMP_ENABLE, new Generator.ConstantBit(true));
            SignalDigitalInput signalDI_21_0 = new SignalDigitalInput(21, 0, PointDataTypeConstants.S7_Bit, Constants.TitleSignal.I_PUMP_WORKING, new Generator.OutputValue(signalDO_25_5));
            SignalDigitalInput signalDI_21_1 = new SignalDigitalInput(21, 1, PointDataTypeConstants.S7_Bit, Constants.TitleSignal.I_PUMP_AUTOMODE, new Generator.ConstantBit(true));
            //насос №3
            SignalDigitalInput signalDI_21_2 = new SignalDigitalInput(21, 2, PointDataTypeConstants.S7_Bit, Constants.TitleSignal.I_PUMP_ENABLE, new Generator.ConstantBit(true));
            SignalDigitalInput signalDI_21_3 = new SignalDigitalInput(21, 3, PointDataTypeConstants.S7_Bit, Constants.TitleSignal.I_PUMP_WORKING, new Generator.OutputValue(signalDO_25_6));
            SignalDigitalInput signalDI_21_4 = new SignalDigitalInput(21, 4, PointDataTypeConstants.S7_Bit, Constants.TitleSignal.I_PUMP_AUTOMODE, new Generator.ConstantBit(true));
            //насос №4
            SignalDigitalInput signalDI_21_5 = new SignalDigitalInput(21, 5, PointDataTypeConstants.S7_Bit, Constants.TitleSignal.I_PUMP_ENABLE, new Generator.ConstantBit(true));
            SignalDigitalInput signalDI_21_6 = new SignalDigitalInput(21, 6, PointDataTypeConstants.S7_Bit, Constants.TitleSignal.I_PUMP_WORKING, new Generator.OutputValue(signalDO_25_7));
            SignalDigitalInput signalDI_21_7 = new SignalDigitalInput(21, 7, PointDataTypeConstants.S7_Bit, Constants.TitleSignal.I_PUMP_AUTOMODE, new Generator.ConstantBit(true));

            ModuleInput moduleInput = new ModuleInput();
            moduleInput.Add(signalDI_16_0);
            moduleInput.Add(signalDI_16_1);
            moduleInput.Add(signalDI_16_2);
            moduleInput.Add(signalDI_16_3);
            moduleInput.Add(signalDI_16_4);
            moduleInput.Add(signalDI_16_5);
            moduleInput.Add(signalDI_16_6);
            moduleInput.Add(signalDI_16_7);
            moduleInput.Add(signalDI_17_0);
            moduleInput.Add(signalDI_17_1);
            moduleInput.Add(signalDI_17_2);
            moduleInput.Add(signalDI_17_3);
            moduleInput.Add(signalDI_17_4);
            moduleInput.Add(signalDI_17_5);
            moduleInput.Add(signalDI_17_6);
            moduleInput.Add(signalDI_17_7);
            moduleInput.Add(signalDI_18_0);
            moduleInput.Add(signalDI_18_1);
            moduleInput.Add(signalDI_18_2);
            moduleInput.Add(signalDI_18_3);
            moduleInput.Add(signalDI_18_4);
            moduleInput.Add(signalDI_18_5);
            moduleInput.Add(signalDI_18_6);
            moduleInput.Add(signalDI_18_7);
            moduleInput.Add(signalDI_19_0);
            moduleInput.Add(signalDI_19_1);
            moduleInput.Add(signalDI_19_2);
            moduleInput.Add(signalDI_19_3);
            moduleInput.Add(signalDI_19_4);
            moduleInput.Add(signalDI_19_5);
            moduleInput.Add(signalDI_19_6);
            moduleInput.Add(signalDI_19_7);
            moduleInput.Add(signalDI_20_0);
            moduleInput.Add(signalDI_20_1);
            moduleInput.Add(signalDI_20_2);
            moduleInput.Add(signalDI_20_3);

            moduleInput.Add(signalDI_20_4);
            moduleInput.Add(signalDI_20_5);
            moduleInput.Add(signalDI_20_6);
            moduleInput.Add(signalDI_20_7);
            moduleInput.Add(signalDI_21_0);
            moduleInput.Add(signalDI_21_1);
            moduleInput.Add(signalDI_21_2);
            moduleInput.Add(signalDI_21_3);
            moduleInput.Add(signalDI_21_4);
            moduleInput.Add(signalDI_21_5);
            moduleInput.Add(signalDI_21_6);
            moduleInput.Add(signalDI_21_7);

            SignalAnalogInput signalAI_72 = new SignalAnalogInput(72, PointDataTypeConstants.S7_DoubleWord, Constants.TitleSignal.ID_PVTEMPERATURE, new Generator.CurrentTemperature(moduleOutput));
            moduleInput.Add(signalAI_72);

            SignalAnalogInput signalAI_78 = new SignalAnalogInput(78, PointDataTypeConstants.S7_DoubleWord, Constants.TitleSignal.ID_PRESSURE, new Generator.CurrentPressure(moduleOutput));
            moduleInput.Add(signalAI_78);

            PLC.Instance.Load();
            PLC.Instance.AddModuleOutput(moduleOutput);
            PLC.Instance.AddModuleInput(moduleInput);

            PLC.Instance.Start();

            mIsWorkUI = true;
            mThreadUI = new Thread(UpdateUI);
            mThreadUI.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            PLC.Instance.Stop();
            ConnectProSim.Instance.Disconnect();

            mIsWorkUI = false;
            mThreadUI.Join();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            ConnectProSim.Instance.Disconnect();
        }

        private Thread mThreadUI;
        private volatile bool mIsWorkUI;
    }
}
