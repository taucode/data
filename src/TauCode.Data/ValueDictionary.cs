// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// 1. https://github.com/aspnet/AspNetWebStack/blob/master/src/Common/PropertyHelper.cs
// 2. https://github.com/aspnet/AspNetWebStack/blob/master/src/System.Web.Http/Routing/HttpRouteValueDictionary.cs

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace TauCode.Data
{
    [SuppressMessage("Microsoft.Usage", "CA2237:MarkISerializableTypesWithSerializable", Justification = "This class will never be serialized.")]
    public class ValueDictionary : Dictionary<string, object>
    {
        public ValueDictionary()
            : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        public ValueDictionary(IDictionary<string, object> dictionary)
            : base(StringComparer.OrdinalIgnoreCase)
        {
            if (dictionary != null)
            {
                foreach (KeyValuePair<string, object> current in dictionary)
                {
                    Add(current.Key, current.Value);
                }
            }
        }

        public ValueDictionary(object values)
            : base(StringComparer.OrdinalIgnoreCase)
        {
            IDictionary<string, object> valuesAsDictionary = values as IDictionary<string, object>;
            if (valuesAsDictionary != null)
            {
                foreach (KeyValuePair<string, object> current in valuesAsDictionary)
                {
                    Add(current.Key, current.Value);
                }
            }
            else if (values != null)
            {
                foreach (PropertyHelper property in PropertyHelper.GetProperties(values))
                {
                    // Extract the property values from the property helper
                    // The advantage here is that the property helper caches fast accessors.
                    Add(property.Name, property.GetValue(values));
                }
            }
        }
    }
}