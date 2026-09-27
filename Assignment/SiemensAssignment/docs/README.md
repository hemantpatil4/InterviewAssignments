# Mendix expression evaluator

This repo contains code to execute a custom AST(Abstract Syntax Tree) format.

## Development

To start, install Visual studio 2022 community edition
https://visualstudio.microsoft.com/vs/community/

This assessment is intended to give insights on your coding and logical thinking skills. So please treat this assignment the same as working for Mendix.
If you have any questions at all, do not hesitate to contact us.

Before starting your assignment, please run* *git init && git add . && git commit -am "initial commit"* so that we can track your changes from the base line.

Open `Assignment.sln`

## Unit Tests

a test project is provided within the solution called `UnitTests`
you should be able to run all tests from the Visual Studio Test menu.


# Documentation

Mendix developers building apps can write short expressions in their apps. An expression can be as simple as `true` or
more complex such as `1 + 1 != 3`.
These expressions are parsed and converted to an AST. The documentation for the complete expression
syntax can be found [here](https://docs.mendix.com/refguide/expressions). This repo contains a simple evaluator that can
execute the AST for a very small subset of the Mendix expressions.


## AST Specification

The AST can contain different types of nodes. The type of node is specified by the `type` field. An AST type can either
be `literal` or `function`.

### Literal Expression

Literals represent constant values specified in an expression.

```json5
{
  type: "literal",
  value: "test"
}
```

#### value

This field can hold a string, boolean, int, float, DateTime, `null`, or an Array containing the same primitives.

### Function Expression

Functions represent an operation that accept zero or more parameters and return a value.
Here is a representation of the c# structure as JSON:

```json5
{
  type: "function",
  name: "function_name",
  parameters: [Expression1, Expression2, ...]
}
```

#### name

The operation to be performed on the parameters.

#### parameters

Zero or more sub-expression(s).

#### Return value

A function can return one of the following types: string, boolean, int, float, DateTime, `null`, or an Array containing the same
primitives.

## Examples

```json5
// Expression: 40 + 2
// AST:
{
  type: "function",
  name: "add",
  parameters: [
    {
      type: "literal",
      value: 40
    },
    {
      type: "literal",
      value: 2
    }
  ]
}
```

```json5
// Expression: 1 + 2 != 3
// AST:
{
  type: "function",
  name: "not",
  parameters: [
    {
      type: "function",
      name: "equals",
      parameters: [
        {
          type: "function",
          name: "add",
          parameters: [
            {
              type: "literal",
              value: 1
            },
            {
              type: "literal",
              value: 2
            },
          ]
        },
        {
          type: "literal",
          value: 3
        }
      ]
    }
  ]
}
```

## The Implementation

This assignment contains an implementation of the specification above. The `Functions.cs` contains three functions:

- `equals` function that should return whether the two arguments passed to it are equal or not.
- `add` function that should return a sum of all passed arguments.
- `not` function that should return `false` if the input parameter is `true` or return `true` if the input parameter  
  is `false`.

The `Evaluator.cs` contains the main component that evaluates an expression's AST.

## The Assignment

In this assignment, we ask you to improve this implementation and add some new functionality.

- This implementation is poorly done, and we could use your help. The code in this assignment can be improved in various
  ways. Please review the code and improve where you think it is necessary. We value not just functional improvements,
  but also the non-functional aspects such as clean code, maintainability, readability and extensibility. Also consider
  extending the unit tests.
- We expect you to work on the assignment in a similar manner to a real life work situation where you are striving
  towards a solution that is robust and "production-ready".
- Implement new functionality. Support evaluating the following expression:
  `contains(fetchGet("https://google.com"), "Bing")`.
  
  Please note that we don't expect you to parse this string into an AST, but support evaluating it.
  (You can use its AST below)
  - A function called `fetchGet` which can retrieve the contents of a given URL with a GET call and return as string.
  - A function called `contains` that accepts two string arguments and returns if the first argument contains the
    second argument.
  - After implementing the functions, the evaluator should able to evaluate the following AST:
    ```json5
    // contains(fetchGet("https://google.com"), "Bing")
    {
      "type": "function",
      "name": "contains",
      "parameters": [
        {
          "type": "function",
          "name": "fetchGet",
          "parameters": [
            {
              "type": "literal",
              "value": "https://google.com"
            }
          ]
        },
        {
          "type": "literal",
          "value": "Bing"
        }
      ]
    }
    ```

### Returning the assignment

You can create a new bundle with `git bundle create assignment.bundle --all` command after you're done with the
assignment. (Don't forget to commit your work) and send it back.

Good luck!
