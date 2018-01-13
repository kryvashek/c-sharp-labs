using System;

namespace lab02 {
	public class Circle : Shape {
        double	_radius;

		public Circle ( Point center, double radius ) :
		    base( center ) {
			_radius = radius;
			_area = Math.PI * _radius * _radius;
			_perimeter = 2 * Math.PI * _radius;
            _kind = "Circle";
		}

		public double Radius {
			get {
				return _radius;
			}
		}
	}
}

