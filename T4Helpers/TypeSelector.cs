// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeSelector.cs" company="Believe">
//   WhenYouBelieve2014@gmail.com
//   https://entityinterfacegenerator.codeplex.com
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <remarks>
//     This class is already defined as part of the file EntityInterfaceGenerator.PartialEntities.tt
//     It is kept here for better readability (syntax highlighting helps).
//     You can remove this file without any issues.
// </remarks>
// <summary>
//   Represent a selection of types to add interfaces or attributes conditionally
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Ducksoft.Soa.Common.T4Helpers
{
    /// <summary>
    ///     Represent a selection of types to add interfaces or attributes conditionally
    /// </summary>
    public class TypeSelector
    {
        #region Fields

        /// <summary>
        ///     The regular expression to match type name.
        /// </summary>
        private readonly string _regularExpressionToMatchTypeName;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeSelector"/> class.
        /// </summary>
        /// <param name="regularExpressionToMatchTypeName">
        /// The regular expression to match type name.
        /// </param>
        public TypeSelector(string regularExpressionToMatchTypeName)
        {
            if (regularExpressionToMatchTypeName == null)
            {
                throw new ArgumentNullException("regularExpressionToMatchTypeName");
            }

            _regularExpressionToMatchTypeName = regularExpressionToMatchTypeName;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the base types to add.
        /// </summary>
        public string BaseTypesToAdd { get; private set; }

        /// <summary>
        ///     Gets the decoration line to add.
        /// </summary>
        public string DecorationLineToAdd { get; private set; }

        /// <summary>
        ///     Gets the regular expression to match type name.
        /// </summary>
        public string RegularExpressionToMatchTypeName
        {
            get { return _regularExpressionToMatchTypeName; }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The + operator to put 2 type selectors into 1 array of 2 for easy iteration
        /// </summary>
        /// <param name="leftSide">
        ///     The left side.
        /// </param>
        /// <param name="rightSide">
        ///     The right side.
        /// </param>
        /// <returns>
        ///     An array containing both left side and right side selectors
        /// </returns>
        public static TypeSelector[] operator +(TypeSelector leftSide, TypeSelector rightSide)
        {
            if (leftSide == null)
            {
                throw new ArgumentNullException("leftSide");
            }

            if (rightSide == null)
            {
                throw new ArgumentNullException("rightSide");
            }

            return new[] { leftSide, rightSide };
        }

        /// <summary>
        ///     The + operator to append an existing array of type selector with a new one.
        /// </summary>
        /// <param name="leftSide">
        ///     The left side.
        /// </param>
        /// <param name="rightSide">
        ///     The right side.
        /// </param>
        /// <returns>
        ///     An array containing both left side and right side selectors
        /// </returns>
        public static TypeSelector[] operator +(TypeSelector[] leftSide, TypeSelector rightSide)
        {
            if (leftSide == null)
            {
                throw new ArgumentNullException("leftSide");
            }

            if (rightSide == null)
            {
                throw new ArgumentNullException("rightSide");
            }

            return leftSide.Concat(new[] { rightSide }).ToArray();
        }

        /// <summary>
        /// Add base types.
        /// </summary>
        /// <param name="baseTypesToAdd">
        /// The base types to add.
        /// </param>
        /// <returns>
        /// The <see cref="TypeSelector"/>.
        /// </returns>
        /// <remarks>
        /// The baseTypesToAdd string will be appended to list of base types in the definition of each class matching the
        ///     criteria.
        ///     The criteria are defined outside of this class and in ForTypeNamesMatchingRegEx(regex)
        /// </remarks>
        public TypeSelector AddBaseTypes(string baseTypesToAdd)
        {
            if (baseTypesToAdd == null)
            {
                throw new ArgumentNullException("baseTypesToAdd");
            }

            BaseTypesToAdd = baseTypesToAdd;
            return this; // for chaining
        }

        /// <summary>
        /// The add decoration line.
        /// </summary>
        /// <param name="decorationLineToAdd">
        /// The decoration line to add.
        /// </param>
        /// <returns>
        /// The <see cref="TypeSelector"/>.
        /// </returns>
        public TypeSelector AddDecorationLine(string decorationLineToAdd)
        {
            if (decorationLineToAdd == null)
            {
                throw new ArgumentNullException("decorationLineToAdd");
            }

            DecorationLineToAdd = decorationLineToAdd;
            return this; // for chaining
        }

        /// <summary>
        /// The matches.
        /// </summary>
        /// <param name="typeName">
        /// The type name.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsMatch(string typeName)
        {
            return RegularExpressionToMatchTypeName != null && typeName != null &&
                   Regex.IsMatch(typeName, RegularExpressionToMatchTypeName);
        }

        #endregion
    } // class TypeSelector
}