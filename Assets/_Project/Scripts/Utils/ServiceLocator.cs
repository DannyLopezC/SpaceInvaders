using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A singleton-based Service Locator implementation for managing and resolving dependencies.
/// This allows for decoupled architecture by registering service factories and retrieving instances on demand.
/// </summary>
public partial class ServiceLocator {
    /// <summary>
    /// The global singleton instance of the ServiceLocator.
    /// </summary>
    public static ServiceLocator Instance { get; } = new();

    /// <summary>
    /// Stores registered service factories, keyed by service type.
    /// </summary>
    private readonly Dictionary<Type, Func<object>> _factories;

    /// <summary>
    /// Stores created service instances, keyed by service type.
    /// </summary>
    private readonly Dictionary<Type, object> _services;

    /// <summary>
    /// Tracks currently resolving service types to detect circular dependencies.
    /// </summary>
    private readonly HashSet<Type> _currentlyResolving;

    /// <summary>
    /// Private constructor to enforce the singleton pattern.
    /// Initializes collections and registers any default factories.
    /// </summary>
    private ServiceLocator() {
        _factories = new Dictionary<Type, Func<object>>();
        _services = new Dictionary<Type, object>();
        _currentlyResolving = new HashSet<Type>();

        RegisterFactories();
    }

    /// <summary>
    /// Registers a factory method for creating instances of the specified service type.
    /// </summary>
    /// <typeparam name="T">The service interface or class type.</typeparam>
    /// <param name="factoryMethod">A function that creates an instance of the service.</param>
    public void AddFactory<T>(Func<ServiceLocator, T> factoryMethod) where T : class {
        _factories[typeof(T)] = () => factoryMethod(this);
    }

    /// <summary>
    /// Removes a factory and its cached instance for the specified service type.
    /// </summary>
    /// <typeparam name="T">The service interface or class type.</typeparam>
    public void RemoveFactory<T>() where T : class {
        Type type = typeof(T);
        if (_factories.ContainsKey(type)) {
            _factories.Remove(type);
        }

        if (_services.ContainsKey(type)) {
            _services.Remove(type); // Also remove cached instance
        }
    }

    /// <summary>
    /// Retrieves a service instance.  
    /// If the service is not already created, its factory will be invoked to create and cache it.
    /// </summary>
    /// <typeparam name="T">The service interface or class type.</typeparam>
    /// <returns>The resolved service instance, or null if no factory is registered.</returns>
    public T GetService<T>() where T : class {
        Type type = typeof(T);

        // Detect circular dependencies
        if (_currentlyResolving.Contains(type)) {
            Debug.LogError($"#ServiceLocator# Circular dependency detected while resolving type {type}");
            return null;
        }

        // If service not yet created, attempt to build it
        if (!_services.ContainsKey(type)) {
            if (!_factories.ContainsKey(type)) {
                // No factory registered for this type
                return null;
            }

            _currentlyResolving.Add(type);
            _services[type] = _factories[type]();
            _currentlyResolving.Remove(type);
        }

        return (T)_services[type];
    }
}