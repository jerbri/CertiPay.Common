CertiPay.Common.Web
===================

This is inspired and takes greatly from Phil Haack's mvc-metadata-conventions package (https://github.com/Haacked/mvc-metadata-conventions)
as well as Humanizer's HumanizerMetadataProvider (https://github.com/MehdiK/Humanizer). 

All credit goes to these projects for a large chunk of the code used here, and I cannot claim I
was too terribly creative in my portions. Thanks to them!!

The intent was to kind of get the best of both worlds for as many types as possible.

Adding this package will automatically register it as the Current ModelMetadataProviders for ASP.net MVC.

It will, of course, allow any [Display] or [DisplayName] attributes specifically set to take precence.

It will the try and match a resource with the property name in the default resource file.

If there is no match, it will fall back and allow Humanizer to work it's magic on splitting the name of the property.