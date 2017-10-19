// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDescriptionSelectable.cs" company="Believe">
//   WhenYouBelieve2014@gmail.com
//   https://entityinterfacegenerator.codeplex.com
// </copyright>
// <summary>
//   The DescriptionSelectable interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Ducksoft.Soa.Common.T4Helpers
{
    /// <summary>
    /// The DescriptionSelectable interface.
    /// </summary>
    public interface IDescriptionSelectable
    {
        #region Public Properties

        /// <summary>
        ///     Gets the description.
        /// </summary>
        string Description { get; }

        #endregion
    }
}