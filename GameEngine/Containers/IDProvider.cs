
using System.ComponentModel;

namespace GameEngine.Containers;

public class IDProvider
{
    public int Capacity { get => Nodes.Count; }
    public int Count { get; private set; }

    public int Allocate()
    {
        int id = 0;

        if (Count == Capacity)
        {
            id = Count;

            Nodes.Add(NodeStatus.AllUsed);

            ++Count;

            return id;
        }

        ++Count;

        while (HasFlag(Nodes[id], NodeStatus.Used))
        {
            int left = 2 * id + 1;
            int right = 2 * id + 2;

            if (left < Capacity && (!HasFlag(Nodes[left], NodeStatus.Used) || !HasFlag(Nodes[left], NodeStatus.ChildrenUsed)))
            {
                id = left;

                continue;
            }

            if (right < Capacity && (!HasFlag(Nodes[right], NodeStatus.Used) || !HasFlag(Nodes[right], NodeStatus.ChildrenUsed)))
            {
                id = right;

                continue;
            }

            throw new InvalidOperationException($"Couldn't find id at {id} with {left} and {right} nodes with capacity {Capacity}"); // something went seriously wrong
        }

        Nodes[id] |= NodeStatus.Used;

        if (id > 0)
        {
            int currentId = (id - 1) / 2;

            while (currentId >= 0)
            {
                int left = 2 * currentId + 1;
                int right = 2 * currentId + 2;

                Nodes[currentId] = Nodes[currentId] & NodeStatus.Used;

                bool leftUsed = left >= Capacity || Nodes[left] == NodeStatus.AllUsed;
                bool rightUsed = right >= Capacity || Nodes[right] == NodeStatus.AllUsed;

                if (leftUsed && rightUsed)
                {
                    Nodes[currentId] |= NodeStatus.ChildrenUsed;
                }

                if (currentId == 0)
                {
                    break;
                }

                currentId = (currentId - 1) / 2;
            }
        }

        return id;
    }

    public void Free(int id)
    {
        if (id < 0 || id >= Capacity)
        {
            throw new IndexOutOfRangeException($"Freed ID {id} is out of bounds with capacity {Capacity}");
        }

        if (!IsAllocated(id))
        {
            throw new InvalidOperationException($"Attempt to free id {id} that is currently not in use with capacity {Capacity}");
        }

        Nodes[id] &= NodeStatus.ChildrenUsed;

        --Count;

        if (id == 0)
        {
            return;
        }

        id = (id - 1) / 2;

        while (id >= 0)
        {
            Nodes[id] &= NodeStatus.Used;

            if (id == 0)
            {
                break;
            }

            id = (id - 1) / 2;
        }
    }

    public int Allocate<T>(List<T> container, T value)
    {
        int id = Allocate();

        if (id > container.Count)
        {
            throw new IndexOutOfRangeException($"Allocated ID {id} is out of bounds of provided container of length {container.Count} with single Add");
        }

        if (id == container.Count)
        {
            container.Add(value);
        }
        else
        {
            container[id] = value;
        }

        return id;
    }

    public void Free<T>(int id, List<T> container, T nullValue)
    {
        Free(id);

        if (id >= container.Count)
        {
            throw new IndexOutOfRangeException($"Freed ID {id} is out of bounds of provided container of length {container.Count}");
        }

        container[id] = nullValue;
    }

    public bool IsAllocated(int id)
    {
        if (id < 0 || id >= Capacity)
        {
            return false;
        }

        return HasFlag(Nodes[id], NodeStatus.Used);
    }

    private bool HasFlag(NodeStatus status, NodeStatus flag)
    {
        return (status & flag) == flag;
    }

    private List<NodeStatus> Nodes = new();

    private enum NodeStatus : byte
    {
        Free = 0,
        Used = 1,
        ChildrenUsed = 2,
        AllUsed = 3
    }
}
