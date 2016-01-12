# UpdateControls

Dependency tracking MVVM framework

## NuGet

Install-Package updatecontrols

## Support for Winforms, WPF, Silverlight, Windows Phone, and UWP

This project compiles for all current .NET client platforms.

## Example

Wrap your object before giving it to the DataContext:

```c#
public partial class Window1 : Window
{
    public Window1()
    {
        InitializeComponent();
        DataContext = ForView.Wrap(new PersonViewModel(new Person()));
    }
}
```

The wrapped object given to DataContext is a plain-old-CLR-object.

```c#
public class PersonViewModel
{
    private Person _person;

    public PersonViewModel(Person person)
    {
        _person = person;
    }

    public Person Person
    {
        get { return _person; }
    }

    public string FirstLast
    {
        get { return _person.FirstName + " " + _person.LastName; }
    }

    public string LastFirst
    {
        get { return _person.LastName + ", " + _person.FirstName; }
    }

    public string Title
    {
        get { return "Person - " + (_person.DisplayStrategy == 0 ? FirstLast : LastFirst); }
    }
}
```

The underlying data object uses Independent properties to keep track of gets and sets. The wrapper can see these properties through layers of code, and automatically wires up property change notifications.

```c#
public class Person
{
    private Independent<string> _firstName = new Independent<string>();
    private Independent<string> _lastName = new Independent<string>();
    private Independent<int> _displayStrategy = new Independent<int>();

    public string FirstName
    {
        get { return _firstName; }
        set { _firstName.Value = value; }
    }

    public string LastName
    {
        get { return _lastName; }
        set { _lastName.Value = value; }
    }

    public int DisplayStrategy
    {
        get { return _displayStrategy; }
        set { _displayStrategy.Value = value; }
    }
}
```

You can data bind to any property, even the ones that calculate their value based on independent properties. When the independent property is changed, the dependent ones are updated, too.

## What just happened?

ForView.Wrap just created a wrapper object for you. This wrapper object has all of the properties of the object you passed in, but they are now bindable. The Silverlight version creates DependencyProperties for you. The WPF version implements INotifyPropertyChanged.

What's more, Update Controls is watching what your properties do. If they call OnGet(), or call anything that calls OnGet(), it sets up a dependency. Later, when something calls OnSet(), that DependencyProperty is updated with the new value.
