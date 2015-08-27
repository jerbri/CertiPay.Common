using CertiPay.Common.Web.Extensions;
using Humanizer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace CertiPay.Common.Web
{
    /// <summary>
    ///
    /// </summary>
    public class ConventionBasedMetadataProvider : DataAnnotationsModelMetadataProvider
    {
        private static Type _defaultResourceType { get; set; }

        /// <summary>
        /// The default resource to use if none is specified on the [Display]
        /// </summary>
        public static Type DefaultResourceType
        {
            get { return _defaultResourceType; }
            set { _defaultResourceType = value; }
        }

        public static void Register()
        {
            ModelMetadataProviders.Current = new ConventionBasedMetadataProvider { };
        }

        // Order of checks:

        // [Display(Name = "xxx", ResourceType = typeof(SomeResource)] == use localized value
        // [Display(Name = "xxx")] == try fo find the resource using a default resource type or fall through
        // No attributes, use conventions to try and find a resource
        // Use a global resource type and the property name, if set
        // Fall back to just "Humanizing" the property name

        protected override ModelMetadata CreateMetadata(IEnumerable<Attribute> attributes, Type containerType, Func<object> modelAccessor, Type modelType, string propertyName)
        {
            var modelMetadata = base.CreateMetadata(attributes, containerType, modelAccessor, modelType, propertyName);

            // Go ahead and grab the attributes on this model at the start

            var propertyAttributes = attributes.ToList();

            if (!IsTransformRequired(modelMetadata, propertyAttributes))
            {
                // We don't need to do anything, the display is provided by the property

                return modelMetadata;
            }

            var conventionType = containerType ?? modelType;

            // See if we have any of our special attributes present to set a default resource type

            var conventionAttribute = conventionType.GetAttributeOnTypeOrAssembly<MetadataConventionsAttribute>();

            if (conventionAttribute != null && conventionAttribute.ResourceType != null)
            {
                // We found a convention attribute present on the model or assembly, use their configured default if we don't already have one

                _defaultResourceType = conventionAttribute.ResourceType;
            }

            // Before we get to our normal display, let's go check if any validation attributes need love

            ApplyConventionsToValidationAttributes(propertyAttributes, containerType, propertyName, _defaultResourceType);

            // See if we can find a DisplayAttribute that isn't fully formed or else it would've been used earlier

            var foundDisplayAttribute = propertyAttributes.OfType<DisplayAttribute>().FirstOrDefault();

            // Copy or create a new display attribute to pass into the rest of the localization pipeline

            var displayAttribute = foundDisplayAttribute.Copy() ?? new DisplayAttribute();

            // Scrap the old display attribute, use our newly copied/created one

            var rewrittenAttributes = 
                propertyAttributes
                .Except(new[] { foundDisplayAttribute })
                .Union(new[] { displayAttribute });

            // Use the attribute's resource type if specified, fall back to the default

            displayAttribute.ResourceType = displayAttribute.ResourceType ?? DefaultResourceType;

            if (displayAttribute.ResourceType != null)
            {
                // If we don't have a resource type, it's not going to be localized
                // So, let's see if we can find a display attribute with a name provided

                string displayAttributeName = GetDisplayAttributeName(containerType, propertyName, displayAttribute);

                if (displayAttributeName != null)
                {
                    // We found a usable display attribute name, update

                    displayAttribute.Name = displayAttributeName;
                }

                // Now check the resource if it has a matching property

                if (!displayAttribute.ResourceType.PropertyExists(displayAttribute.Name))
                {
                    // Meh, no luck, scrap that resource

                    displayAttribute.ResourceType = null;
                }
            }

            // Gran the metadata from the new attributes for inspection

            ModelMetadata metadata = base.CreateMetadata(rewrittenAttributes, containerType, modelAccessor, modelType, propertyName);

            if (metadata.DisplayName == null || metadata.DisplayName == metadata.PropertyName)
            {
                // We couldn't find a match, fall back to letting Humanizer split the property name up

                metadata.DisplayName = metadata.PropertyName.Humanize(LetterCasing.Title);
            }

            return metadata;
        }

        private static void ApplyConventionsToValidationAttributes(IEnumerable<Attribute> attributes, Type containerType, string propertyName, Type defaultResourceType)
        {
            // Don't limit the love to DisplayAttribute, also apply it for ValidationAttribute's

            foreach (ValidationAttribute validationAttribute in attributes.OfType<ValidationAttribute>())
            {
                // Only applying if there's not already an error message specified

                if (string.IsNullOrEmpty(validationAttribute.ErrorMessage))
                {
                    // Get the name of the attribute sans "Attribute"

                    string attributeShortName = validationAttribute.GetType().Name.Replace("Attribute", "");

                    // Build the key we're going to look for

                    string resourceKey = GetResourceKey(containerType, propertyName) + "_" + attributeShortName;

                    // If there's a resource type set, use that, otherwise fall back to the default

                    var resourceType = validationAttribute.ErrorMessageResourceType ?? defaultResourceType;

                    // Try and find an error message matching ResourceKey = containerType.Name + "_" + propertyName + "_" + Validation

                    if (!resourceType.PropertyExists(resourceKey))
                    {
                        // Hmm, no luck, try propertyName + "_Validation" instead

                        resourceKey = propertyName + "_" + attributeShortName;

                        if (!resourceType.PropertyExists(resourceKey))
                        {
                            // Still no? Ok try "Error_Validation"

                            resourceKey = "Error_" + attributeShortName;

                            // Alas, let's try a different attribute?

                            if (!resourceType.PropertyExists(resourceKey)) continue;
                        }
                    }

                    validationAttribute.ErrorMessageResourceType = resourceType;
                    validationAttribute.ErrorMessageResourceName = resourceKey;
                }
            }
        }

        private static string GetDisplayAttributeName(Type containerType, string propertyName, DisplayAttribute displayAttribute)
        {
            if (containerType != null)
            {
                if (String.IsNullOrEmpty(displayAttribute.Name))
                {
                    // Check for a resource named containerType.Name + "_" + propertyName

                    string resourceKey = GetResourceKey(containerType, propertyName);

                    if (displayAttribute.ResourceType.PropertyExists(resourceKey))
                    {
                        // Sweet, we found a match, use that

                        return resourceKey;
                    }

                    return propertyName;
                }
            }
            return null;
        }

        private static string GetResourceKey(Type containerType, string propertyName)
        {
            return containerType.Name + "_" + propertyName;
        }

        /// <summary>
        /// Returns true if the model does not have a specified display or display name specified
        /// </summary>
        private static Boolean IsTransformRequired(ModelMetadata modelMetadata, IList<Attribute> propertyAttributes)
        {
            if (String.IsNullOrEmpty(modelMetadata.PropertyName))
                return false;

            if (propertyAttributes.OfType<DisplayNameAttribute>().Any())
                return false;

            if (propertyAttributes.OfType<DisplayAttribute>().Any(_ => _.CanSupplyDisplayName()))
                return false;

            return true;
        }
    }
}