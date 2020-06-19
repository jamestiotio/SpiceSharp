﻿using SpiceSharp.Diagnostics;
using SpiceSharp.ParameterSets;
using SpiceSharp.Simulations;
using System;

namespace SpiceSharp.Entities
{
    /// <summary>
    /// Base class for any circuit object that can take part in simulations.
    /// </summary>
    public abstract class Entity : ParameterSetCollection, IEntity
    {
        /// <summary>
        /// Gets or sets a value indicating whether the parameters should reference that of the entity.
        /// If the parameters are not referenced, then the parameters are cloned instead.
        /// </summary>
        /// <value>
        ///   <c>true</c> if parameters are referenced instead of cloned; otherwise, <c>false</c>.
        /// </value>
        public bool LinkParameters { get; set; } = true;

        /// <inheritdoc/>
        public string Name { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Entity"/> class.
        /// </summary>
        /// <param name="name">The name of the entity.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is <c>null</c>.</exception>
        protected Entity(string name)
        {
            Name = name.ThrowIfNull(nameof(name));
        }

        /// <inheritdoc/>
        public abstract void CreateBehaviors(ISimulation simulation);

        /// <summary>
        /// Sets the value of a parameter of the specified type and with the specified name. This is just a wrapper
        /// that allows chaining these commands.
        /// </summary>
        /// <typeparam name="P">The parameter value type.</typeparam>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="value">The value of the parameter.</param>
        /// <returns>
        /// The entity.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is <c>null</c>.</exception>
        /// <exception cref="ParameterNotFoundException">Thrown if the parameter was not found.</exception>
        public new Entity SetParameter<P>(string name, P value)
        {
            base.SetParameter(name, value);
            return this;
        }

        /// <inheritdoc/>
        protected override ICloneable Clone()
        {
            var clone = (Entity)Activator.CreateInstance(GetType(), Name);
            clone.CopyFrom(this);
            return clone;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return "{0} ({1})".FormatString(Name, GetType().Name);
        }
    }
}
