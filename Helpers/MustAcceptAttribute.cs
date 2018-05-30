using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Ducksoft.SOA.Common.Helpers
{
    /// <summary>
    /// The custom data annotation classs which is used to validate html checkbox must be checked.
    /// </summary>
    /// <seealso cref="ValidationAttribute" />
    public class MustAcceptAttribute : ValidationAttribute, IClientValidatable
    {
        /// <summary>
        /// Returns true if ... is valid.
        /// </summary>
        /// <param name="value">The value of the object to validate.</param>
        /// <returns>
        /// true if the specified value is valid; otherwise, false.
        /// </returns>
        public override bool IsValid(object value) => Equals(value, true);

        /// <summary>
        /// Applies formatting to an error message, based on the data field where the error occurred.
        /// </summary>
        /// <param name="name">The name to include in the formatted message.</param>
        /// <returns>
        /// An instance of the formatted error message.
        /// </returns>
        public override string FormatErrorMessage(string name) => $"Must select '{name}' option!";

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
                ValidationType = nameof(MustAcceptAttribute),
                ErrorMessage = ErrorMessageString
            });
        }
    }
}
