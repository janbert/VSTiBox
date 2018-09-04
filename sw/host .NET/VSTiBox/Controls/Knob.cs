/*
 * 
 * Created by SharpDevelop.
 * User: Augusto Bornholdt
 * Date: 26/08/2013
 * 
 */
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace VSTiBox
{
	
	public class Knob : RangeControl
	{
		private int 	_markerWidth;
		private int		_maxAngle;
		private int		_knobRadius;
		private int 	_lastMousePos;
    	private bool 	_tickVisible;
    	private bool 	_isMouseDown;
    	private Color 	_knobColor;
    	private Color 	_tickColor;
    	private Color 	_markerColor;
		private Point 	_mousePos;
		private TextKnobRelation _textKnobRelation;
		private KnobStyle _knobBorderStyle;
    	
		private static float 	_tickFrecuency = 11.25f; //degrees
		private static int 	 	_p = 96;
		private static double 	_piDiv4 = Math.PI / 4;
	
		
		public Knob()
		{
			_markerWidth = 2;
			_maxAngle = 270;
			_tickVisible = true;
			_tickColor = Color.Black;
			_knobColor = Color.Black;
			_markerColor = Color.Black;
			_textKnobRelation = TextKnobRelation.TextAboveKnob;
			_knobBorderStyle = KnobStyle.RibbedBorder;
			_knobRadius = 12;
		}
		
		
		protected override Size DefaultSize {
			 get { return new Size(52, 62); }
		}
		
		
		public int GetMarkerWidth()
		{
			return _markerWidth;
		}

		
		public virtual void SetMarkerWidth(int markerWidth)
		{
			if(_markerWidth != markerWidth) {
				_markerWidth = markerWidth;
				base.Invalidate();
			}
		}

		
		protected int GetKnobRadius()
		{
			return _knobRadius;
		}
		
		
		protected void SetKnobRadius(int radius)
		{
			if(_knobRadius != radius){
				_knobRadius = radius;
				base.Invalidate();
			}
		}
		
		
		public KnobStyle GetKnobBorderStyle()
		{
			return _knobBorderStyle;
		}
		
		
		public void SetKnobBorderStyle(KnobStyle style)
		{
			if(_knobBorderStyle != style){
				_knobBorderStyle = style;
				base.Invalidate();
			}
		}
		
		
		public virtual Color GetKnobColor()
		{
			return _knobColor;
		}
	
		
		public void SetKnobColor(Color knobColor)
		{
			if(_knobColor != knobColor){
				_knobColor = knobColor;
				base.Invalidate();
			}
		}
		
		
		public void SetMaxAngle(int maxAngle)
		{
			if(_maxAngle != maxAngle){
				_maxAngle = maxAngle;
				base.Invalidate();
			}
		}
	
		
		public int GetMaxAngle()
		{
			return _maxAngle;
		}
		
		
		public TextKnobRelation GetTextKnobRelation()
		{
			return _textKnobRelation;
		}
		
		
		public  void SetTextKnobRelation(TextKnobRelation textKnobRelation)
		{
			if(textKnobRelation != _textKnobRelation){
				_textKnobRelation = textKnobRelation;
				base.Invalidate();
			}
		}
		
		
		public bool IsTickVisible()
		{
			return _tickVisible;
		}
		
		
		public virtual void SetTickVisible(bool visible)
		{
			if(_tickVisible != visible){
				_tickVisible = visible;
				base.Invalidate();
			}
		}
		
		
		public Color GetTickColor()
		{
			return _tickColor;
		}
			
			
		public void SetTickColor(Color color)
		{
			if(color != _tickColor){
				_tickColor = color;
				base.Invalidate();
			}
		}
		
		
		public Color GetMarkerColor()
		{
			return _markerColor;
		}
			
			
		public void SetMarkerColor(Color color)
		{
			if(color != _markerColor){
				_markerColor = color;
				base.Invalidate();
			}
		}
		
		
		public float GetAngle(int value)
		{
			float range = GetMaximum() - GetMinimum();
			return _maxAngle * ((value - GetMinimum()) / range);
			
		}
		
		
		public float GetAngle()
		{
			float range = GetMaximum() - GetMinimum();
			return _maxAngle * ((GetValue() - GetMinimum()) / range);
		}
		
		
		public Point GetKnobCenter()
		{
			Point result = new Point(Width / 2, Height / 2);
			if(Text.Length > 0){
				if(_textKnobRelation == TextKnobRelation.TextAboveKnob){
					result.Y = Math.Max(_knobRadius + FontHeight + 7, Height / 2);
				} else {
					result.Y = Math.Min(result.Y, (Height - FontHeight) / 2);
				}
			} 
			return result;
		}

		
		protected override void OnTextChanged(EventArgs e)
		{
			base.OnTextChanged(e);
			base.Invalidate();
		}

		
		
		private static Color ColorFromHSL(float H, float S, float L)
		{
			int Max, Mid = 0, Min;
			
			if (L <= 0.5){
				Min = (int)((L-(L*S))*255);
			}else{
				Min = (int)(((S*L)+L-S)*255);
			}
			Max = (int)(2*L*255) - Min;
			
			if ( H >= 0 && H <= 60 ){
				Mid = (int)((H/60)*(Max-Min))+Min;
				return Color.FromArgb(Max, Mid, Min);
			}else if ( H <= 120 ){
				Mid = (int)(-1*(((H-120)/60)*(Max-Min))+Min);
				return Color.FromArgb(Mid, Max, Min);
			}else if ( H <= 180 ){
				Mid = (int)((((H-120)/60)*(Max-Min))+Min);
				return Color.FromArgb(Min, Max, Mid);
			}else if ( H <= 240 ){
				Mid = (int)(-1*(((H-240)/60)*(Max-Min))+Min);
				return Color.FromArgb(Min, Mid, Max);
			}else if ( H <= 300 ){
				Mid = (int)((((H-240)/60)*(Max-Min))+Min);
				return Color.FromArgb(Mid, Min, Max);
			}else if ( H <= 360 ){
				Mid = (int)(-1*(((H-360)/60)*(Max-Min))+Min);
				return Color.FromArgb(Max, Min, Mid);
			}else{
				return Color.FromArgb(0, 0, 0);
			}
		}
		
		
		public static float DegreeToRadians(float degrees)
		{
			return 0.017453f * degrees;  //PI / 180.0F * degrees
		}
		
		
		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			Brush backBrush = new SolidBrush(BackColor);
			g.SmoothingMode = SmoothingMode.AntiAlias;
		
			float angle;
			if(_maxAngle > 360)
				angle =  270 - GetAngle();
			else
				angle =  (90 + _maxAngle * 0.5f) - GetAngle();
			
			float radAngle = DegreeToRadians(angle);
			float outerRadius = _knobRadius;
			float w = outerRadius / 16.0f;
			float knobRadius = outerRadius -  w * 2 - 1.5f;
			float s = (float)(Math.PI * 2) / _p;
			outerRadius -= w;
			Point center = GetKnobCenter();
			
			if(_knobBorderStyle == KnobStyle.RibbedBorder || _knobBorderStyle == KnobStyle.RibbedBorder3D){
				
				PointF[] pts1 = new PointF[_p];
				PointF[] pts2 = new PointF[_p];
				for (int i = 0; i < _p; i++ ) {
					double k = Math.Cos(i * _piDiv4) * w + outerRadius;
					pts1[i].X = (float)(center.X + Math.Cos(radAngle) * k);
					pts1[i].Y = (float)(center.Y + -Math.Sin(radAngle) * k);
					pts2[i].X = (float)(center.X + Math.Cos(radAngle) * knobRadius);
					pts2[i].Y = (float)(center.Y + -Math.Sin(radAngle) * knobRadius);
					radAngle += s;
				}
				
				Brush knobBrush = new SolidBrush(_knobColor);
				GraphicsPath path = new GraphicsPath();
				path.AddPolygon(pts1);
				
			
				if(_knobBorderStyle == KnobStyle.RibbedBorder3D){
					g.FillPath (knobBrush, path);
					RectangleF rect = new RectangleF(center.X - knobRadius, center.Y - knobRadius, knobRadius * 2, knobRadius * 2);
					Brush centerBrush = new LinearGradientBrush(rect, Color.WhiteSmoke, ColorFromHSL(_knobColor.GetHue(), _knobColor.GetSaturation(), 0.18f), 45);
					g.FillEllipse(centerBrush, center.X - knobRadius, center.Y - knobRadius, knobRadius * 2, knobRadius * 2);
					centerBrush.Dispose();
				} else {
					path.AddPolygon(pts2);
					g.FillPath (knobBrush, path);
				}
				knobBrush.Dispose();
				
			} else if (_knobBorderStyle == KnobStyle.FlatBorder || _knobBorderStyle == KnobStyle.FlatBorder3D) {
				Pen knobPen = new Pen(_knobColor, MarkerWidth);
				g.DrawEllipse(knobPen, center.X - knobRadius, center.Y - knobRadius, knobRadius * 2, knobRadius * 2);
				if(_knobBorderStyle == KnobStyle.FlatBorder3D){
					RectangleF rect = new RectangleF(center.X - knobRadius, center.Y - knobRadius, knobRadius * 2, knobRadius * 2);
					Brush centerBrush = new LinearGradientBrush(rect, Color.WhiteSmoke, ColorFromHSL(_knobColor.GetHue(), _knobColor.GetSaturation(), 0.18f), 45);
					g.FillEllipse(centerBrush, center.X - knobRadius, center.Y - knobRadius, knobRadius * 2, knobRadius * 2);
					centerBrush.Dispose();
				}
				knobPen.Dispose();
			}
			
			PointF pt1 = new PointF();
			PointF pt2 = new PointF();
			Pen markerPen = new Pen(GetMarkerColor(), _markerWidth);
			pt1.X = (float)(center.X + Math.Cos(radAngle) * (knobRadius - 2));
			pt1.Y = (float)(center.Y + -Math.Sin(radAngle) * (knobRadius - 2));
			pt2.X = (float)(center.X + Math.Cos(radAngle) * 2);
			pt2.Y = (float)(center.Y + -Math.Sin(radAngle) * 2);
			
			g.DrawLine(markerPen, pt2, pt1);
			//g.DrawEllipse(markerPen, pt1.X, pt1.Y, 1, 1);
			
			if(_tickVisible){
				Pen tickPen = new Pen(_tickColor);
				outerRadius += 6 + w;
				angle = ((180 -_maxAngle) * 0.5f);
				radAngle = DegreeToRadians(angle);
				s = DegreeToRadians(_tickFrecuency);
				for (int i = 0; i <= _maxAngle / _tickFrecuency ; i++ ) {
					pt1.X = (float)(center.X + Math.Cos(radAngle) * (outerRadius));
					pt1.Y = (float)(center.Y + -Math.Sin(radAngle) * (outerRadius));
					if(i % 4 != 0){
						pt2.X = (float)(center.X + Math.Cos(radAngle) * (outerRadius - 2));
						pt2.Y = (float)(center.Y + -Math.Sin(radAngle) * (outerRadius - 2));
						g.DrawLine(tickPen, pt1, pt2);
					} else {
						pt2.X = (float)(center.X + Math.Cos(radAngle) * (outerRadius - 4));
						pt2.Y = (float)(center.Y + -Math.Sin(radAngle) * (outerRadius - 4));
						g.DrawLine(tickPen, pt1, pt2);
					}
					radAngle += s;
				}
				tickPen.Dispose();
			}
			
			if(Text.Length > 0){
				Rectangle TextBounds = new Rectangle(0, (int)(center.Y - outerRadius - FontHeight), Width, FontHeight + 2);
				switch (_textKnobRelation) {
					case TextKnobRelation.TextAboveKnob:
						TextRenderer.DrawText(g, Text, Font,TextBounds, ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
						break;
					case TextKnobRelation.KnobAboveText :
						TextBounds.Y = (int)(center.Y + outerRadius);
						TextRenderer.DrawText(g, Text, Font,TextBounds, ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
						break;
					case TextKnobRelation.TextTopCenter :
						TextBounds.Y = 0;
						TextRenderer.DrawText(g, Text, Font,TextBounds, ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
						break;
					case TextKnobRelation.TextBottomCenter :
						TextBounds.Y = Height - FontHeight - 2;
						TextRenderer.DrawText(g, Text, Font,TextBounds, ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
						break;
				}
			}
			
			backBrush.Dispose();
			markerPen.Dispose();
			base.OnPaint(e);
		}
	
		
		#region "Mouse events"
			
		
		protected override void OnMouseDown(MouseEventArgs e)
		{
			_isMouseDown = true;
			_mousePos = Cursor.Position;
			_lastMousePos = e.Y;
			Invalidate();
			Cursor.Hide();
			base.OnMouseDown(e);
		}
		
		protected override void OnMouseUp(MouseEventArgs e)
		{
			_isMouseDown = false;
			Cursor.Position = _mousePos;
			Invalidate();
			Cursor.Show();
			base.OnMouseUp(e);
		}
		
		protected override void OnMouseMove(MouseEventArgs e)
		{
			if(_isMouseDown){
				int num = _lastMousePos - e.Y;
				_lastMousePos = e.Y;
				SetValue(GetValue() + num * GetSmallChange());
			}
			base.OnMouseMove(e);
		}
		
		
		#endregion
		
		
		#region "Properties"
		
		[DefaultValueAttribute(270)]
		public int MaxAngle
		{
			get{return GetMaxAngle();}
			set{SetMaxAngle(value);}
		}
		
		[DefaultValueAttribute(TextKnobRelation.TextAboveKnob)]
		public TextKnobRelation TextKnobRelation
		{
			get{return GetTextKnobRelation();}
			set{SetTextKnobRelation(value);}
		}
		
		
		[DefaultValueAttribute(2)]
		public int MarkerWidth
		{
			get{return GetMarkerWidth();}
			set{SetMarkerWidth(value);}
		}
		
		public Color KnobColor
		{
			get{return GetKnobColor();}
			set{SetKnobColor(value);}
		}
		
		
		[DefaultValueAttribute(KnobStyle.RibbedBorder)]
		public KnobStyle KnobBorderStyle
		{
			get{return GetKnobBorderStyle();}
			set{SetKnobBorderStyle(value);}
		}
		
		
		[DefaultValueAttribute(true)]
		public bool TickVisible
		{
			get{return IsTickVisible();}
			set{SetTickVisible(value);}
		}
		
		public Color TickColor
		{
			get{return GetTickColor();}
			set{SetTickColor(value);}
		}
		
		
		public Color MarkerColor
		{
			get{return GetMarkerColor();}
			set{SetMarkerColor(value);}
		}
		
		
		[DefaultValueAttribute(12)]
		public int KnobRadius
		{
			get { return GetKnobRadius(); }
			set { SetKnobRadius(value); }
		}
		
		
		#endregion

		
	}
	
	
	
	public enum TextKnobRelation : short
	{
		TextAboveKnob,
		KnobAboveText,
		TextTopCenter,
		TextBottomCenter,
	}
	
	
	public enum KnobStyle : short
	{
		FlatBorder,
		RibbedBorder,
		Transparent,
		FlatBorder3D,
		RibbedBorder3D
	}
}
