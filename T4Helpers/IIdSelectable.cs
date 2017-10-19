// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IIdSelectable.cs" company="Believe">
//   WhenYouBelieve2014@gmail.com
//   https://entityinterfacegenerator.codeplex.com
// </copyright>
// <summary>
//   The IIdSelectable interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Ducksoft.Soa.Common.T4Helpers
{
    /// <summary>
    ///     The IIdSelectable interface.
    /// </summary>
    public interface IIdSelectable
    {
        #region Public Properties

        /// <summary>
        ///     Gets the id.
        /// </summary>
        int Id { get; }

        #endregion
    }
}