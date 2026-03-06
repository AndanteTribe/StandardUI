#nullable enable

using System;
using UnityEngine.EventSystems;

namespace StandardUI
{
    /// <summary>
    /// Controls UI input, such as temporarily blocking all uGUI interaction.
    /// </summary>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// using UnityEngine;
    /// using StandardUI;
    ///
    /// public class TouchSurfaceSample : MonoBehaviour
    /// {
    ///     private readonly TouchSurface _touchSurface = new TouchSurface();
    ///
    ///     private void SomeOperation()
    ///     {
    ///         // Disable all UI input during processing and restore it automatically
    ///         // when leaving the using scope.
    ///         using (_touchSurface.BlockScope())
    ///         {
    ///             // Use this while running long or asynchronous operations.
    ///         }
    ///     }
    /// }
    /// ]]>
    /// </code>
    /// </example>
    public sealed class TouchSurface
    {
        private readonly EventSystem _eventSystem;
        private uint _refCount;

        public TouchSurface(EventSystem? eventSystem = null)
        {
            _eventSystem = eventSystem ?? EventSystem.current;
        }

        /// <summary>
        /// Temporarily blocks UI input.
        /// </summary>
        /// <returns>A handle used to restore input.</returns>
        public BlockingScope BlockScope()
        {
            if (_refCount == 0)
            {
                _eventSystem.enabled = false;
            }
            _refCount++;
            return new BlockingScope(this);
        }

        /// <summary>
        /// Handle struct that controls temporary input blocking.
        /// </summary>
        /// <remarks>
        /// Intended to be used with a using scope.
        /// </remarks>
        public readonly struct BlockingScope : IDisposable
        {
            private readonly TouchSurface _surface;

            internal BlockingScope(TouchSurface surface) => _surface = surface;

            void IDisposable.Dispose()
            {
                _surface._refCount--;
                if (_surface._refCount == 0)
                {
                    _surface._eventSystem.enabled = true;
                }
            }
        }
    }
}