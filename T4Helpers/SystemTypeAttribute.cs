// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemTypeAttribute.cs" company="Believe">
//   WhenYouBelieve2014@gmail.com
//   https://entityinterfacegenerator.codeplex.com
// </copyright>
// <summary>
//   Indicates that the table's content is to be populated by the system or internal staff, not end users
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Ducksoft.Soa.Common.T4Helpers
{
    /// <summary>
    ///     Indicates that the table's content is to be populated by the system or internal staff, not end users
    /// </summary>
    /// <remarks>
    ///     This is just a made up attribute to demonstrate how you can add custom attributes to a group of entities.
    /// </remarks>
    public class SystemTypeAttribute : Attribute
    {
    }
}