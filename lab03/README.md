# Лабораторная работа 3: Реализация коллекций и интерфейсов

## Цель

Познакомиться с интерфейсами коллекций в C# (.NET), реализовать простые коллекции и понять их контракт и функциональность.

## Реализованные коллекции

### 1. SimpleList<T>

Класс `SimpleList<T>` реализует интерфейсы:

- `IEnumerable<T>` - поддержка перебора элементов (foreach)
- `ICollection<T>` - базовые операции: добавление, удаление, подсчёт элементов
- `IList<T>` - индексированный доступ, вставка, удаление по индексу

**Особенности реализации:**

- Внутренне использует динамический массив (без готовых коллекций List)
- Автоматическое увеличение размера массива при необходимости
- Поддерживает: добавление, получение/установку по индексу, удаление, поиск индекса

**Основные методы:**

- `Add(T item)` - добавление элемента в конец
- `Remove(T item)` - удаление элемента
- `RemoveAt(int index)` - удаление по индексу
- `Insert(int index, T item)` - вставка по индексу
- `Contains(T item)` - проверка наличия элемента
- `IndexOf(T item)` - поиск индекса элемента
- `Clear()` - очистка коллекции
- `Count` - количество элементов

### 2. SimpleDictionary<TKey, TValue>

Класс `SimpleDictionary<TKey, TValue>` реализует интерфейсы:

- `IDictionary<TKey, TValue>` - словарь с ключами и значениями
- `IReadOnlyDictionary<TKey, TValue>` - поддержка только чтения

**Особенности реализации:**

- Использует хеш-таблицу с цепочками для разрешения коллизий
- Поддерживает добавление, удаление и поиск по ключу
- Обеспечивает перебор через `IEnumerable<KeyValuePair<TKey, TValue>>`

**Основные методы:**

- `Add(TKey key, TValue value)` - добавление пары ключ-значение
- `Remove(TKey key)` - удаление по ключу
- `ContainsKey(TKey key)` - проверка наличия ключа
- `TryGetValue(TKey key, out TValue value)` - безопасное получение значения
- `Clear()` - очистка словаря
- `Count` - количество элементов

### 3. DoublyLinkedList<T>

Класс `DoublyLinkedList<T>` реализует интерфейс:

- `IList<T>` - индексированный доступ, вставка, удаление по индексу

**Особенности реализации:**

- Двунаправленный связный список (каждый узел имеет ссылки на предыдущий и следующий)
- Оптимизированный поиск: для индексов в первой половине обход с начала, для второй половины - с конца
- Поддерживает все операции IList

**Основные методы:**

- `Add(T item)` - добавление элемента в конец
- `Remove(T item)` - удаление элемента
- `RemoveAt(int index)` - удаление по индексу
- `Insert(int index, T item)` - вставка по индексу
- `Contains(T item)` - проверка наличия элемента
- `IndexOf(T item)` - поиск индекса элемента
- `Clear()` - очистка списка
- `Count` - количество элементов

## Запуск проекта

### Сборка проекта

```bash
dotnet build
```

### Запуск приложения

```bash
dotnet run --project lab03.csproj
```

### Запуск тестов

```bash
dotnet test
```

## Примеры использования

### SimpleList

```csharp
var list = new SimpleList<int>();
list.Add(1);
list.Add(2);
list.Add(3);
Console.WriteLine(list[0]); // 1
list.Remove(2);
Console.WriteLine(list.Count); // 2
```

### SimpleDictionary

```csharp
var dict = new SimpleDictionary<string, int>();
dict.Add("один", 1);
dict.Add("два", 2);
Console.WriteLine(dict["один"]);
dict.Remove("один");
```

### DoublyLinkedList

```csharp
var list = new DoublyLinkedList<int>();
list.Add(1);
list.Add(2);
list.Insert(1, 10);
Console.WriteLine(list[1]);
```
