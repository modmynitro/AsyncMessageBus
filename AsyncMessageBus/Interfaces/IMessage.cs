using System;
using System.Collections.Generic;
using System.Text;

namespace AsyncMessageBus.Interfaces
{
    public interface IMessage
    {
        /// <summary>
        ///     Get the tag. May be <see langword="null"/>
        /// </summary>
        object? Tag { get; }
    }
}
