using System;

namespace lab02 {
	public interface AbstractShape {
		// площадь данной фигуры
		double Area { get; }

		// периметр данной фигуры
		double Perimeter { get; }

		// центр масс данной фигуры (при равномерной плотности и одинаковой толщине)
		Point MassCenter { get; }

		// позиция данной фигуры (в большинстве случаев совпадает с центром масс)
        Point Position { get; set; }

        string Description { get; }
	}
}

