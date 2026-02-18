# Post LinkedIn - PipeException

## C# 14 Extension Blocks : Implémenter l'opérateur | pour des validations fluides

---

Combien de fois avez-vous écrit ce genre de code ?

```csharp
public void ProcessOrder(Order order, int quantity, string customerId)
{
    if (order == null)
        throw new ArgumentNullException(nameof(order));

    if (quantity <= 0)
        throw new ArgumentException("Quantity must be positive", nameof(quantity));

    if (quantity > 1000)
        throw new ArgumentException("Quantity cannot exceed 1000", nameof(quantity));

    if (string.IsNullOrWhiteSpace(customerId))
        throw new ArgumentException("Customer ID is required", nameof(customerId));

    // Enfin, le code métier...
}
```

10 lignes de validation avant même de commencer le vrai travail. C'est verbeux, répétitif, et ça noie notre logique métier dans un océan de conditions.

Et si je vous disais qu'avec .NET 10, on peut écrire ça :

```csharp
public void ProcessOrder(Order order, int quantity, string customerId)
{
    var validOrder = order | (o => o != null);
    var validQty = quantity | (q => q > 0) | (q => q <= 1000);
    var validId = customerId | (s => !string.IsNullOrWhiteSpace(s));

    // Le code métier, directement
}
```

C'est propre. C'est lisible. C'est chaînable. Et ça lève les bonnes exceptions automatiquement.

Comment c'est possible ? Grâce aux **Extension Blocks**, une nouvelle fonctionnalité de C# 14 / .NET 10.

---

## Les Extension Blocks : une révolution pour C#

Jusqu'à présent, les méthodes d'extension en C# avaient une limitation majeure : on pouvait ajouter des méthodes, mais pas des opérateurs ou des propriétés.

Voici comment on définissait une extension classique :

```csharp
public static class StringExtensions
{
    public static bool IsNotEmpty(this string s) => !string.IsNullOrEmpty(s);
}
```

Avec les **Extension Blocks** de .NET 10, on peut maintenant définir des opérateurs sur n'importe quel type existant :

```csharp
public static class MyExtensions
{
    extension<T>(T source)
    {
        public static T operator |(T left, Func<T, bool> predicate)
        {
            // Notre logique ici
        }
    }
}
```

La syntaxe `extension<T>(T source)` crée un bloc où `source` représente l'instance sur laquelle on travaille. À l'intérieur de ce bloc, on peut définir des opérateurs, des propriétés, et des méthodes comme si on modifiait directement le type.

C'est cette nouveauté qui rend **PipeException** possible.

---

## PipeExceptionOperations : Le cœur de la librairie

Voici le code source complet de la classe principale :

```csharp
public static class PipeExceptionOperations
{
    extension<T>(T source)
    {
        /// <summary>
        /// Pipe operator for validation. Returns ValidationResult for deferred exception selection.
        /// </summary>
        public static ValidationResult<T> operator |(T left, Func<T, bool> predicate)
        {
            return new ValidationResult<T>(left, predicate, null, null);
        }

        /// <summary>
        /// Pipe operator with custom message.
        /// </summary>
        public static ValidationResult<T> operator |(T left, (Func<T, bool> predicate, string message) validation)
        {
            return new ValidationResult<T>(left, validation.predicate, validation.message, null);
        }

        /// <summary>
        /// Validates that the condition is met, throwing ArgumentException if not.
        /// </summary>
        public T Ensure(
            Func<T, bool> predicate,
            string? message = null,
            [CallerArgumentExpression(nameof(predicate))] string? predicateExpression = null)
        {
            if (!predicate(source))
            {
                var errorMessage = message ?? $"Condition not met: {predicateExpression}";
                throw new ArgumentException(errorMessage);
            }
            return source;
        }
    }
}
```

Décortiquons ce code :

**1. `extension<T>(T source)`**
C'est la magie des extension blocks. Ce bloc s'applique à n'importe quel type `T`. La variable `source` contient la valeur sur laquelle on opère.

**2. L'opérateur `|` surchargé**
Quand vous écrivez `value | (x => x > 0)`, cet opérateur est appelé. Il ne valide pas immédiatement ! Il retourne un `ValidationResult<T>` qui encapsule la valeur et le prédicat.

**3. La surcharge avec tuple**
`value | (x => x > 0, "Must be positive")` passe par la deuxième surcharge, qui capture aussi le message personnalisé.

**4. La méthode `Ensure`**
Elle utilise `[CallerArgumentExpression]` pour capturer automatiquement le texte du prédicat. Si vous écrivez `value.Ensure(x => x > 0 && x < 100)`, le message d'erreur contiendra `"Condition not met: x => x > 0 && x < 100"`.

---

## ValidationResult<T> : Le pattern fonctionnel

La vraie puissance vient de `ValidationResult<T>` :

```csharp
public readonly struct ValidationResult<T>
{
    private readonly T _value;
    private readonly Func<T, bool> _predicate;
    private readonly string? _message;

    // Permet le chaînage : result | (x => x < 100)
    public static ValidationResult<T> operator |(ValidationResult<T> result, Func<T, bool> predicate)
    {
        T value = result;  // Valide d'abord le prédicat précédent
        return new ValidationResult<T>(value, predicate, null, null);
    }

    // Conversion implicite vers T - c'est ici que l'exception est levée
    public static implicit operator T(ValidationResult<T> result)
    {
        if (!result._predicate(result._value))
        {
            var errorMessage = result._message ?? $"Condition not met: {result._predicateExpression}";
            throw new ArgumentException(errorMessage);
        }
        return result._value;
    }

    // Choisir le type d'exception
    public T OrThrowNull(string? paramName = null)
    {
        if (!_predicate(_value))
            throw new ArgumentNullException(paramName, _message);
        return _value;
    }

    public T OrThrowInvalidOperation()
    {
        if (!_predicate(_value))
            throw new InvalidOperationException(_message);
        return _value;
    }

    public T OrThrow<TException>(Func<string, TException> exceptionFactory)
        where TException : Exception
    {
        if (!_predicate(_value))
            throw exceptionFactory(_message ?? "Validation failed");
        return _value;
    }
}
```

**Le pattern clé :**
- La validation est **différée** jusqu'à ce qu'on ait besoin de la valeur
- La **conversion implicite** vers `T` déclenche la validation
- Les méthodes `OrThrow*` permettent de **choisir le type d'exception**
- L'opérateur `|` sur `ValidationResult` permet le **chaînage**

---

## Exemples pratiques

### Validation simple
```csharp
int age = userInput | (x => x >= 0);
// Lance ArgumentException si négatif
```

### Validations chaînées
```csharp
int score = value
    | (x => x >= 0)      // Doit être positif ou zéro
    | (x => x <= 100)    // Maximum 100
    | (x => x % 5 == 0); // Multiple de 5
```

### Messages personnalisés
```csharp
int age = input | (x => x >= 18, "Vous devez être majeur pour continuer");
```

### Choisir le type d'exception
```csharp
// ArgumentNullException
var user = GetUser() | (u => u != null);
user.OrThrowNull(nameof(user));

// InvalidOperationException
var config = LoadConfig() | (c => c.IsValid);
config.OrThrowInvalidOperation();

// Exception personnalisée
var data = FetchData() | (d => d.Count > 0);
data.OrThrow(msg => new DataNotFoundException(msg));
```

### Validateurs prêts à l'emploi
```csharp
// Numériques
int positive = value | Validate.Positive;
int nonNeg = value | Validate.NonNegative;
int ranged = value | Validate.InRange(1, 100);

// Strings
string name = input | Validate.NotNullOrEmpty;
string text = input | Validate.NotNullOrWhiteSpace;

// Collections
var items = list | Validate.NotEmpty<int>();
var sized = list | Validate.HasMinCount<int>(5);
```

---

## Extensions spécialisées incluses

La librairie inclut aussi des extensions fluides pour les cas courants :

### Strings
```csharp
string validated = input
    .EnsureNotNullOrEmpty()
    .EnsureMinLength(3)
    .EnsureMaxLength(50);
```

### Collections
```csharp
var items = list
    .EnsureNotEmpty()
    .EnsureMinCount(1)
    .EnsureMaxCount(100);
```

### Nullables
```csharp
int value = nullableInt.EnsureNotNull("La valeur est requise");
User user = nullableUser.EnsureNotNull(nameof(user), "Utilisateur non trouvé");
```

---

## Pourquoi PipeException ?

1. **Lisibilité** : Le code de validation se lit de gauche à droite, comme une phrase
2. **Chaînabilité** : Combinez plusieurs validations en une seule expression
3. **Flexibilité** : Choisissez le type d'exception après la validation
4. **Messages automatiques** : `CallerArgumentExpression` génère des messages significatifs
5. **Type-safe** : Tout est vérifié à la compilation

---

## Essayez-le !

**Prérequis** : .NET 10 Preview (les extension blocks sont une fonctionnalité C# 14)

**Installation** :
```bash
dotnet add package PipeException
```

**Le repo** : https://github.com/phmatray/PipeException

**NuGet** : https://www.nuget.org/packages/PipeException/

---

Les extension blocks vont-ils changer notre façon d'écrire du C# ? Qu'est-ce que vous en pensez ?

J'aimerais beaucoup avoir vos retours sur cette approche de la validation. Et si vous avez des idées d'améliorations, les PRs sont les bienvenues !

---

#dotnet #csharp #opensource #dotnet10 #coding #validation #cleancode #extensionmethods #programming #softwareengineering
