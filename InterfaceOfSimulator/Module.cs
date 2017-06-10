using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InterfaceOfSimulator
{
    public class Module : IModule
    {
        public Module()
        {
            mSignalCollection = new SignalCollection();
        }

        public virtual void Add(ISignal signal)
        {
            mSignalCollection.Add(signal);
        }

        public virtual ISignal Get(int byteIndex, int bitIndex)
        {
            ISignal buffer = null;

            foreach (ISignal item in mSignalCollection)
            {
                if (item is ISignalDigital && item.ByteIndex == byteIndex)
                {
                    if (((ISignalDigital)item).BitIndex == bitIndex)
                    {
                        buffer = item;
                    }
                }
            }

            return buffer;
        }

        public virtual ISignal Get(int byteIndex)
        {
            ISignal buffer = null;

            foreach (ISignal item in mSignalCollection)
            {
                if (item is ISignalAnalog && item.ByteIndex == byteIndex)
                {
                    buffer = item;
                }
            }

            return buffer;
        }

        public virtual bool Contains(int byteIndex, int bitIndex)
        {
            bool isContains = false;

            if(mSignalCollection.Contains(byteIndex, byteIndex))
            {
                isContains = true;
            }

            return isContains;
        }

        public virtual bool Contains(int byteIndex)
        {
            bool isContains = false;

            if (mSignalCollection.Contains(byteIndex))
            {
                isContains = true;
            }

            return isContains;
        }

        public virtual ISignalCollection GetSignalCollection()
        {
            return mSignalCollection;
        }
        
        private ISignalCollection mSignalCollection;
    }

    public sealed class ModuleInput: Module, IModuleInput
    {
        public ModuleInput() : base()
        {
            
        }

        public override void Add(ISignal item)
        {
            if (item is ISignalInput)
            {
                base.Add(item);
            }
        }

        public void Add(ISignalInput item)
        {
            base.Add(item);
        }

        public void Generate()
        {
            foreach (ISignalInput item in base.GetSignalCollection())
            {
                item.Generate();
            }
        }

        public void Send()
        {
            foreach (ISignalInput item in base.GetSignalCollection())
            {
                item.Send();
            }
        }
    }

    public sealed class ModuleOutput : Module, IModuleOutput
    {
        public ModuleOutput() : base()
        {
        }

        public override void Add(ISignal item)
        {
            if (item is ISignalOutput)
            {
                base.Add(item);
            }
        }

        public void Add(ISignalOutput item)
        {
            base.Add(item);
        }

        public void Read()
        {
            foreach (ISignalOutput item in base.GetSignalCollection())
            {
                item.Read();
            }
        }
    }

    public sealed class ModuleCollection : IModuleCollection
    {
        public ModuleCollection()
        {
            mModuleCollection = new List<IModule>();
        }

        public IModule this[int index]
        {
            get { return (IModule) mModuleCollection[index]; }
            set { mModuleCollection[index] = value; }
        }

        public IEnumerator<IModule> GetEnumerator()
        {
            return new ModuleEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new ModuleEnumerator(this);
        }

        public bool Contains(IModule signal)
        {
            bool isContains = false;

            //-- описать алгоритм поиска одинаковых модулей
            
            return isContains;
        }

        public void Add(IModule item)
        {
            if (!Contains(item))
            {
                mModuleCollection.Add(item);
            }
        }

        public void Clear()
        {
            mModuleCollection.Clear();
        }

        public void CopyTo(IModule[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException("The array can't be null!");
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException("The starting array index can't be negative!");
            if (Count > array.Length - arrayIndex + 1)
                throw new ArgumentException("The destination array has fewer elements than the collection!");

            for (int i = 0; i < mModuleCollection.Count; i++)
            {
                array[i + arrayIndex] = mModuleCollection[i];
            }
        }

        public int Count
        {
            get { return mModuleCollection.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(IModule item)
        {
            bool isRemove = false;

            for (int i = 0; i < mModuleCollection.Count; i++)
            {
                IModule currentItem = mModuleCollection[i];

                if (new ModuleComparator().Equals(currentItem, item))
                {
                    mModuleCollection.RemoveAt(i);
                    isRemove = true;
                    break;
                }
            }

            return isRemove;
        }

        private List<IModule> mModuleCollection;
    }

    public sealed class ModuleEnumerator : IEnumerator<IModule>
    {
        public ModuleEnumerator(ModuleCollection collection)
        {
            mModuleCollection = collection;
            mCurrentIndex = -1;
            mCurrentModule = default(IModule);
        }

        public bool MoveNext()
        {
            if (++mCurrentIndex >= mModuleCollection.Count())
            {
                return false;
            }
            else
            {
                mCurrentModule = mModuleCollection[mCurrentIndex];
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

        public IModule Current
        {
            get { return mCurrentModule; }
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }

        private ModuleCollection mModuleCollection;
        private int mCurrentIndex;
        private IModule mCurrentModule;
    }

    public sealed class ModuleComparator : EqualityComparer<IModule>
    {
        public override bool Equals(IModule x, IModule y)
        {
            bool isEquals = false;

            //--- сравнение на идентичность

            return isEquals;
        }

        public override int GetHashCode(IModule item)
        {
            int hashCode = item.GetHashCode();

            return hashCode.GetHashCode();
        }
    }
}
