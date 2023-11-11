// -----------------------------------------------------------------------
// <copyright file="NetworkBroadcastDataType.cs" company="Lethal Company Modding Community">
// Copyright (c) Lethal Company Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LC_API.Data;

/// <summary>
/// Represents the type of data that has been received.
/// </summary>
// ReSharper disable IdentifierTypo
public enum NetworkBroadcastDataType
{
    /// <summary>
    /// Represents data that is an integer.
    /// </summary>
    BDint = 0,

    /// <summary>
    /// Represents data that is a float.
    /// </summary>
    BDfloat = 1,

    /// <summary>
    /// Represents data that is a vector.
    /// </summary>
    BDvector3 = 2,

    /// <summary>
    /// Represents data that is a string.
    /// </summary>
    BDstring = 3,
}