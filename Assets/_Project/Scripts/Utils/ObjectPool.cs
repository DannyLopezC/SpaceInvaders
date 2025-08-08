using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A generic object pooling system for Unity that stores and reuses component instances
/// to reduce runtime allocations and improve performance.
/// </summary>
/// <typeparam name="T">The type of component to pool. Must inherit from <see cref="Component"/>.</typeparam>
public class ObjectPool<T> where T : Component {
    /// <summary>
    /// The prefab that will be instantiated when the pool needs more objects.
    /// </summary>
    private readonly T prefab;

    /// <summary>
    /// The parent transform under which all pooled objects are stored in the hierarchy.
    /// </summary>
    private readonly Transform parent;

    /// <summary>
    /// The queue storing available (inactive) objects in the pool.
    /// </summary>
    private readonly Queue<T> pool = new();

    /// <summary>
    /// Creates a new object pool and pre-fills it with a given number of objects.
    /// </summary>
    /// <param name="prefab">The prefab to use for creating new instances.</param>
    /// <param name="initialSize">The number of objects to create at initialization.</param>
    /// <param name="parent">
    /// Optional. The parent transform to hold pooled objects in the hierarchy.
    /// If null, a new GameObject will be created as the pool container.
    /// </param>
    public ObjectPool(T prefab, int initialSize, Transform parent = null) {
        this.prefab = prefab;

        if (parent == null) {
            GameObject parentObj = new GameObject($"{typeof(T).Name}_Pool");
            this.parent = parentObj.transform;
        } else {
            this.parent = parent;
        }

        for (int i = 0; i < initialSize; i++) {
            T obj = Object.Instantiate(prefab, this.parent);
            obj.gameObject.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    /// <summary>
    /// Retrieves an object from the pool.
    /// If no objects are available, a new one will be instantiated.
    /// </summary>
    /// <returns>An active instance of type <typeparamref name="T"/>.</returns>
    public T Get() {
        if (pool.Count > 0) {
            T obj = pool.Dequeue();
            obj.gameObject.SetActive(true);
            return obj;
        } else {
            return Object.Instantiate(prefab, parent);
        }
    }

    /// <summary>
    /// Returns an object to the pool, disabling it for reuse.
    /// </summary>
    /// <param name="obj">The object to return to the pool.</param>
    public void ReturnToPool(T obj) {
        obj.gameObject.SetActive(false);
        pool.Enqueue(obj);
    }
}