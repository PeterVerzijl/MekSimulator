using System.Collections.Generic;

public class PriorityQueue<T> where T : class {

    public int Count {
        get { return stack.Count; }
    }

    public List<T> stack = new List<T>();

    public delegate int Compare(T a, T b);
    public Compare compare;

    public PriorityQueue(Compare compare) {
        this.compare = compare;
    }

    public bool Push(T item) {
        bool hasAdded = false;
        if (stack.Count == 0) {
            stack.Add(item);
            hasAdded = true;
        } else if (!Contains(item)) {
            for (int stackIndex = 0; stackIndex < stack.Count; stackIndex++) {
                T stackItem = stack[stackIndex];
                int compareResult = compare(item, stackItem);
                if (compareResult == -1) {
                    stack.Insert(stackIndex, item);
                    hasAdded = true;
                    break;
                } else if (compareResult == 1) {
                    continue;
                } else if (compareResult == 0) {
                    stack.Insert(stackIndex + 1, item);
                    hasAdded = true;
                    break;
                }
            }
            if (!hasAdded) {
                // Add at the end
                stack.Add(item);
            }
        }
        return hasAdded;
    }

    public T Pop() {
        T result = stack[0];
        stack.RemoveAt(0);
        return result;
    }

    internal bool Remove(T item) {
        for (int stackIndex = 0; stackIndex < stack.Count; stackIndex++) {
            T stackItem = stack[stackIndex];
            if (stackItem == item) {
                return stack.Remove(stackItem);
            }
        }
        return false;
    }

    internal bool Contains(T target) {
        foreach(T item in stack) {
            if (item == target) {
                return true;
            }
        }
        return false;
    }
}
