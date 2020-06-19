﻿using SpiceSharp.Algebra;
using SpiceSharp.Behaviors;
using SpiceSharp.ParameterSets;
using SpiceSharp.Simulations;
using System.Numerics;

namespace SpiceSharp.Components.Diodes
{
    /// <summary>
    /// Small-signal behavior for <see cref="Diode"/>.
    /// </summary>
    /// <seealso cref="Dynamic"/>
    /// <seealso cref="IFrequencyBehavior"/>
    public class Frequency : Dynamic,
        IFrequencyBehavior
    {
        private readonly ElementSet<Complex> _elements;
        private readonly IComplexSimulationState _complex;

        /// <summary>
        /// The complex variables used by the behavior.
        /// </summary>
        protected readonly DiodeVariables<Complex> ComplexVariables;

        /// <include file='../../Common/docs.xml' path='docs/members[@name="frequency"]/Voltage/*'/>
        [ParameterName("v"), ParameterName("vd"), ParameterInfo("The complex voltage across the internal diode")]
        public Complex ComplexVoltage => (ComplexVariables.PosPrime.Value - ComplexVariables.Negative.Value) / Parameters.SeriesMultiplier;

        /// <include file='../../Common/docs.xml' path='docs/members[@name="frequency"]/Current/*'/>
        [ParameterName("i"), ParameterName("id"), ParameterName("c"), ParameterName("cd"), ParameterInfo("The complex current through the diode")]
        public Complex ComplexCurrent
        {
            get
            {
                var geq = LocalCapacitance * _complex.Laplace + LocalConductance;
                return ComplexVoltage * geq * Parameters.ParallelMultiplier;
            }
        }

        /// <include file='../../Common/docs.xml' path='docs/members[@name="frequency"]/Power/*'/>
        [ParameterName("p"), ParameterName("pd"), ParameterInfo("The complex power")]
        public Complex ComplexPower => ComplexVoltage * Complex.Conjugate(ComplexCurrent);

        /// <summary>
        /// Initializes a new instance of the <see cref="Frequency"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="context">The context.</param>
        public Frequency(string name, IComponentBindingContext context) : base(name, context)
        {
            _complex = context.GetState<IComplexSimulationState>();
            ComplexVariables = new DiodeVariables<Complex>(name, _complex, context);
            _elements = new ElementSet<Complex>(_complex.Solver,
                ComplexVariables.GetMatrixLocations(_complex.Map));
        }

        void IFrequencyBehavior.InitializeParameters()
        {
            CalculateCapacitance(LocalVoltage);
        }

        void IFrequencyBehavior.Load()
        {
            var state = _complex;

            var gspr = ModelTemperature.Conductance * Parameters.Area;
            var geq = LocalConductance;
            var xceq = LocalCapacitance * state.Laplace.Imaginary;

            // Load Y-matrix
            var m = Parameters.ParallelMultiplier;
            var n = Parameters.SeriesMultiplier;
            geq *= m / n;
            gspr *= m / n;
            xceq *= m / n;
            _elements.Add(
                gspr, new Complex(geq, xceq), new Complex(geq + gspr, xceq),
                -new Complex(geq, xceq), -new Complex(geq, xceq), -gspr, -gspr);
        }
    }
}
