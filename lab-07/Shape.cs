using System;

namespace lab02 {
	public abstract class Shape : AbstractShape	{
        protected string    _kind;
        protected Vertex	_position,
                            _massCenter;
		protected double	_area,
							_perimeter;

		public Shape ( Vertex position ) {
			_massCenter = position;
			_position = position;
            _area = 0;
            _perimeter = 0;
            _kind = "Shape";
		}

		public Vertex Position {
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

		public Vertex MassCenter {
			get {
				return _massCenter;
			}
		}

        public abstract Vertex Min { get; }

        public abstract Vertex Max { get; }

        public string Description {
            get {
                return _kind + " with position " + _position + ", mass center " + _massCenter + ", perimeter " + _perimeter + " and area " + _area;
            }
        }

        public abstract string ParsValue();

        public virtual void Draw( Cairo.Context context ) {
            context.MoveTo( _position.X, _position.Y );
            context.ClosePath();
        }

        public override string ToString() {
            return _kind;
        }
	}
}

