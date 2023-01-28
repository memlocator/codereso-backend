using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine;
public class Component
{
    public string Name = "Component";
    public Entity? ParentObject { get; private set; }
    public Component? Parent { get; private set; }
    public List<Component> Children { get; private set; } = new();

    public void AddComponent(Component component)
    {
        component.Parent = this;
        component.AttachToInternal(ParentObject);
        component.ParentObject = ParentObject;
        Children.Add(component);
    }

    public void RemoveComponent(Component component)
    {
        if (component.Parent != this)
            return;

        Children.Remove(component);
        component.Parent = null;
        component.ParentObject = null;
    }

    public void ClearChildren(bool deep)
    {
        foreach (Component child in Children)
        {
            child.Parent = null;
            child.ParentObject = null;

            if (deep)
            {
                child.ClearChildren(deep);
            }
        }

        Children.Clear();
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
    public void DetatchInternal()
    {
        if (ParentObject == null) return;

        ParentObject = null;

        foreach (Component component in Children)
        {
            component.DetatchInternal();
        }
    }
}