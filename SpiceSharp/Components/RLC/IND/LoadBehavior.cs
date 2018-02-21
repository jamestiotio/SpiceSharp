﻿using SpiceSharp.NewSparse;
using SpiceSharp.Simulations;
using SpiceSharp.Circuits;
using SpiceSharp.Behaviors;
using System;

namespace SpiceSharp.Components.InductorBehaviors
{
    /// <summary>
    /// Load behavior for a <see cref="Inductor"/>
    /// </summary>
    public class LoadBehavior : Behaviors.LoadBehavior, IConnectedBehavior
    {
        /// <summary>
        /// Nodes
        /// </summary>
        int posNode, negNode;
        public int BranchEq { get; protected set; }
        
        /// <summary>
        /// Matrix elements
        /// </summary>
        protected MatrixElement<double> PosBranchPtr { get; private set; }
        protected MatrixElement<double> NegBranchPtr { get; private set; }
        protected MatrixElement<double> BranchNegPtr { get; private set; }
        protected MatrixElement<double> BranchPosPtr { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name</param>
        public LoadBehavior(Identifier name) : base(name) { }

        /// <summary>
        /// Create export method
        /// </summary>
        /// <param name="propertyName">Property</param>
        /// <returns></returns>
        public override Func<RealState, double> CreateExport(string propertyName)
        {
            switch (propertyName)
            {
                case "v": return (RealState state) => state.Solution[posNode] - state.Solution[negNode];
                case "i":
                case "c": return (RealState state) => state.Solution[BranchEq];
                case "p": return (RealState state) => (state.Solution[posNode] - state.Solution[negNode]) * state.Solution[BranchEq];
                default: return null;
            }
        }

        /// <summary>
        /// Setup behavior
        /// </summary>
        /// <param name="provider">Provider</param>
        public override void Setup(SetupDataProvider provider)
        {
            // We don't need anything, acts like a short circuit
        }

        /// <summary>
        /// Connect
        /// </summary>
        /// <param name="pins">Pins</param>
        public void Connect(params int[] pins)
        {
            if (pins == null)
                throw new ArgumentNullException(nameof(pins));
            if (pins.Length != 2)
                throw new Diagnostics.CircuitException("Pin count mismatch: 2 pins expected, {0} given".FormatString(pins.Length));
            posNode = pins[0];
            negNode = pins[1];
        }

        /// <summary>
        /// Gets matrix pointers
        /// </summary>
        /// <param name="nodes">Nodes</param>
        /// <param name="solver">Solver</param>
        public override void GetEquationPointers(Nodes nodes, Solver<double> solver)
        {
            if (nodes == null)
                throw new ArgumentNullException(nameof(nodes));
            if (solver == null)
                throw new ArgumentNullException(nameof(solver));

            // Create current equation
            BranchEq = nodes.Create(Name.Grow("#branch"), Node.NodeType.Current).Index;

            // Get matrix pointers
            PosBranchPtr = solver.GetMatrixElement(posNode, BranchEq);
            NegBranchPtr = solver.GetMatrixElement(negNode, BranchEq);
            BranchNegPtr = solver.GetMatrixElement(BranchEq, negNode);
            BranchPosPtr = solver.GetMatrixElement(BranchEq, posNode);
        }

        /// <summary>
        /// Unsetup
        /// </summary>
        public override void Unsetup()
        {
            // Remove references
            PosBranchPtr = null;
            NegBranchPtr = null;
            BranchNegPtr = null;
            BranchPosPtr = null;
        }

        /// <summary>
        /// Execute behavior
        /// </summary>
        /// <param name="simulation">Base simulation</param>
        public override void Load(BaseSimulation simulation)
        {
            PosBranchPtr.Value += 1;
            NegBranchPtr.Value -= 1;
            BranchPosPtr.Value += 1;
            BranchNegPtr.Value -= 1;
        }
    }
}
