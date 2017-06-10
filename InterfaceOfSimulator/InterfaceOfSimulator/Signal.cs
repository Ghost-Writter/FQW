using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using S7PROSIMLib;

namespace InterfaceOfSimulator
{
    public class Signal : ISignal
    {
        public Signal(int byteIndex, PointDataTypeConstants dataType, string title)
        {
            ByteIndex = byteIndex;
            DataType = dataType;
            Data = BitConverter.GetBytes(0);
            Title = title;
        }

        public int ByteIndex
        {
            get { return mByteIndex; }
            private set { mByteIndex = value; }
        }

        public PointDataTypeConstants DataType
        {
            get { return mDataType; }
            private set { mDataType = value; }
        }

        public virtual byte[] Data
        {
            get { return mData; }
            set { mData = value; } 
        }

        public virtual Object GetDataObject()
        {
            Object tempObject = null;

            if(DataType == PointDataTypeConstants.S7_Bit)
            {
                tempObject = BitConverter.ToBoolean(Data, 0);
            }
            else if(DataType == PointDataTypeConstants.S7_Byte)
            {
                tempObject = BitConverter.ToInt16(Data, 0);
            }
            else if (DataType == PointDataTypeConstants.S7_Word)
            {
                tempObject = BitConverter.ToSingle(Data, 0);
            }
            else if (DataType == PointDataTypeConstants.S7_DoubleWord)
            {
                tempObject = BitConverter.ToSingle(Data, 0);
            }

            return tempObject;
        }

        public virtual string Title
        {
            get { return mTitle; }
            private set { mTitle = value; }
        }

        private int mByteIndex;
        private PointDataTypeConstants mDataType;
        private byte[] mData;
        private string mTitle;
    }

    public class SignalDigital : Signal, ISignalDigital
    {
        public SignalDigital(int byteIndex, int bitIndex, PointDataTypeConstants dataType, string title)
            : base(byteIndex, dataType, title)
        {
            BitIndex = bitIndex;
        }

        public int BitIndex
        {
            get { return mBitIndex; }
            private set { mBitIndex = value; }
        }

        private int mBitIndex;
    }

    public class SignalAnalog : Signal, ISignalAnalog
    {
        public SignalAnalog(int byteIndex, PointDataTypeConstants dataType, string title)
            : base(byteIndex, dataType, title)
        {

        }
    }

    public sealed class SignalDigitalInput : SignalDigital, ISignalInput
    {
        public SignalDigitalInput(int byteIndex, int bitIndex, PointDataTypeConstants dataType, string title, IGenerationMethod method)
            : base(byteIndex, bitIndex, dataType, title)
        {
            mGenerationMethod = method;
        }

        public void Generate()
        {
            Data = mGenerationMethod.Generate();
        }

        public void Send()
        {
            Object dataReformed = GetDataObject();

            ConnectProSim.Instance.Connection.WriteInputPoint(ByteIndex, BitIndex, ref dataReformed);

            if (System.Runtime.InteropServices.Marshal.IsComObject(dataReformed))
                System.Runtime.InteropServices.Marshal.ReleaseComObject(dataReformed);

            dataReformed = null;
        }

        private IGenerationMethod mGenerationMethod; 
    }

    public sealed class SignalAnalogInput : SignalAnalog, ISignalInput
    {
        public SignalAnalogInput(int byteIndex, PointDataTypeConstants dataType, string title, IGenerationMethod method)
            : base(byteIndex, dataType, title)
        {
            mGenerationMethod = method;
        }

        public void Generate()
        {
            Data = mGenerationMethod.Generate();
        }

        public void Send()
        {
            byte[] bufferArray = Data;
            Array.Reverse(bufferArray);

            Object dataReformed = new Object();
            dataReformed = bufferArray;

            ConnectProSim.Instance.Connection.WriteInputImage(ByteIndex, ref dataReformed); 
        }

        private IGenerationMethod mGenerationMethod; 
    }

    public sealed class SignalDigitalOutput : SignalDigital, ISignalOutput
    {
        public SignalDigitalOutput(int byteIndex, int bitIndex, PointDataTypeConstants dataType, string title)
            : base(byteIndex, bitIndex, dataType, title)
        {

        }

        public void Read()
        {
            Object tempData = null;

            ConnectProSim.Instance.Connection.ReadOutputPoint(ByteIndex, BitIndex, DataType, ref tempData);

            bool value = false;

            if (tempData is bool)
            {
                value = (bool)tempData;
            }

            if(System.Runtime.InteropServices.Marshal.IsComObject(tempData))
                System.Runtime.InteropServices.Marshal.ReleaseComObject(tempData);

            tempData = null;

            Data = BitConverter.GetBytes(value);
        }
    }

    public sealed class SignalCollection : ISignalCollection
    {
        public SignalCollection()
        {
            mSignalCollection = new List<ISignal>();
        }

        public ISignal this[int index]
        {
            get { return (ISignal)mSignalCollection[index]; }
            set { mSignalCollection[index] = value; }
        }

        public IEnumerator<ISignal> GetEnumerator()
        {
            return new SignalEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new SignalEnumerator(this);
        }

        public bool Contains(ISignal signal)
        {
            bool isContains = false;

            if (signal is ISignalDigital)
            {
                foreach (ISignal item in mSignalCollection)
                {
                    if (item is ISignalDigital && new SignalComparator().Equals((ISignalDigital) item, (ISignalDigital) signal))
                    {
                        isContains = true;
                        break;
                    }
                }
            }
            else if (signal is ISignalAnalog)
            {
                foreach (ISignal item in mSignalCollection)
                {
                    if(item is ISignalAnalog && new SignalComparator().Equals((ISignalAnalog) item, (ISignalAnalog) signal))
                    {
                        isContains = true;
                        break;
                    }
                }
            }
            
            return isContains;
        }

        public bool Contains(int byteIndex, int bitIndex)
        {
            bool isContains = false;

            foreach (ISignal item in mSignalCollection)
            {
                if (item is ISignalDigital && new SignalComparator().Equals((ISignalDigital)item, byteIndex, bitIndex))
                {
                    isContains = true;
                    break;
                }
            }

            return isContains;
        }

        public bool Contains(int byteIndex)
        {
            bool isContains = false;

            foreach (ISignal item in mSignalCollection)
            {
                if (item is ISignalAnalog && new SignalComparator().Equals((ISignalAnalog)item, byteIndex))
                {
                    isContains = true;
                    break;
                }
            }

            return isContains;
        }

        public void Add(ISignal item)
        {
            if (!Contains(item))
            {
                mSignalCollection.Add(item);
            }
        }

        public void Clear()
        {
            mSignalCollection.Clear();
        }

        public void CopyTo(ISignal[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException("The array can't be null!");
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException("The starting array index can't be negative!");
            if (Count > array.Length - arrayIndex + 1)
                throw new ArgumentException("The destination array has fewer elements than the collection!");

            for (int i = 0; i < mSignalCollection.Count; i++)
            {
                array[i + arrayIndex] = mSignalCollection[i];
            }
        }

        public int Count
        {
            get { return mSignalCollection.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(ISignal item)
        {
            bool isRemove = false;

            for (int i = 0; i < mSignalCollection.Count; i++)
            {
                ISignal currentItem = mSignalCollection[i];

                if (new SignalComparator().Equals(currentItem, item))
                {
                    mSignalCollection.RemoveAt(i);
                    isRemove = true;
                    break;
                }
            }

            return isRemove;
        }

        private List<ISignal> mSignalCollection;
    }

    public sealed class SignalEnumerator : IEnumerator<ISignal>
    {
        public SignalEnumerator(SignalCollection collection)
        {
            mSignalCollection = collection;
            mCurrentIndex = -1;
            mCurrentSignal = default(ISignal);
        }

        public bool MoveNext()
        {
            if (++mCurrentIndex >= mSignalCollection.Count())
            {
                return false;
            }
            else
            {
                mCurrentSignal = mSignalCollection[mCurrentIndex];
            }

            return true;
        }

        public void Reset()
        {
            mCurrentIndex = -1;
        }

        void IDisposable.Dispose()
        {

        }

        public ISignal Current
        {
            get { return mCurrentSignal; }
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }

        private SignalCollection mSignalCollection;
        private int mCurrentIndex;
        private ISignal mCurrentSignal;
    }

    public sealed class SignalComparator : EqualityComparer<ISignal>
    {
        public override bool Equals(ISignal x, ISignal y)
        {
            bool isEquals = false;

            if (x is ISignalDigital && y is ISignalDigital)
            {
                isEquals = Equals((ISignalDigital)x, (ISignalDigital)y);
            }
            else if (x is ISignalAnalog && y is ISignalAnalog)
            {
                isEquals = Equals((ISignalAnalog)x, (ISignalAnalog)y);
            }

            return isEquals;
        }

        public bool Equals(ISignalDigital x, ISignalDigital y)
        {
            bool isEquals = false;

            if (x.ByteIndex == y.ByteIndex && x.BitIndex == y.BitIndex)
            {
                isEquals = true;
            }

            return isEquals;
        }

        public bool Equals(ISignalAnalog x, ISignalAnalog y)
        {
            bool isEquals = false;

            if (x.ByteIndex == y.ByteIndex)
            {
                isEquals = true;
            }

            return isEquals;
        }

        public bool Equals(ISignalDigital signalDigital, int byteIndex, int bitIndex)
        {
            bool isEquals = false;

            if (signalDigital.ByteIndex == byteIndex && signalDigital.BitIndex == bitIndex)
            {
                isEquals = true;
            }

            return isEquals;
        }

        public bool Equals(ISignalAnalog signalAnalog, int byteIndex)
        {
            bool isEquals = false;

            if (signalAnalog.ByteIndex == byteIndex)
            {
                isEquals = true;
            }

            return isEquals;
        }

        public override int GetHashCode(ISignal item)
        {
            int hashCode = item.ByteIndex ^ item.Data.GetHashCode();

            return hashCode.GetHashCode();
        }
    }
}
