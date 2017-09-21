// Copyright 2017 Serilog Contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Serilog.Events;
using Serilog.Parsing;

namespace Serilog.Capturing
{
    /// <summary>
    /// A pre-processed message template with no arguments.
    /// </summary>
    public class PreparedMessageTemplate
    {
        static readonly MessageTemplateParser NonCachingParser = new MessageTemplateParser();
        internal static readonly IEnumerable<LogEventProperty> NoProperties = new LogEventProperty[0];

        readonly MessageTemplate _messageTemplate;

        internal static void ParseAndValidate(string messageTemplate, int expectedPropertyTokenCount, out MessageTemplate template, out PropertyToken[] propertyTokens)
        {
            if (messageTemplate == null) throw new ArgumentNullException(nameof(messageTemplate));

            template = NonCachingParser.Parse(messageTemplate);
            propertyTokens = template.Tokens.OfType<PropertyToken>().ToArray();

            if (propertyTokens.Length != expectedPropertyTokenCount)
                throw new ArgumentException("The message template being prepared does not have the expected number of property tokens.", nameof(messageTemplate));

            if (propertyTokens.Any(pt => pt.IsPositional))
                throw new ArgumentException("Prepared message templates cannot have positionally-bound tokens.");
        }

        /// <summary>
        /// Parse and pre-process <paramref name="messageTemplate"/> with no parameters.
        /// </summary>
        /// <param name="messageTemplate">The message template to prepare.</param>
        /// <returns>An object representing the prepared template.</returns>
        public PreparedMessageTemplate(string messageTemplate)
        {
            ParseAndValidate(messageTemplate, 0, out var template, out _);
            _messageTemplate = template;
        }

        /// <summary>
        /// Write an event through <paramref name="logger"/> carrying the template.
        /// </summary>
        /// <param name="logger">The logger to write an event to.</param>
        /// <param name="level">The level of the written event.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteEvent(ILogger logger, LogEventLevel level)
        {
            // ReSharper disable once IntroduceOptionalParameters.Global
            WriteEvent(logger, level, null);
        }

        /// <summary>
        /// Write an event through <paramref name="logger"/> carrying the template.
        /// </summary>
        /// <param name="logger">The logger to write an event to.</param>
        /// <param name="level">The level of the written event.</param>
        /// <param name="exception">An exception associated with the event.</param>
        public void WriteEvent(ILogger logger, LogEventLevel level, Exception exception)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (!logger.IsEnabled(level))
                return;

            logger.Write(new LogEvent(DateTimeOffset.Now, level, exception, _messageTemplate, NoProperties));
        }
    }

    /// <summary>
    /// A pre-processed message template with one argument.
    /// </summary>
    public class PreparedMessageTemplate<TArg0>
    {
        readonly PropertyToken _pt0;
        readonly MessageTemplate _messageTemplate;

        /// <summary>
        /// Parse and pre-process <paramref name="messageTemplate"/> with one parameter.
        /// </summary>
        /// <param name="messageTemplate">The message template to prepare.</param>
        /// <returns>An object representing the prepared template.</returns>
        public PreparedMessageTemplate(string messageTemplate)
        {
            PreparedMessageTemplate.ParseAndValidate(messageTemplate, 1, out var template, out var propertyTokens);
            _pt0 = propertyTokens[0];
            _messageTemplate = template;
        }

        /// <summary>
        /// Write an event through <paramref name="logger"/> carrying the template.
        /// </summary>
        /// <param name="logger">The logger to write an event to.</param>
        /// <param name="level">The level of the written event.</param>
        /// <param name="arg0">The first argument to bind according to the prepared template.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteEvent(ILogger logger, LogEventLevel level, TArg0 arg0)
        {
            WriteEvent(logger, level, null, arg0);
        }

        /// <summary>
        /// Write an event through <paramref name="logger"/> carrying the template.
        /// </summary>
        /// <param name="logger">The logger to write an event to.</param>
        /// <param name="level">The level of the written event.</param>
        /// <param name="exception">An exception associated with the event.</param>
        /// <param name="arg0">The first argument to bind according to the prepared template.</param>
        public void WriteEvent(ILogger logger, LogEventLevel level, Exception exception, TArg0 arg0)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (!logger.IsEnabled(level))
                return;

            var evt = new LogEvent(DateTimeOffset.Now, level, exception, _messageTemplate, PreparedMessageTemplate.NoProperties);

            logger.BindProperty(_pt0.PropertyName, arg0, _pt0.Destructuring == Destructuring.Destructure, out var p0);
            evt.AddOrUpdateProperty(p0);

            logger.Write(evt);
        }
    }

    /// <summary>
    /// A pre-processed message template with two arguments.
    /// </summary>
    public class PreparedMessageTemplate<TArg0, TArg1>
    {
        readonly PropertyToken _pt0, _pt1;
        readonly MessageTemplate _messageTemplate;

        /// <summary>
        /// Parse and pre-process <paramref name="messageTemplate"/> with two parameters.
        /// </summary>
        /// <param name="messageTemplate">The message template to prepare.</param>
        /// <returns>An object representing the prepared template.</returns>
        public PreparedMessageTemplate(string messageTemplate)
        {
            PreparedMessageTemplate.ParseAndValidate(messageTemplate, 2, out var template, out var propertyTokens);
            _pt1 = propertyTokens[1];
            _pt0 = propertyTokens[0];
            _messageTemplate = template;
        }

        /// <summary>
        /// Write an event through <paramref name="logger"/> carrying the template.
        /// </summary>
        /// <param name="logger">The logger to write an event to.</param>
        /// <param name="level">The level of the written event.</param>
        /// <param name="arg0">The first argument to bind according to the prepared template.</param>
        /// <param name="arg1">The second argument to bind according to the prepared template.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteEvent(ILogger logger, LogEventLevel level, TArg0 arg0, TArg1 arg1)
        {
            WriteEvent(logger, level, null, arg0, arg1);
        }

        /// <summary>
        /// Write an event through <paramref name="logger"/> carrying the template.
        /// </summary>
        /// <param name="logger">The logger to write an event to.</param>
        /// <param name="level">The level of the written event.</param>
        /// <param name="exception">An exception associated with the event.</param>
        /// <param name="arg0">The first argument to bind according to the prepared template.</param>
        /// <param name="arg1">The second argument to bind according to the prepared template.</param>
        public void WriteEvent(ILogger logger, LogEventLevel level, Exception exception, TArg0 arg0, TArg1 arg1)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (!logger.IsEnabled(level))
                return;

            var evt = new LogEvent(DateTimeOffset.Now, level, exception, _messageTemplate, PreparedMessageTemplate.NoProperties);

            logger.BindProperty(_pt0.PropertyName, arg0, _pt0.Destructuring == Destructuring.Destructure, out var p0);
            evt.AddOrUpdateProperty(p0);

            logger.BindProperty(_pt1.PropertyName, arg1, _pt1.Destructuring == Destructuring.Destructure, out var p1);
            evt.AddOrUpdateProperty(p1);

            logger.Write(evt);
        }
    }

    /// <summary>
    /// A pre-processed message template with three arguments.
    /// </summary>
    public class PreparedMessageTemplate<TArg0, TArg1, TArg2>
    {
        readonly PropertyToken _pt0, _pt1, _pt2;
        readonly MessageTemplate _messageTemplate;

        /// <summary>
        /// Parse and pre-process <paramref name="messageTemplate"/> with three parameters.
        /// </summary>
        /// <param name="messageTemplate">The message template to prepare.</param>
        /// <returns>An object representing the prepared template.</returns>
        public PreparedMessageTemplate(string messageTemplate)
        {
            PreparedMessageTemplate.ParseAndValidate(messageTemplate, 3, out var template, out var propertyTokens);
            _pt2 = propertyTokens[2];
            _pt1 = propertyTokens[1];
            _pt0 = propertyTokens[0];
            _messageTemplate = template;
        }

        /// <summary>
        /// Write an event through <paramref name="logger"/> carrying the template.
        /// </summary>
        /// <param name="logger">The logger to write an event to.</param>
        /// <param name="level">The level of the written event.</param>
        /// <param name="arg0">The first argument to bind according to the prepared template.</param>
        /// <param name="arg1">The second argument to bind according to the prepared template.</param>
        /// <param name="arg2">The third argument to bind according to the prepared template.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteEvent(ILogger logger, LogEventLevel level, TArg0 arg0, TArg1 arg1, TArg2 arg2)
        {
            WriteEvent(logger, level, null, arg0, arg1, arg2);
        }

        /// <summary>
        /// Write an event through <paramref name="logger"/> carrying the template.
        /// </summary>
        /// <param name="logger">The logger to write an event to.</param>
        /// <param name="level">The level of the written event.</param>
        /// <param name="exception">An exception associated with the event.</param>
        /// <param name="arg0">The first argument to bind according to the prepared template.</param>
        /// <param name="arg1">The second argument to bind according to the prepared template.</param>
        /// <param name="arg2">The third argument to bind according to the prepared template.</param>
        public void WriteEvent(ILogger logger, LogEventLevel level, Exception exception, TArg0 arg0, TArg1 arg1, TArg2 arg2)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (!logger.IsEnabled(level))
                return;

            var evt = new LogEvent(DateTimeOffset.Now, level, exception, _messageTemplate, PreparedMessageTemplate.NoProperties);

            logger.BindProperty(_pt0.PropertyName, arg0, _pt0.Destructuring == Destructuring.Destructure, out var p0);
            evt.AddOrUpdateProperty(p0);
            // evt.AddOrUpdateProperty(new LogEventProperty(_pt0.PropertyName, new ScalarValue(arg0)));

            logger.BindProperty(_pt1.PropertyName, arg1, _pt1.Destructuring == Destructuring.Destructure, out var p1);
            evt.AddOrUpdateProperty(p1);
            // evt.AddOrUpdateProperty(new LogEventProperty(_pt1.PropertyName, new ScalarValue(arg1)));

            logger.BindProperty(_pt2.PropertyName, arg2, _pt2.Destructuring == Destructuring.Destructure, out var p2);
            evt.AddOrUpdateProperty(p2);
            // evt.AddOrUpdateProperty(new LogEventProperty(_pt2.PropertyName, new ScalarValue(arg2)));

            logger.Write(evt);
        }
    }

}
