using System;

namespace lecture
{
	class MainClass
	{
		enum defaults {
			Length = 15,
			Maximum = 10,
			Minimum = 0
		};

		static int indexToValue( int index ) {
			switch (index) {
			case 0:
				return (int)defaults.Length;
			case 1:
				return (int)defaults.Maximum;
			case 2:
				return (int)defaults.Minimum;
			default:
				throw new ArgumentOutOfRangeException ("Unexpected argument index");
			}
		}

		static int argToValue(string[] args, int index) {
			if (index < args.Length)
				return Convert.ToInt32 (args [index]);

			return indexToValue( index );
		}

		static void randomizeArray( int[] array, int minimum, int maximum ) {
			Random	source = new Random();

			for (int i = 0; i < array.Length; i++)
				array [i] = source.Next (minimum, maximum);
		}

		static void outputArray(int[] array) {
			foreach (int value in array)
				Console.Write (value.ToString() + " ");

			Console.WriteLine ();
		}

		static void sortArray(int[] array) {
			int mindex,
				tmpval,
				i, j;

			for (i = 0; i < array.Length - 1; i++)
				for (j = i + 1; j < array.Length; j++)
					if( array[j] < array[i] ){
						tmpval = array [i];
						array [i] = array [j];
						array [j] = tmpval;
					}

			return;
		}

		public static void Main (string[] args)
		{
			int[]	array = new int[argToValue (args, 0 )];

			randomizeArray (array, argToValue (args, 2), argToValue (args, 1));
			outputArray (array);
			sortArray (array);
			outputArray (array);
		}
	}
}
