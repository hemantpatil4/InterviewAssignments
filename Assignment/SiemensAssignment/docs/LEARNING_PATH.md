# Learning path — Expression Evaluator

We improve the **original `Assessment/` code one step at a time** inside `Submission/`.

Goal: each change should fit your mental model before the next one.

---

## How we work

1. Discuss **one** change (why + approach)
2. You try it, or ask to apply just that step
3. Run playground / tests and observe
4. Check the step off here, then move on

```bash
cd Submission
dotnet run --project Assessment
dotnet test UnitTests
```

---

## Roadmap

| Step | Focus | Status | Why |
|------|--------|--------|-----|
| **1** | Fix `not` | done | Evaluates arg, then ignores it; breaks nested AST |
| **2** | Fix `add` numbers | done | Boxing / mixed int–double |
| **3** | Fix `equals` | done | Should compare real values, not only bools |
| **4** | Clearer errors | done | Meaningful messages instead of raw cast explosions |
| **5** | Registry / less if-else | done | Easier to add functions |
| **6** | `contains` + `fetchGet` | done | Assignment feature |
| **7** | Tests | done | Lock behavior in |

---

## Step notes

### Step 1 — Fix `not`

**Bug**

```csharp
var param1 = Evaluate(function.Parameters[0]);  // computed...
return Functions.Not(function.Parameters[0]);     // ...but not used!
```

`Not` takes a `Node` and assumes `Literal`, so nested cases like `not(equals(...))` fail.

**Mental model**

For any function: evaluate children first → operate on **values**, not raw AST nodes.

**Approach**

1. Change `Not` to take a `Value` (already evaluated), not a `Node`
2. In `Evaluator`, pass `param1` into `Not`

**Done when**

- `not(true)` → `false`
- `not(equals(true, true))` → `false` (no cast crash)

**What you did**

- Original `Not(Node)` / `Not(function.Parameters[0])` left commented
- `Not(Value arg)` + `return Functions.Not(param1)`

---

### Step 2 — Fix `add` numbers

**Bug (original)**

```csharp
return new Value(param1.Get<double>() + param2.Get<double>());
```

`new Value(3)` boxes an `int`. `Get<double>()` tries to unbox as `double` → `InvalidCastException`.

**Mental model**

For arithmetic, `int` and `double` are the same family: “number.”  
Don’t require identical runtime types — convert both to a common numeric view, then add.

**Done when**

- `add(3, 6)` → `9`
- `add(0.3, 0.6)` → `0.9`
- `add(3, 6.8)` → `9.8`

**What you did**

- Original `Get<double>()` line left commented
- `TryAsNumber`: `int` → widen to `double`, `double` stays `double`
- Both ints → return `int`; otherwise return `double`
- String+string concat kept

---

### Step 3 — Fix `equals`

**Bug (original)**

```csharp
return new Value(param1.Get<bool>() == param2.Get<bool>());
```

Only works if **both** sides are already `bool`.  
Fails for: `equals(add(1, 2), 3)`.

**Mental model**

`equals` means: “are these two **values** the same?”

**Done when**

- `equals(true, true)` → `true`
- `equals(3, 3)` / `equals(3, 3.0)` → `true`
- `equals(add(1, 2), 3)` → `true`
- `not(equals(add(1, 2), 3))` → `false`

**What you did**

- Original bool-only `Equals` left commented
- Numbers via `TryAsNumber`; null rules; otherwise `object.Equals`

---

### Step 4 — Clearer errors

**Problem**

Failures blew up as `InvalidCastException` or vague `Exception("Unknown function")` — hard to debug.

**Mental model**

Evaluation failures are domain errors. Throw a dedicated type with a sentence that names **what failed** and **what types you had**.

**Approach**

1. Add `EvaluationException`
2. `Value.Get<T>()` — check type, then throw with expected vs actual
3. Unknown function — include the name
4. `add` type mismatch — include both argument types

**Done when**

- Wrong `Get<T>` → `EvaluationException` with a readable message
- `toString()` → `Unknown function: 'toString'`
- Bad `add` types → message names the types

**What you did**

- Added `EvaluationException.cs`
- Original `Get` cast / generic throws left commented where replaced
- `Program.cs` left unchanged

---

### Step 5 — Registry / less if-else

**Problem**

`Evaluator` grew an if/else per function. Every new function meant editing the walker again.

**Mental model**

- Evaluator: walk AST, evaluate children, look up by name  
- Registry: name → implementation  
- Each function: its own `IFunction` class

**What you did**

- Added `IFunction`, `FunctionRegistry`
- Moved `add` / `equals` / `not` into `AddFunction` / `EqualsFunction` / `NotFunction`
- Slimmed `Evaluator` to: evaluate args → `Registry.TryGet` → `Invoke`
- Old if/else and static `Functions` left commented
- `Program.cs` / public `Evaluator.Evaluate` API unchanged

---

### Step 6 — `contains` + `fetchGet`

**What you did**

- `ContainsFunction` — two strings, returns bool
- `FetchGetFunction(HttpClient)` — GET URL → body string
- `FunctionRegistry.CreateDefault(HttpClient?)` — inject client for tests
- `Evaluator` instance + `EvaluateNode` for custom registry; static `Evaluate` unchanged for `Program.cs`
- Unit test uses `FakeHandler` (no real network)

---

### Step 7 — Tests

**What you did**

- Mixed int/double `add`, N-arg `add(3,4,5,6)`
- `equals` numbers / mixed numeric
- README nested `not(equals(add(1,2),3))`
- `contains` happy path + bad types
- `contains(fetchGet(...), "Bing")` with fake `HttpClient`

Also closed gap: `add` sums **all** arguments (README), not only exactly two.

---

## Session log

| Date | What happened |
|------|----------------|
| 2026-09-21 | Reverted `Submission` to original starter. Starting Step 1 again interactively. |
| 2026-09-21 | Applied Steps 1–3; original code kept as comments. |
| 2026-09-21 | Applied Step 4 (clearer errors). `Program.cs` untouched. |
| 2026-09-21 | Applied Step 5 (registry / IFunction). |
| 2026-09-21 | Step 6: contains + fetchGet with injectable HttpClient. |
| 2026-09-21 | Closed gaps: N-arg add + expanded unit tests (Step 7). |
