using System;
using System.Collections.Generic;
using UnityEngine;

public partial class ServiceLocator {
    public static ServiceLocator Instance { get; } = new();

    private readonly Dictionary<Type, Func<object>> _factories;
    private readonly Dictionary<Type, object> _services;
    private readonly HashSet<Type> _currentlyResolving;

    // Private constructor to enforce singleton pattern
    private ServiceLocator() {
        _factories = new Dictionary<Type, Func<object>>();
        _services = new Dictionary<Type, object>();
        _currentlyResolving = new HashSet<Type>();

        RegisterFactories();
    }

    // Method to add factory
    public void AddFactory<T>(Func<ServiceLocator, T> factoryMethod) where T : class {
        _factories[typeof(T)] = () => factoryMethod(this);
    }

    public void RemoveFactory<T>() where T : class {
        Type type = typeof(T);
        if (_factories.ContainsKey(type)) {
            _factories.Remove(type);
        }

        if (_services.ContainsKey(type)) {
            _services.Remove(type); // Remove the cached instance as well
        }
    }

    // Method to retrieve a service, caching it if not already created
    public T GetService<T>() where T : class {
        Type type = typeof(T);
        if (_currentlyResolving.Contains(type)) {
            Debug.LogError($"#ServiceLocator# Circular dependency detected while resolving type {type}");
            return null;
        }

        if (!_services.ContainsKey(type)) {
            if (!_factories.ContainsKey(type)) {
                Debug.LogError($"#ServiceLocator# No factory registered for type {type}");
                return null;
            }

            _currentlyResolving.Add(type);
            _services[type] = _factories[type]();
            _currentlyResolving.Remove(type);
        }

        return (T)_services[type];
    }
}