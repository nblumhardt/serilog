using System;
using System.Collections.Generic;
using Serilog.Events;

namespace Serilog.Pipeline.Properties
{
    // These types use arrays rather than dictionaries because element counts are almost always small (ten or fewer)
    // and copying is frequent.
    // TODO, make methods inlinable by extracting throw statements; check _elements for null and fail gracefully.

    readonly struct EventProperties
    {
        readonly EventProperty[] _elements;

        // TODO - make this "safe"/copying, and move the non-copying behavior to FromElements
        public EventProperties(EventProperty[] elements, int count)
        {
            if (elements == null) throw new ArgumentNullException(nameof(elements));
            if (elements.Length < count) throw new ArgumentOutOfRangeException(nameof(elements), "Too few elements provided.");
            if (count <= 0) throw new ArgumentOutOfRangeException(nameof(count), "Count must be non-negative.");
            if (count != elements.Length)
                Array.Resize(ref elements, count);
            _elements = elements;
        }

        public int Count => _elements.Length;

        public bool Contains(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            for (var i = 0; i < _elements.Length; ++i)
            {
                if (_elements[i].Name == name)
                    return true;
            }

            return false;
        }

        public bool TryGetValue(string name, out LogEventPropertyValue value)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            for (var i = 0; i < _elements.Length; ++i)
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
        }
        
        public EventPropertiesBuilder ToBuilder(int reservedCapacity)
        {
            return new EventPropertiesBuilder(_elements, reservedCapacity);
        }
    }
}
