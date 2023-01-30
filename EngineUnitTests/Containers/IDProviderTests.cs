using GameEngine.Containers;
using System.Linq;

namespace EngineUnitTests.Containers;

public class IDProviderTests
{
    [Fact]
    public void TestAllocate()
    {
        IDProvider provider = new();
        int allocations = 1000;

        int lastId = -1;

        for (int i = 0; i < allocations; ++i)
        {
            int id = provider.Allocate();

            Assert.Equal(lastId + 1, id);
            Assert.Equal(i + 1, provider.Capacity);
            Assert.Equal(i + 1, provider.Count);

            for (int j = 0; j <= i; ++j)
            {
                Assert.True(provider.IsAllocated(j));
            }

            lastId = id;
        }
    }

    [Fact]

    public void TestFree()
    {
        IDProvider provider = new();
        int allocations = 1000;

        for (int i = 0; i < allocations; ++i)
        {
            provider.Allocate();

            Assert.Equal(i + 1, provider.Capacity);
            Assert.Equal(i + 1, provider.Count);
        }

        int count = allocations;

        for (int i = allocations; i > 0; --i)
        {
            --count;

            provider.Free(i - 1);

            Assert.Equal(allocations, provider.Capacity);
            Assert.Equal(count, provider.Count);

            for (int j = 0; j < allocations; ++j)
            {
                Assert.Equal(j < i - 1, provider.IsAllocated(j));
            }
        }
    }

    [Fact]
    public void TestShuffle()
    {
        IDProvider provider = new();
        List<(int, bool)> allocated = new();
        int allocations = 1000;
        List<int> indices = new(allocations);

        for (int i = 0; i < allocations; ++i)
        {
            indices.Add(0);

            if (i > 0)
            {
                allocated.Insert(i.GetHashCode() % allocated.Count, (i, true));
            }
            else
            {
                allocated.Add((0, true));
            }

            provider.Allocate();
        }

        for (int i = 0; i < allocations; ++i)
        {
            indices[allocated[i].Item1] = i;
        }

        int count = allocations;

        for (int i = 0; i < allocations; ++i)
        {
            --count;

            (int, bool) allocation = allocated[i];

            provider.Free(allocation.Item1);
            allocation.Item2 = false;
            allocated[i] = allocation;

            Assert.Equal(allocations, provider.Capacity);
            Assert.Equal(count, provider.Count);

            for (int j = 0; j < allocations; ++j)
            {
                Assert.Equal(allocated[j].Item2, provider.IsAllocated(allocated[j].Item1));
            }
        }

        Assert.Equal(allocations, provider.Capacity);
        Assert.Equal(0, provider.Count);

        for (int i = 0; i < allocations; ++i)
        {
            ++count;

            int id = provider.Allocate();

            Assert.Equal(allocations, provider.Capacity);
            Assert.Equal(count, provider.Count);

            int index = indices[id];

            allocated[index] = (id, true);

            for (int j = 0; j < allocations; ++j)
            {
                Assert.Equal(allocated[j].Item2, provider.IsAllocated(allocated[j].Item1));
            }
        }

        int middle = allocations / 2;

        for (int i = 0; i < middle; ++i)
        {
            --count;

            (int, bool) allocation = allocated[i];

            provider.Free(allocation.Item1);
            allocation.Item2 = false;
            allocated[i] = allocation;

            Assert.Equal(allocations, provider.Capacity);
            Assert.Equal(count, provider.Count);

            for (int j = 0; j < allocations; ++j)
            {
                Assert.Equal(allocated[j].Item2, provider.IsAllocated(allocated[j].Item1));
            }
        }

        Assert.Equal(allocations, provider.Capacity);
        Assert.Equal(count, provider.Count);
        Assert.Equal(allocations - middle, count);

        for (int j = 0; j < middle; ++j)
        {
            int i = j + middle;

            {
                ++count;

                int id = provider.Allocate();

                Assert.Equal(allocations, provider.Capacity);
                Assert.Equal(count, provider.Count);

                int index = indices[id];

                allocated[index] = (id, true);

                for (int k = 0; k < allocations; ++k)
                {
                    Assert.Equal(allocated[k].Item2, provider.IsAllocated(allocated[k].Item1));
                }
            }

            {
                --count;

                (int, bool) allocation = allocated[i];

                provider.Free(allocation.Item1);
                allocation.Item2 = false;
                allocated[i] = allocation;

                Assert.Equal(allocations, provider.Capacity);
                Assert.Equal(count, provider.Count);

                for (int k = 0; k < allocations; ++k)
                {
                    Assert.Equal(allocated[k].Item2, provider.IsAllocated(allocated[k].Item1));
                }
            }
        }

        Assert.Equal(allocations, provider.Capacity);
        Assert.Equal(count, provider.Count);
        Assert.Equal(allocations - middle, count);

        for (int i = 0; i < allocations; ++i)
        {
            (int, bool) allocation = allocated[i];

            if (allocation.Item2)
            {
                --count;

                provider.Free(allocation.Item1);
                allocation.Item2 = false;
                allocated[i] = allocation;

                for (int k = 0; k < allocations; ++k)
                {
                    Assert.Equal(allocated[k].Item2, provider.IsAllocated(allocated[k].Item1));
                }
            }
        }

        (int, bool) last = allocated.Last();

        if (last.Item2)
        {
            --count;

            provider.Free(last.Item1);
            last.Item2 = false;
            allocated[allocated.Count - 1] = last;
        }

        for (int k = 0; k < allocations; ++k)
        {
            Assert.Equal(allocated[k].Item2, provider.IsAllocated(allocated[k].Item1));
            Assert.False(allocated[k].Item2);
        }
    }
}