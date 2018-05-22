using Ducksoft.Soa.Common.Utilities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Ducksoft.Soa.Common.Helpers
{
    /// <summary>
    /// The custom data annotation classs which is used to validate html radio button must selected anyone option.
    /// </summary>
    /// <seealso cref="ValidationAttribute" />
    public class MustSelectAttribute : ValidationAttribute, IClientValidatable
    {
        /// <summary>
        /// Gets a value indicating whether this instance is ignore none option.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is ignore none option; otherwise, <c>false</c>.
        /// </value>
        public bool IsIgnoreNoneOption { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MustSelectAttribute" /> class.
        /// </summary>
        /// <param name="isIgnoreNoneOption">if set to <c>true</c> [is ignore none option].</param>
        public MustSelectAttribute(bool isIgnoreNoneOption = false) : base()
        {
            IsIgnoreNoneOption = isIgnoreNoneOption;
        }

        /// <summary>
        /// Returns true if ... is valid.
        /// </summary>
        /// <param name="value">The value of the object to validate.</param>
        /// <returns>
        /// true if the specified value is valid; otherwise, false.
        /// </returns>
        public override bool IsValid(object value)
        {
            var source = (ThreeStateOptionTypes)value;
            if ((IsIgnoreNoneOption) && (source == ThreeStateOptionTypes.None))
            {
                return (false);
            }

            return (true);
        }

        /// <summary>
        /// Applies formatting to an error message, based on the data field where the error occurred.
        /// </summary>
        /// <param name="name">The name to include in the formatted message.</param>
        /// <returns>
        /// An instance of the formatted error message.
        /// </returns>
        public override string FormatErrorMessage(string name) =>
            $"Must select '{name}' related either of the options below!";

        /// <summary>
        /// When implemented in a class, returns client validation rules for that class.
        /// </summary>
        /// <param name="metadata">The model metadata.</param>
        /// <param name="context">The controller context.</param>
        /// <returns>
        /// The client validation rules for this validator.
        /// </returns>
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(
            ModelMetadata metadata, ControllerContext context)
        {
            yield return (new ModelClientValidationRule
            {
                ValidationType = nameof(MustSelectAttribute),
                ErrorMessage = ErrorMessageString
            });
        }
    }
}
