/*
 * 
 * Created by SharpDevelop.
 * User: Augusto Bornholdt
 * Date: 26/08/2013
 * 
 */
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace VSTiBox
{
	public class RangeControl : Control
	{
		
		private int 	_value;
		private int 	_maxValue;
	    private int 	_minValue;
	    private int 	_smallChange;
		private int 	_largeChange;
		
		public event EventHandler ValueChanged;
				
		
		public RangeControl()
		{
			base.SetStyle(ControlStyles.DoubleBuffer, true);
		    base.SetStyle(ControlStyles.UserMouse, true);
		    base.SetStyle(ControlStyles.Selectable, true);
		    
			_value = 0;
			_maxValue = 100;
			_minValue = 0;
			_smallChange = 1;
			_largeChange = 10;
		}
		
		
		public void SetRange(int minValue, int maxValue)
		{
			if ((_minValue != minValue) || (_maxValue != maxValue)){
			
				if (minValue > maxValue)
					maxValue = minValue;
				
				_minValue = minValue;
				_maxValue = maxValue;
				
				if(_value < _minValue)
					SetValue(_minValue);
				else if(_value > _maxValue)
					SetValue(_maxValue);
				
				OnRangeChanged(EventArgs.Empty);
			}
		}
		
		
		public int GetMaximum()
		{
			return _maxValue;
		}
		
		
		public void SetMaximum(int maxValue)
		{
			if(maxValue != _maxValue){
				if(maxValue < _minValue)
					_minValue = maxValue;
				_maxValue = maxValue;
				
				if(_maxValue < _value)
					SetValue(_maxValue);
				else
					base.Invalidate();
				
				OnRangeChanged(EventArgs.Empty);
			}
		}

		
		public int GetMinimum()
		{
			return _minValue;
		}
		
		
		public void SetMinimum(int minValue)
		{
			if(minValue != _minValue){
				if(minValue > _maxValue)
					_maxValue = minValue;
				
				_minValue = minValue;

				if(_minValue > _value)
					SetValue(_minValue);
				else
					base.Invalidate();
				
				OnRangeChanged(EventArgs.Empty);
			}
		}
		
		
		public int GetValue()
		{
			return _value;
		}

		
		public void SetValue(int value)
		{
			if(_value != value) {
				if(value < _minValue)
					_value = _minValue;
				else if(value > _maxValue)
					_value = _maxValue;
				else {
					_value = value;
				}
				OnValueChanged(EventArgs.Empty);
				base.Invalidate();	
			}
		}
		
		
		public int GetSmallChange()
		{
			return _smallChange;
		}

		
		public void SetSmallChange(int smallChange)
		{
			if (smallChange < 0)
				throw new ArgumentOutOfRangeException("smallChange");
			if(_smallChange != smallChange){
				_smallChange = smallChange;
				OnSmallChangeChanged(EventArgs.Empty);
			}
		}
		
		
		public virtual void OnSmallChangeChanged(EventArgs e)
		{
		}
		
		
		public int GetLargeChange()
		{
			return _largeChange;
		}

		
		public void SetLargeChange(int largeChange)
		{
			if (largeChange < 0)
				throw new ArgumentOutOfRangeException("largeChange");
			if(_largeChange != largeChange){
				_largeChange = largeChange;
				OnLargeChangeChanged(EventArgs.Empty);
			}
		}
		
		
		public virtual void OnLargeChangeChanged(EventArgs e)
		{
		}
		
		
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			switch (keyData) {
				case Keys.Add:
					SetValue(_value + _smallChange);
					return true;
				case Keys.Up:
				case Keys.Right:
					SetValue(_value + _largeChange);
					return true;
				case Keys.Subtract:
					SetValue(_value - _smallChange);
					return true;
				case Keys.Down:
				case Keys.Left:
					SetValue(_value - _largeChange);
					return true;
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}
		
	
		protected virtual void OnRangeChanged(EventArgs e)
		{

		}
				
		protected virtual void OnValueChanged(EventArgs e)
		{
			if(ValueChanged != null)
				ValueChanged(this, e);
		}
		
	
		protected override void WndProc(ref Message m)
		{
		    switch (m.Msg)
		    {
		        case 0x20a:
		    		int num = (int)((long) m.WParam) ;
		    		if(num > 0){
		    			OnMouseWheel(1);
		    		} else {
		    			OnMouseWheel(-1);
		    		}
		    		 break;
		        default:
		            base.WndProc(ref m);
		            break;
		    }
		}
		
		
		
		#region "Properties"
		
		
		[DefaultValueAttribute(100)]
		public int Maximum
		{
			get{return GetMaximum();}
			set{SetMaximum(value);}
		}
		
		[DefaultValueAttribute(0)]
		public int Minimum
		{
			get{return GetMinimum();}
			set{SetMinimum(value);}
		}
		
		
		[DefaultValueAttribute(0)]
		public int Value
		{
			get{return GetValue();}
			set{SetValue(value);}
		}
		
		[DefaultValue(10)]
		public int LargeChange
		{
			get{return GetLargeChange();}
			set{SetLargeChange(value);}
		}
		
		
		[DefaultValue(1)]
		public int SmallChange
		{
			get{return GetSmallChange();}
			set{SetSmallChange(value);}
		}
		

		#endregion
	
		
		#region "Mouse events"
		
		
		protected virtual void OnMouseWheel(int delta)
		{
		  	SetValue(_value + delta * _smallChange);
		}

		
		 #endregion
		
		
		
	}
}
