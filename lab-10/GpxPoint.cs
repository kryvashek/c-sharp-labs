using System;
using System.Xml.Linq;

namespace lab10 {
    public class GpxPoint {
        readonly XElement _entry;
        readonly DateTime _moment;
        readonly double _latitude, _longitude, _altitude, _hdop;
        readonly XNamespace _xmlns;
        double _velocity, _distance;

        public double Latitude { get { return _latitude; } }

        public double Longitude { get { return _longitude; } }

        public double Altitude { get { return _altitude; } }

        public double Hdop { get { return _hdop; } }

        public DateTime Moment { get { return _moment; } }

        public XElement Entry { get { return _entry; } }

        public double Distance {
            get { return _distance; }
            set {
                _distance = value;

                if( null == _entry.Element( _xmlns + "extensions" ) )
                    _entry.Add( new XElement( _xmlns + "extensions" ) );

                _entry.Element( _xmlns + "extensions" ).SetElementValue( _xmlns + "distance", value );
            }
        }

        public double Velocity {
            get { return _velocity; }
            set {
                _velocity = value;

                if( null == _entry.Element( _xmlns + "extensions" ) )
                    _entry.Add( new XElement( _xmlns + "extensions" ) );

                _entry.Element( _xmlns + "extensions" ).SetElementValue( _xmlns + "velocity", value );
            }
        }

        public GpxPoint Previous {
            set {
                Distance = CalcDistance( value, this );
                Velocity = CalcVelocity( value, this, Distance );
            }
        }

        public GpxPoint( double latitude, double longitude, DateTime moment, double altitude = .0, double hdop = .0 ) {
            _latitude = latitude;
            _longitude = longitude;
            _moment = moment;
            _altitude = altitude;
            _hdop = hdop;
            _xmlns = XNamespace.Get( @"http://www.topografix.com/GPX/1/1" );
            _velocity = .0;
            _distance = .0;
            _entry = new XElement( _xmlns + "trkpt" );
            _entry.SetAttributeValue( _xmlns + "lat", _latitude );
            _entry.SetAttributeValue( _xmlns + "lon", _longitude );
            _entry.SetElementValue( _xmlns + "ele", _altitude );
            _entry.SetElementValue( _xmlns + "hdop", _hdop );
            _entry.SetElementValue( _xmlns + "time", _moment.ToUniversalTime() );
        }

        private bool ParseDouble( string text, out double value ) {
            return double.TryParse( text.Replace( '.', ',' ), out value );
        }

        private double ReadDoubleAttr( XElement entry, XName name ) {
            XAttribute attr = entry.Attribute( name );
            double temp;

            if( null == attr )
                throw new ArgumentOutOfRangeException( "No '" + name + "' attribute passed to GpxPoint in XML entry:\n" + entry );

            if( !ParseDouble( attr.Value, out temp ) )
                throw new ArgumentOutOfRangeException( "Wrong value of '" + name + "' attribute ('" + attr.Value + "') passed to GpxPoint via XML." );
               
            return temp;
        }

        private double ReadDoubleField( XElement entry, XName name ) {
            XElement field = entry.Element( name );
            double temp;

            if( null != field && ParseDouble( field.Value, out temp ) )
                return temp;

            return .0;
        }

        public GpxPoint( XElement entry, XNamespace xmlns ) {
            _entry = entry;
            _xmlns = xmlns;
            _latitude = ReadDoubleAttr( _entry, "lat" );
            _longitude = ReadDoubleAttr( _entry, "lon" );
            _altitude = ReadDoubleField( _entry, _xmlns + "ele" );
            _hdop = ReadDoubleField( _entry, _xmlns + "hdop" );

            XElement timeEntry = _entry.Element( _xmlns + "time" );

            if( null == timeEntry )
                throw new ArgumentOutOfRangeException( "No 'time' element passed to GpxPoint in XML entry:\n" + entry );

            if( !DateTime.TryParse( timeEntry.Value, out _moment ) )
                throw new ArgumentOutOfRangeException( "Wrong value of 'time' element ('" + timeEntry.Value + "') passed to GpxPoint via XML." );
        }

        override public string ToString() {
            return _latitude + ":" + _longitude + ":" + _altitude + " on " + _moment;
        }

        public static double CalcDistance( GpxPoint previous, GpxPoint current ) {
            const double R = 6371e3; // Earth radius
            double  rLat1 = previous.Latitude * Math.PI / 180.0,
                    rLat2 = current.Latitude * Math.PI / 180.0,
                    drLon = ( current.Longitude - previous.Longitude ) * Math.PI / 180.0;

            double  a = Math.Sin( ( rLat2 - rLat1 ) / 2.0 ),
                    b = Math.Sin( drLon / 2.0 );

            double  hcls = a * a + Math.Cos( rLat1 ) * Math.Cos( rLat2 ) * b * b;

            return R * 2 * Math.Atan2( Math.Sqrt( hcls ), Math.Sqrt( 1 - hcls ) );
        }

        public static double CalcVelocity( GpxPoint previous, GpxPoint current, double distance = -1.0 ) {
            if( .0 > distance )
                distance = CalcDistance( previous, current );
            
            return distance / ( current.Moment - previous.Moment ).TotalSeconds;
        }
    }
}

