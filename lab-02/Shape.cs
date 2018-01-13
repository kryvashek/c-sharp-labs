using System;

namespace lab02 {
	public abstract class Shape : AbstractShape	{
        protected string    _kind;
        protected Point		_position,
                            _massCenter;
		protected double	_area,
							_perimeter;

		public Shape ( Point position ) {
			_massCenter = position;
			_position = position;
            _area = 0;
            _perimeter = 0;
            _kind = "Shape";
		}

		public Point Position {
			get {
				return _position;
			}

			set {
                _massCenter = _massCenter + value - _position;
				_position = value;
			}
		}

		public double Area {
			get {
				return _area;
			}
		}

		public double Perimeter {
			get {
				return _perimeter;
			}
		}

		public Point MassCenter {
			get {
				return _massCenter;
			}
		}

        public string Description {
            get {
                return _kind + " with position " + _position + ", mass center " + _massCenter + ", perimeter " + _perimeter + " and area " + _area;
            }
        }
	}
}

