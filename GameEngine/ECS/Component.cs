using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.ECS;
public class Component
{
    public string Name = "Component";
    public Entity? ParentObject { get; private set; }
    public Component? Parent { get; private set; }
    public List<Component> Children { get; private set; } = new();
    public bool Enabled = true;

    public ChildUpdateStatus AddComponent(Component component)
    {
        if (IsDescendantOf(component))
            return ChildUpdateStatus.Failed;

        if (component.Parent != null)
        {
            component.Parent.RemoveComponent(component);
        }

        if (QueueRemovals)
        {
            QueuedAdds.Add(component);

            return ChildUpdateStatus.Deferred;
        }

        component.Parent = this;
        component.AttachToInternal(ParentObject);
        component.ParentObject = ParentObject;
        Children.Add(component);

        return ChildUpdateStatus.Succeeded;
    }

    public ChildUpdateStatus RemoveComponent(Component component)
    {
        if (component.Parent != this)
            return ChildUpdateStatus.Failed;

        component.Parent = null;
        component.ParentObject = null;

        if (QueueRemovals)
        {
            QueuedRemovals.Add(component);

            return ChildUpdateStatus.Deferred;
        }

        Children.Remove(component);

        return ChildUpdateStatus.Succeeded;
    }

    public ChildUpdateStatus ClearChildren(bool deep)
    {
        if (QueueRemovals)
        {
            ClearAll = true;
            ClearAllDeep |= deep;

            return ChildUpdateStatus.Deferred;
        }

        foreach (Component child in Children)
        {
            child.Parent = null;
            child.ParentObject = null;

            if (deep)
            {
                child.ClearChildren(deep);
            }
            else
            {
                child.DetatchInternal(ParentObject);
            }
        }

        Children.Clear();

        return ChildUpdateStatus.Succeeded;
    }

    public bool IsAncestorOf(Component component)
    {
        return component.IsDescendantOf(this);
    }

    public bool IsDescendantOf(Component component)
    {
        Component? ancestor = Parent;

        while (ancestor != null)
        {
            if (ancestor == component)
                return true;

            ancestor = ancestor.Parent;
        }

        return false;
    }

    public bool IsDescendantOf(Entity entity)
    {
        return ParentObject == entity;
    }

    private bool QueueRemovals = false;
    private bool ClearAll = false;
    private bool ClearAllDeep = false;
    private List<Component> QueuedRemovals = new();
    private List<Component> QueuedAdds = new();

    public virtual void Update(float delta)
    {
        QueueRemovals = true;

        foreach (Component child in Children)
        {
            child.Update(delta);
        }

        QueueRemovals = false;

        if (ClearAll)
        {
            ClearChildren(ClearAllDeep);

            ClearAll = false;
            ClearAllDeep = false;
            QueuedRemovals.Clear();
            QueuedAdds.Clear();

            return;
        }

        ClearAllDeep = false;

        foreach (Component component in QueuedRemovals)
        {
            if (component.Parent != this)
            {
                Children.Remove(component);
            }
        }

        QueuedRemovals.Clear();

        foreach (Component component in QueuedAdds)
        {
            if (component.Parent == this && !Children.Contains(component))
            {
                Children.Add(component);
            }
        }

        QueuedAdds.Clear();
    }

    public Component[] GetChildren()
    {
        if (!QueueRemovals)
        {
            return Children.ToArray();
        }

        int removedChildren = 0;

        foreach (Component component in Children)
        {
            if (component.Parent != this)
            {
                ++removedChildren;
            }
        }

        if (removedChildren == 0)
        {
            return Children.ToArray();
        }

        Component[] children = new Component[Children.Count - removedChildren];

        int index = 0;

        foreach (Component component in Children)
        {
            if (component.Parent == this)
            {
                children[index] = component;
                ++index;
            }
        }

        return children;
    }

    /* This method is slow and mostly used for unit testing. Prefer to use IsAncestorOf and IsDescendantOf instead */
    public bool Contains(Component component)
    {
        foreach (Component child in GetChildren())
        {
            if (child == component || child.Contains(component))
            {
                return true;
            }
        }

        return false;
    }

    /* Warning: only call from Entity */
    public void AttachToInternal(Entity? entity)
    {
        ParentObject = entity;

        foreach (Component child in Children)
        {
            child.AttachToInternal(entity);
        }
    }

    /* Warning: only call from Entity */
    public void DetatchInternal(Entity? parentObject)
    {
        if (ParentObject != parentObject) return;

        ParentObject = null;

        foreach (Component component in Children)
        {
            component.DetatchInternal(parentObject);
        }
    }

    public override string ToString() => Name;
}