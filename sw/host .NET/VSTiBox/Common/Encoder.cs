using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSTiBox
{
    public class Encoder
    {
        public event DeltaChangedDelegate DeltaChanged;

        public Encoder()
        {
            Button = new Button();
        }

        public Button Button
        {
            get;
            private set;
        }

        private Int16 mDelta;
        public Int16 Delta
        {
            get
            {
                return mDelta;
            }
            set
            {
                bool changed = (mDelta != value);
                mDelta = value;
                if (changed && DeltaChanged != null)
                {
                    DeltaChanged(this, mDelta);
                }
            }
        }
    }
}
