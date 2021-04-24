﻿using SpiceSharp.Attributes;
using SpiceSharp.Components.Switches;
using SpiceSharp.Entities;
using SpiceSharp.ParameterSets;

namespace SpiceSharp.Components
{
    /// <summary>
    /// A model for a <see cref="CurrentSwitch"/>
    /// </summary>
    [AutoGeneratedBehaviors]
    public partial class CurrentSwitchModel : Entity<CurrentModelParameters>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CurrentSwitchModel"/> class.
        /// </summary>
        /// <param name="name">The name of the model.</param>
        public CurrentSwitchModel(string name)
            : base(name)
        {
        }
    }
}
