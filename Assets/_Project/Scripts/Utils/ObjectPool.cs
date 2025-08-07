using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : Component {
    private readonly T prefab;
    private readonly Transform parent;
    private readonly Queue<T> pool = new();

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

    public T Get() {
        if (pool.Count > 0) {
            T obj = pool.Dequeue();
            obj.gameObject.SetActive(true);
            return obj;
        } else {
            T obj = Object.Instantiate(prefab, parent);
            return obj;
        }
    }

    public void ReturnToPool(T obj) {
        obj.gameObject.SetActive(false);
        pool.Enqueue(obj);
    }
}