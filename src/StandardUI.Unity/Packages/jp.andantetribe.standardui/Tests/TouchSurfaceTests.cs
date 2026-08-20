#nullable enable

using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;

namespace StandardUI.Tests
{
    public sealed class TouchSurfaceTests
    {
        private readonly List<GameObject> _gameObjects = new List<GameObject>();

        [TearDown]
        public void TearDown()
        {
            for (var i = _gameObjects.Count - 1; i >= 0; i--)
            {
                if (_gameObjects[i] != null)
                {
                    UnityEngine.Object.DestroyImmediate(_gameObjects[i]);
                }
            }

            _gameObjects.Clear();
        }

        [Test]
        public void ConstructorUsesCurrentEventSystemWhenArgumentIsNull()
        {
            var eventSystem = CreateEventSystem("CurrentEventSystem");
            EventSystem.current = eventSystem;
            var surface = new TouchSurface();

            using (surface.BlockScope())
            {
                Assert.That(eventSystem.enabled, Is.False);
            }

            Assert.That(eventSystem.enabled, Is.True);
        }

        [Test]
        public void ConstructorUsesProvidedEventSystemInsteadOfCurrent()
        {
            var currentEventSystem = CreateEventSystem("CurrentEventSystem");
            var providedEventSystem = CreateEventSystem("ProvidedEventSystem");
            EventSystem.current = currentEventSystem;
            var surface = new TouchSurface(providedEventSystem);

            using (surface.BlockScope())
            {
                Assert.That(currentEventSystem.enabled, Is.True);
                Assert.That(providedEventSystem.enabled, Is.False);
            }

            Assert.That(currentEventSystem.enabled, Is.True);
            Assert.That(providedEventSystem.enabled, Is.True);
        }

        [Test]
        public void ConstructorUsesCurrentEventSystemWhenProvidedEventSystemIsDestroyed()
        {
            var currentEventSystem = CreateEventSystem("CurrentEventSystem");
            var destroyedEventSystem = CreateEventSystem("DestroyedEventSystem");
            EventSystem.current = currentEventSystem;
            UnityEngine.Object.DestroyImmediate(destroyedEventSystem.gameObject);
            var surface = new TouchSurface(destroyedEventSystem);

            using (surface.BlockScope())
            {
                Assert.That(currentEventSystem.enabled, Is.False);
            }

            Assert.That(currentEventSystem.enabled, Is.True);
        }

        [Test]
        public void NestedBlockScopesKeepEventSystemDisabledUntilLastScopeIsDisposed()
        {
            var eventSystem = CreateEventSystem("EventSystem");
            var surface = new TouchSurface(eventSystem);

            using (surface.BlockScope())
            {
                Assert.That(eventSystem.enabled, Is.False);

                using (surface.BlockScope())
                {
                    Assert.That(eventSystem.enabled, Is.False);
                }

                Assert.That(eventSystem.enabled, Is.False);
            }

            Assert.That(eventSystem.enabled, Is.True);
        }

        [Test]
        public void UsingScopeRestoresEventSystemWhenBodyThrows()
        {
            var eventSystem = CreateEventSystem("EventSystem");
            var surface = new TouchSurface(eventSystem);
            var expectedException = new InvalidOperationException("Expected test exception.");

            var actualException = Assert.Throws<InvalidOperationException>(() =>
            {
                using (surface.BlockScope())
                {
                    Assert.That(eventSystem.enabled, Is.False);
                    throw expectedException;
                }
            });

            Assert.That(actualException, Is.SameAs(expectedException));
            Assert.That(eventSystem.enabled, Is.True);
        }

        private EventSystem CreateEventSystem(string name)
        {
            var gameObject = new GameObject(name, typeof(EventSystem));
            _gameObjects.Add(gameObject);
            return gameObject.GetComponent<EventSystem>();
        }
    }
}
