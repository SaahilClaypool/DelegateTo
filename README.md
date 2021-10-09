# Delegate To

Inheritance is often used to share code, but this can lead to unintended complexity.
One way around this is to favor composition over inheritance.

Some languages have a bit of syntactic sugar to make delegating behavior to a sub-entity easier:
- [Ruby/Rails](https://apidock.com/rails/Module/delegate) allows you to specify a "delegate" where methods of a given name are forwarded to a composite type

    ```rb
    class Greeter < ActiveRecord::Base
        def hello
            'hello'
        end
    end

    class Foo < ActiveRecord::Base
        belongs_to :greeter
        delegate :hello, to: :greeter
    end

    Foo.new.hello  # implicitly forward "hello" to the contained "Greeter"
    ```

- [Go](https://www.geeksforgeeks.org/promoted-fields-in-golang-structure/) has a concept of promoted fields where methods are forwarded to an anonymous composed type

    ```go
    struct Student {
        Year int
        Person // anonymous person delegate
    }
    
    struct Person {
        Age int
    }

    student := Student { Year: 2010, Person: Person { Age: 18 } }
    student.Age // -> 18
    ```

C# does not have this feature, so inheritance is the easiest way to share code or interfaces between classes.

**until now**

With source generators, we can forward, at compile time, all desired functionality from parent class to a composed class, and inherit all required interfaces.

Example: 

```csharp
partial class Student
{
    public int Year { get; set; }

    [GenerateDelegate] // signal source generator to create delegate functions
    Person Person { get; set; }
}

class Person
{
    public int Age { get; set; }
}

var student = new Student { Year = 2010, Person = new() { Age = 18 }};
student.Age // -> 18

```

This will be done by generating the following partial class for student with the needed functionality


```cs
partial class Student
{
    public Age { get => Person.Age; set => Person.Age = value; }
}
```

As a bonus, the Student or parent class should be able to automatically gain any and all interfaces implemented by the composed class as it will now fulfill all of those contracts.

See [./DelegateTo.Test](./DelegateTo.Test) for examples.
