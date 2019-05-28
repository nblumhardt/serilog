using System;
using System.Collections.Generic;
using Serilog.Events;

namespace Serilog.Pipeline.Properties
{
    // These types use arrays rather than dictionaries because element counts are almost always small (ten or fewer)
    // and copying is frequent.
    // TODO, make methods inlinable by extracting throw statements; check _elements for null and fail gracefully.
    struct EventPropertiesBuilder : IEnumerable<EventProperty>
    {
        const int DefaultInitialCapacity = 4;

        EventProperty[] _elements;
        int _count;

        public EventPropertiesBuilder(int capacity)
        {
            if (capacity < 0) throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be non-negative.");
            _elements = new EventProperty[capacity];
            _count = 0;
        }

        public EventPropertiesBuilder(EventProperty[] initial, int reservedCapacity)
        {
            if (initial == null) throw new ArgumentNullException(nameof(initial));
            if (reservedCapacity < 0) throw new ArgumentOutOfRangeException(nameof(reservedCapacity), "Reserved capacity must be non-negative.");
            Array.Resize(ref initial, Math.Max(initial.Length + reservedCapacity, DefaultInitialCapacity));
            _elements = initial ?? throw new ArgumentNullException(nameof(initial));
            _count = initial.Length;
        }

        public EventProperties ToImmutable()
        {
            return new EventProperties(_elements, _count);
        }
        
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        // Struct enumerator would help, here.
        public IEnumerator<EventProperty> GetEnumerator()
        {
            for (var i = 0; i < _count; ++i)
            {
                yield return _elements[i];
            }
        }

        public int Count => _count;

        public bool Contains(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            for (var i = 0; i < _count; ++i)
            {
                if (_elements[i].Name == name)
                    return true;
            }

            return false;
        }

        public bool TryGetValue(string name, out LogEventPropertyValue value)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            for (var i = 0; i < _count; ++i)
            {
                var (n, v) = _elements[i];
                if (n == name)
                {
                    value = v;
                    return true;
                }
            }

            value = null;
            return false;
        }

        public LogEventPropertyValue this[string name]
        {
            get
            {
                if (!TryGetValue(name, out var value))
                    throw new KeyNotFoundException($"The property `{name}` was not found.");

                return value;
            }
            set => AddOrUpdate(new EventProperty(name, value));
        }

        // The duplication between the (name, value) and (property) overloads below needs to be resolved, but
        // currently they may perform slightly differently due to the potential avoidance o

        public bool TryAdd(in EventProperty property)
        {
            if (property.Equals(EventProperty.None)) throw new ArgumentNullException(nameof(property));

            for (var i = 0; i < _count; ++i)
            {
                if (_elements[i].Name == property.Name)
                    return false;
            }

            if (_elements.Length == _count)
                Array.Resize(ref _elements, Math.Max(DefaultInitialCapacity, _count * 2));

            _elements[_count] = property;
            _count += 1;
            return true;
        }

        public bool TryAdd(string name, LogEventPropertyValue value)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (value == null) throw new ArgumentNullException(nameof(value));

            for (var i = 0; i < _count; ++i)
            {
                if (_elements[i].Name == name)
                    return false;
            }

            if (_elements.Length == _count)
                Array.Resize(ref _elements, Math.Max(DefaultInitialCapacity, _count * 2));

            _elements[_count] = new EventProperty(name, value);
            _count += 1;
            return true;
        }

        public void Add(string name, LogEventPropertyValue value)
        {
            if (!TryAdd(name, value))
                throw new InvalidOperationException($"A property named `{name}` already exists.");
        }

        public void Add(in EventProperty property)
        {
            if (!TryAdd(in property))
                throw new InvalidOperationException($"A property named `{property.Name}` already exists.");
        }

        public void AddOrUpdate(in EventProperty property)
        {
            if (property.Equals(EventProperty.None)) throw new ArgumentNullException(nameof(property));

            for (var i = 0; i < _count; ++i)
            {
                if (_elements[i].Name == property.Name)
                {
                    _elements[i] = property;
                    return;
                }
            }

            if (_elements.Length == _count)
                Array.Resize(ref _elements, Math.Max(DefaultInitialCapacity, _count * 2));

            _elements[_count] = property;
            _count += 1;
        }

        // Uncertain how this should be approached; currently this enables fast copying
        // from an (already-distinct-keyed) LogEvent.Properties value to the builder.
        public void AddUnchecked(in EventProperty property)
        {
            _elements[_count] = property;
            _count += 1;
        }

        public bool Remove(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            for (var i = 0; i < _count; ++i)
            {
                if (_elements[i].Name == name)
                {
                    for (var j = i + 1; j < _count; ++i, ++j)
                    {
                        _elements[i] = _elements[j];
                    }

                    _count -= 1;
                    return true;
                }
            }

            return false;
        }
    }
}
