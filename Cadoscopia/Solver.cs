// MIT License

// Copyright(c) 2016 Cadoscopia http://cadoscopia.com

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using Accord.Math.Differentiation;
using Accord.Math.Optimization;
using Cadoscopia.Constraints;
using Cadoscopia.SketchServices;
using JetBrains.Annotations;

namespace Cadoscopia
{
    public static class Solver
    {
        #region Methods

        static Func<double[], double[]> Grad(int nbOfVariables, Func<double[], double> fn)
        {
            var gradient = new FiniteDifferences(nbOfVariables, fn);
            return x => gradient.Compute(x);
        }

        public static bool Solve([NotNull] List<Constraint> constraints)
        {
            if (constraints == null) throw new ArgumentNullException(nameof(constraints));
            if (constraints.Count == 0)
                throw new ArgumentException(@"Value cannot be an empty collection.", nameof(constraints));

            Parameter[] parameters = constraints.Where(c => !c.UseSharedParameters)
                .SelectMany(c => c.Parameters).ToArray();
            if (!parameters.Any()) return true;

            Func<double[], double> objective = args => {
                for (int i = 0; i < args.Length; i++)
                    parameters[i].Value = args[i];
                return constraints.Sum(c => c.Error);
            };
            
            var bbfgs = new BoundedBroydenFletcherGoldfarbShanno(parameters.Length, objective,
                Grad(parameters.Length, objective));

            return bbfgs.Minimize(parameters.Select(p => p.Value).ToArray());
        }

        #endregion
    }
}