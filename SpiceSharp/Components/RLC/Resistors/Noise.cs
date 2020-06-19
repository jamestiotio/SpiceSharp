﻿using SpiceSharp.Behaviors;
using SpiceSharp.Components.NoiseSources;
using SpiceSharp.ParameterSets;
using SpiceSharp.Simulations;

namespace SpiceSharp.Components.Resistors
{
    /// <summary>
    /// Noise behavior for a <see cref="Resistor"/>.
    /// </summary>
    /// <seealso cref="Frequency"/>
    /// <seealso cref="INoiseBehavior"/>
    public class Noise : Frequency, INoiseBehavior
    {
        private readonly NoiseThermal _thermal;

        /// <inheritdoc/>
        public double OutputNoiseDensity => _thermal.OutputNoiseDensity;

        /// <inheritdoc/>
        public double TotalOutputNoise => _thermal.TotalOutputNoise;

        /// <inheritdoc/>
        public double TotalInputNoise => _thermal.TotalInputNoise;

        /// <summary>
        /// Gets the thermal noise source of the resistor.
        /// </summary>
        /// <value>
        /// The thermal noise source.
        /// </value>
        [ParameterName("thermal"), ParameterInfo("The thermal noise source")]
        public INoiseSource Thermal => _thermal;

        /// <summary>
        /// Initializes a new instance of the <see cref="Noise"/> class.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="context">The binding context.</param>
        public Noise(string name, IComponentBindingContext context) : base(name, context)
        {
            var state = context.GetState<IComplexSimulationState>();
            _thermal = new NoiseThermal("r",
                state.GetSharedVariable(context.Nodes[0]),
                state.GetSharedVariable(context.Nodes[1]));
        }

        /// <inheritdoc/>
        void INoiseSource.Initialize()
        {
            _thermal.Initialize();
        }

        /// <inheritdoc/>
        void INoiseBehavior.Compute()
        {
            _thermal.Compute(Conductance, Parameters.Temperature);
        }
    }
}
