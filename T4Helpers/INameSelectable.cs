// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INameSelectable.cs" company="Believe">
//   WhenYouBelieve2014@gmail.com
//   https://entityinterfacegenerator.codeplex.com
// </copyright>
// <summary>
//   The NameSelectable interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Ducksoft.Soa.Common.T4Helpers
{
    /// <summary>
    ///     The NameSelectable interface.
    /// </summary>
    public interface INameSelectable
    {
        #region Public Properties

        /// <summary>
        ///     Gets the name.
        /// </summary>
        string Name { get; }

        #endregion
    }
}