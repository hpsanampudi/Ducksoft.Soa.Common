using System;

namespace Ducksoft.SOA.Common.Utilities
{
    /// <summary>
    /// Interface which holds datbase entity related audit information
    /// </summary>
    public interface IAuditColumns<TUserType> where TUserType : struct
    {
        /// <summary>
        /// Gets or sets the insert by.
        /// </summary>
        /// <value>
        /// The insert by.
        /// </value>
        TUserType InsertBy { get; set; }

        /// <summary>
        /// Gets or sets the insert date.
        /// </summary>
        /// <value>
        /// The insert date.
        /// </value>
        DateTime InsertDate { get; set; }

        /// <summary>
        /// Gets or sets the update by.
        /// </summary>
        /// <value>
        /// The update by.
        /// </value>
        TUserType? UpdateBy { get; set; }

        /// <summary>
        /// Gets or sets the update date.
        /// </summary>
        /// <value>
        /// The update date.
        /// </value>
        DateTime? UpdateDate { get; set; }

        /// <summary>
        /// Gets or sets the delete by.
        /// </summary>
        /// <value>
        /// The delete by.
        /// </value>
        TUserType? DeleteBy { get; set; }

        /// <summary>
        /// Gets or sets the delete date.
        /// </summary>
        /// <value>
        /// The delete date.
        /// </value>
        DateTime? DeleteDate { get; set; }
    }

    /// <summary>
    /// Class which holds audit column types used across the application.
    /// </summary>
    public enum AuditColumnTypes
    {
        /// <summary>
        /// The none
        /// </summary>
        [EnumDescription("None")]
        None = -1,
        /// <summary>
        /// All
        /// </summary>
        [EnumDescription("InsertBy,InsertDate,UpdateBy,UpdateDate,DeleteBy,DeleteDate")]
        All,
        /// <summary>
        /// The insert
        /// </summary>
        [EnumDescription("InsertBy,InsertDate")]
        Insert,
        /// <summary>
        /// The update
        /// </summary>
        [EnumDescription("UpdateBy,UpdateDate")]
        Update,
        /// <summary>
        /// The delete
        /// </summary>
        [EnumDescription("DeleteBy,DeleteDate")]
        Delete
    }
}
