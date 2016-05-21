// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

namespace System
{
    /// <summary>
    ///     Dummy attribute to make collections Serializable when compiled as .NET 4.0 project
    /// </summary>
    internal sealed class SerializableAttribute : Attribute {}
}