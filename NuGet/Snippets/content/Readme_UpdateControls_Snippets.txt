Use the following snippets to create independent and dependent properties:

ind - Independent property
  An independent property is one that can change on its own. These will be the
  values of your data model.

indlist - Independent list
  The user can add and remove items in an independent collection. These will be
  child collections in your data model.

dep - Dependent property
  A dependenty property calculates its value from code. The user cannot change
  its value directly. They can only change the independent properties that the
  dependent depends upon.

  All view model properties are dependent by default. Only use this template
  to cache the dependent value when it is expensive to calculate.

deplist - Dependent list
  A dependent list calculates its contents from a linq query. The user cannot
  add or remove items in this list. They can only change the independent lists
  and properties that the linq query depends upon.

  All view model lists are dependent by default. Only use this template to
  cache lists that are expensive to calculate, or are used by other properties
  of the view model.

If these snippets aren't working for you, please remove and reinstall the
UpdateControls.Snippets package.