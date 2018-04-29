using System;
using Gtk;

namespace lab02 {
    public interface AbstractShape : Pars.IParsable {
		// площадь данной фигуры
		double Area { get; }

		// периметр данной фигуры
		double Perimeter { get; }

		// центр масс данной фигуры (при равномерной плотности и одинаковой толщине)
		Vertex MassCenter { get; }

		// позиция данной фигуры (в большинстве случаев совпадает с центром масс)
        Vertex Position { get; set; }

        // минимальные значения координат среди всех точек фигуры (левый нижний угол)
        Vertex Min { get; }

        // максимальные значения координат среди всех точек фигуры (правый верхний угол)
        Vertex Max { get; }

        // человекочитаемое изложение параметров фигуры (центр масс, позиция, периметр и площадь)
        string Description { get; }

        // рисование фигуры с заданными параметрами (context) на указанном холсте (canvas)
        void Draw( Cairo.Context context );
	}
}

