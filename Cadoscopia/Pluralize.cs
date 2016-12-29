using System;
using JetBrains.Annotations;

namespace Cadoscopia
{
    public static class Pluralize
    {
        #region Methods

        public static string Do([NotNull] string word, int count)
        {
            if (string.IsNullOrWhiteSpace(word))
                throw new ArgumentException(@"Value cannot be null or whitespace.", nameof(word));
            if (count < 1) throw new ArgumentOutOfRangeException(nameof(count));
            
            return count > 1 ? word + "s" : word;
        }

        #endregion
    }
}