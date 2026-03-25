using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NelderMeadOptimization.Models
{
    internal class Point
    {
        public double[] Coordinates { get; }
        public int Dimension => Coordinates.Length; // размерность пространства
        private double value; // значение функции в точке
        public double Value
        {
            get => value;
            private set => this.value = value;
        }

        public Point(double[] coordinates, double value)
        {
            Coordinates = coordinates ?? throw new ArgumentNullException(nameof(coordinates));
            this.value = value;
        }
        // конструктор без значения функции (будет вычислена позже)
        public Point(double[] coordinates) : this(coordinates, 0) { }
        public double this[int index] => Coordinates[index]; // доступ к координате по индексу
        public void UpdateValue(double newValue)
        {
            Value = newValue;
        }
        // создание копии точки
        public Point Clone()
        {
            return new Point((double[])Coordinates.Clone(), Value);
        }
        public override string ToString()
        {
            string coords = string.Join("; ", Coordinates.Select(c => c.ToString("F6")));
            return $" ({coords})={Value}";
        }
    }
}
