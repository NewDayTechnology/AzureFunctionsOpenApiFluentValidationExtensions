# FunctionsValidationFilter

Load FluentValidation rules at runtime and decorate the schema generator with all rules that can be translated in the OpenApi spec.

This could be a shared library to be used across multiple projects or even open sourced.

Currently the Spec generator in the Microsoft library is not open for extension but there is already an open PR to allow for this. If it doesn’t get merged, it will require working with a fork or internal package.
Azure/azure-functions-openapi-extension#344

Rules can get tricky to convert and there are hard to solve problems (e.g. translating regex dialect) so we should take a pragmatic approach of including only what is easy to port programmatically, otherwise a manual approach would be better (eventually this could become a validation library to check that most of the rules in the spec match the one in FluentValidation to avoid rules getting out of sync).
