using System;
using Gtk;
using System.Text;
using System.IO;
using System.Collections.Generic;
using lab02;
using Pars;

namespace lab07 {
    public partial class MainWindow : Gtk.Window {
        uint _statusBarContext;
        List<AbstractShape> _shapes;
        ParsGetter _parsGetter;
        Vertex _min, _max, _logicCanvasSize, _scale, _offset, _lineWidthGap, _currentAreaSize;
        double _lineWidth, _prescale, _actualScale;
        Cairo.Surface _surface;

        public MainWindow() : base( Gtk.WindowType.Toplevel ) {
            Build();
            // настройка элементов интерфейса
            _statusBarContext = statusBar.GetContextId( "statusbar - ctx" );
            viewLogText.Editable = false;
            drawingArea.AddEvents( ( int )Gdk.EventMask.PointerMotionMask );

            // параметры "реального" холста
            drawingArea.ModifyBg( StateType.Normal, new Gdk.Color( 0xff, 0xff, 0xff ) );
            drawingArea.ModifyBase( StateType.Normal, new Gdk.Color( 0x00, 0x00, 0x00 ) );

            // параметры виртуального холста
            _prescale = 20.0;
            _lineWidth = 1.0;
            _lineWidthGap = new Vertex( _lineWidth, _lineWidth );
            _surface = null;
            _shapes = null;

            // настройка парсера
            initParser();

            log( "начало работы программы" );
        }

        protected void initParser() {
            _parsGetter = new ParsGetter();

            _parsGetter.Add( "L", delegate ( ParsGetter.ArgumentsList argList ) {
                var resultList = new List<AbstractShape>( argList.Count );

                foreach( var argument in argList.Values )
                    resultList.Add( argument as AbstractShape );

                return resultList;

            } ).Add( "V", delegate ( ParsGetter.ArgumentsList argList ) {
                return new Vertex( ( double )argList[ "x" ], ( double )argList[ "y" ] );

            } ).Add( "C", delegate ( ParsGetter.ArgumentsList argList ) {
                return new Circle( ( Vertex )argList[ "c" ], ( double )argList[ "r" ] );

            } ).Add( "E", delegate ( ParsGetter.ArgumentsList argList ) {
                return new Ellipse( ( Vertex )argList[ "f1" ], ( Vertex )argList[ "f2" ], ( double )argList[ "ma" ] );

            } ).Add( "P", delegate ( ParsGetter.ArgumentsList argList ) {
                var points = new List<Vertex>( argList.Count );

                foreach( var point in argList.Values )
                    points.Add( ( Vertex )point );

                return new Polygon( points );
            } );
        }

        protected void drawFrame( int indent, int gauge ) {
            Gdk.Rectangle rect;

            for( int gap = indent; gap < indent + gauge; gap++ ) {
                rect = new Gdk.Rectangle( gap, gap, ( int )_currentAreaSize.X - 2 * gap - 1, ( int )_currentAreaSize.Y - 2 * gap - 1 );
                drawingArea.GdkWindow.DrawRectangle( drawingArea.Style.BaseGC( StateType.Normal ), false, rect );
            }
        }

        protected void placeShapes() {
            if( _surface != null ) {
                drawingArea.GdkWindow.Clear();
                _surface.Dispose();
                _surface = null;
            }

            _min = Vertex.CreateMax();
            _max = Vertex.CreateMin();

            foreach( AbstractShape shape in _shapes ) {
                _min.Minimize( shape.Min );
                _max.Maximize( shape.Max );
                log( "фигура {0} расположена на позиции от {1} до {2}", shape, shape.Min.Literal, shape.Max.Literal );
            }

            _min -= _lineWidthGap;
            _max += _lineWidthGap;
            _logicCanvasSize = _max - _min;
            log( "размер логического холста (с учётом толщины линий) - {0}", _logicCanvasSize.Literal );
            _logicCanvasSize *= _prescale;
            _surface = new Cairo.ImageSurface( Cairo.Format.Argb32, ( int )_logicCanvasSize.X, ( int )_logicCanvasSize.Y );

            using( var surfaceContext = new Cairo.Context( _surface ) ) {
                surfaceContext.SetSourceRGB( 1.0, 1.0, 1.0 );
                surfaceContext.Paint();
                surfaceContext.LineWidth = 1.0;
                surfaceContext.SetSourceRGB( .0, .0, .0 );
                surfaceContext.Scale( _prescale, -_prescale );
                surfaceContext.Translate( -_min.X, -_max.Y );

                if( null != _shapes && 0 < _shapes.Count )
                    foreach( AbstractShape shape in _shapes ) {
                        shape.Draw( surfaceContext );
                        surfaceContext.Stroke();
                    }
            }
        }

        protected void drawShapes() {
            if( _surface != null )
                using( var areaContext = Gdk.CairoHelper.Create( drawingArea.GdkWindow ) ) {
                    _scale = _currentAreaSize / _logicCanvasSize;
                    _actualScale = Math.Min( _scale.X, _scale.Y );
                    areaContext.Scale( _actualScale, _actualScale );
                    _offset = ( _currentAreaSize / _actualScale - _logicCanvasSize ) / 2;
                    areaContext.SetSourceSurface( _surface, ( int )Math.Round( _offset.X ), ( int )Math.Round( _offset.Y ) );
                    areaContext.Paint();
                }
            
            drawFrame( 0, 1 );
        }

        protected delegate void DangerousAction();

        protected void invokeDangerous( string description, DangerousAction action ) {
            try {
                action();
                log( "успех операции '{0}'", description );
            } catch( Exception exc ) {
                log( "ошибка операции '{0}': {1}", description, exc );
            }
        }

        protected void saveTextFile( string filename, string text, bool append ) {
            using( var file = new FileStream( filename,
                                             ( append ? FileMode.Append : FileMode.Create ),
                                             FileAccess.Write,
                                             FileShare.Read ) ) {
                byte[] data = Encoding.UTF8.GetBytes( text );

                invokeDangerous( String.Format( "запись {0}", filename ), delegate {
                    file.Write( data, 0, data.Length );
                } );
            }
        }

        protected string logData() {
            TextIter start = viewLogText.Buffer.StartIter,
                     finish = viewLogText.Buffer.EndIter;

            return viewLogText.Buffer.GetText( start, finish, false );
        }

        protected void appendLog() {
            saveTextFile( "lab-07.log", logData(), true );
        }

        protected void saveLog( string filename ) {
            saveTextFile( filename, logData(), false );
        }

        protected void saveScheme( string filename ) {
            using( var parsAdder = new ParsAddder( "L" ) )
                saveTextFile( filename, parsAdder.Add( "s", _shapes ).Finish(), false );
        }

        protected void loadScheme( string filename ) {
            string text = null;

            invokeDangerous( String.Format( "чтение {0}", filename ), delegate {
                text = File.ReadAllText( filename, Encoding.UTF8 );
            } );

            if( null == text )
                return;

            bool shapesListChanged = false;

            invokeDangerous( String.Format( "парсинг {0}", filename ), delegate {
                object parsResult;

                if( !_parsGetter.Parse( text, out parsResult ) ||
                    null == parsResult as List<AbstractShape> )
                    throw new InvalidDataException( "текст не соответствует ожидаемому формату" );
                
                _shapes = parsResult as List<AbstractShape>;
                shapesListChanged = true;
            } );

            if( shapesListChanged ) {
                placeShapes();
                drawShapes();
            }
        }

        protected void setStatus( string format, params object[] args ) {
            statusBar.Push( _statusBarContext, String.Format( format, args ) );
        }

        protected void log( string format, params object[] args ) {
            string message = String.Format( format, args );
            setStatus( message );
            viewLogText.Buffer.InsertAtCursor( String.Format( "{0}: {1}{2}", DateTime.Now, message, Environment.NewLine ) );
        }

        protected delegate void WritingDataToFile( string filename );

        protected void saveFileBrief( string extension, WritingDataToFile writeAction ) {
            FileChooserDialog dialog = new FileChooserDialog( extension, "Сохранить как", this,
                                               FileChooserAction.Save,
                                               Stock.Cancel, ResponseType.Cancel,
                                               Stock.Ok, ResponseType.Ok );

            switch( dialog.Run() ) {
                case ( int )ResponseType.Cancel:
                    log( "отмена сохранения" );
                    break;

                case ( int )ResponseType.Ok:
                    var builder = new StringBuilder( dialog.Filename );

                    if( !dialog.Filename.EndsWith( extension, StringComparison.OrdinalIgnoreCase ) )
                        builder.Append( extension );

                    writeAction( builder.ToString() );
                    break;
            }
            dialog.Destroy();
        }

        #region effects

        protected void effectExpose( object o, ExposeEventArgs args ) {
            int width, height;
            drawingArea.GdkWindow.GetSize( out width, out height );
            _currentAreaSize.X = width;
            _currentAreaSize.Y = height;
            drawShapes();
            setStatus( "Размер полотна в пикселях: {0} в ширину на {1} в высоту", width, height );
        }

        protected void effectDelete( object sender, DeleteEventArgs a ) {
            a.RetVal = true;
            effectQuit( sender, a );
        }

        protected void effectMotion( object o, MotionNotifyEventArgs args ) {
            setStatus( "Позиция указателя на полотне (в пикселях): {0}; {1}", args.Event.X, args.Event.Y );
        }

        protected void effectLoadScheme( object sender, EventArgs e ) {
            FileChooserDialog dialog = new FileChooserDialog( ".sts", "Загрузка", this,
                                               FileChooserAction.Open,
                                               Stock.Cancel, ResponseType.Cancel,
                                               Stock.Ok, ResponseType.Ok );
            switch( dialog.Run() ) {
                case ( int )ResponseType.Cancel:
                    log( "отмена загрузки" );
                    break;
                case ( int )ResponseType.Ok:
                    loadScheme( dialog.Filename );
                    break;
            }

            dialog.Destroy();
        }

        protected void effectSaveScheme( object sender, EventArgs e ) {
            if( null != _shapes )
                saveFileBrief( ".sts", saveScheme );
        }

        protected void effectSavePic( object sender, EventArgs e ) {
            saveFileBrief( ".png", delegate ( string filename ) {
                invokeDangerous( String.Format( "запись {0}", filename ), delegate {
                    _surface.WriteToPng( filename );
                } );
            } );
        }

        protected void effectSaveLog( object sender, EventArgs e ) {
            saveFileBrief( ".log", saveLog );
        }

        protected void effectQuit( object sender, EventArgs e ) {
            log( "завершение работы программы" );
            appendLog();
            Application.Quit();
        }

        #endregion
    }

}
