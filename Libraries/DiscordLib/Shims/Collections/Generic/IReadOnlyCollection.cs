// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*============================================================
**
** Interface:  IReadOnlyCollection<T>
** 
** 
**
** Purpose: Base interface for read-only generic lists.
** 
===========================================================*/

using System.Collections;
using System.Collections.Generic;

namespace System.Collections.Generic
{
    /// <summary>
    /// Provides a read-only, view of a generic list.
    /// </summary>
    public interface IReadOnlyCollection<T> : IEnumerable, IEnumerable<T>
    {
        int Count { get; }
    }
}