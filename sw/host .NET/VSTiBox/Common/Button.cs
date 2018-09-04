using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSTiBox
{
    public delegate void ButtonDelegate(Button btn);
    
    public class Button
    {      
        public event ButtonDelegate Pressed;
        public event ButtonDelegate Released;

        public bool IsPressed { get; private set; }

        public Button()
        {
            IsPressed = false;
        }

        public void Set(Boolean value)
        {
            if (IsPressed != value)
            {
                IsPressed = value;
                if (value)
                {
                    if (Pressed != null)
                    {
                        Pressed(this);
                    }
                }
                else
                {
                    if (Released != null)
                    {
                        Released(this);
                    }
                }
            }
        }
    }
}
