using GameEngine.Containers;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using System.Text.Json;
using GameEngine.Utils;

namespace GameEngine.ECS;

public enum ChildUpdateStatus : byte
{
    Failed,
    Succeeded,
    Deferred
}

public enum VisitType : byte
{
    Continue,
    Skip,
    Stop
}

public class EntityJsonConverter : JsonConverter<Entity>
{
    public override Entity Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return new Entity();
    }

    public override void Write(Utf8JsonWriter writer, Entity value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("name");
        writer.WriteStringValue(value.Name);
        writer.WritePropertyName("id");
        writer.WriteNumberValue(value.EntityID);
        writer.Write("transform", value.Transform, options, true);
        writer.WritePropertyName("components");
        writer.WriteStartArray();

        value.ForEachComponent(false, (component) =>
        {
            if (component == value.Transform) return VisitType.Skip;
            if (!component.Replicated) return VisitType.Skip;
            if (!component.ShouldBeReplicated) return VisitType.Skip;

            writer.Write(value.Transform, options);

            return VisitType.Continue;
        });

        writer.WriteEndArray();
        writer.WriteEndObject();
    }
}

[JsonConverter(typeof(EntityJsonConverter))]
public class Entity
{
    public string Name = "Entity";
    public Transform? Transform { get; private set; }
    public Entity? Parent { get => ParentInternal; set => SetParent(value); }
    private Entity? ParentInternal;

    private List<Component> Components = new();
    private List<Entity> Children = new();
    public bool Enabled = true;
    public bool Replicated = true;
    private static List<WeakReference<Entity>> ActiveEntities = new();
    private static IDProvider EntityIDProvider = new();
    public int EntityID { get; private set; }

    public Entity(string name, bool makeTransform = true)
    {
        EntityID = EntityIDProvider.Allocate(ActiveEntities, new WeakReference<Entity>(this));

        Name = name;

        if (makeTransform)
        {
            AddTransform(new());
        }
    }

    public Entity(bool makeTransform = true)
    {
        EntityID = EntityIDProvider.Allocate(ActiveEntities, new WeakReference<Entity>(this));

        if (makeTransform)
        {
            AddTransform(new());
        }
    }

    ~Entity()
    {
        EntityIDProvider.Free(EntityID);
    }

    public static bool TryGetEntity(int id, [MaybeNullWhen(false), NotNullWhen(true)] out Entity? entity)
    {
        entity = null;

        if (!EntityIDProvider.IsAllocated(id))
        {
            return false;
        }

        return ActiveEntities[id].TryGetTarget(out entity);
    }

    public ChildUpdateStatus AddComponent(Component component)
    {
        if (component.ParentObject == this && component.Parent == null)
            return ChildUpdateStatus.Succeeded;

        if (component.ParentObject != null)
        {
            component.ParentObject.RemoveComponent(component);
        }

        component.AttachToInternal(this);

        if (QueueRemovals)
        {
            QueuedComponentAdds.Add(component);

            return ChildUpdateStatus.Deferred;
        }

        Components.Add(component);

        return ChildUpdateStatus.Succeeded;
    }

    public ChildUpdateStatus RemoveComponent(Component component)
    {
        if (component.ParentObject != this)
            return ChildUpdateStatus.Failed;

        if (component.Parent != null)
        {
            return component.Parent.RemoveComponent(component.Parent);
        }

        if (component == Transform)
        {
            Transform = null;
        }

        component.DetatchInternal(this);

        if (QueueRemovals)
        {
            QueuedComponentRemovals.Add(component);

            return ChildUpdateStatus.Deferred;
        }

        Components.Remove(component);

        return ChildUpdateStatus.Succeeded;
    }

    public ChildUpdateStatus AddTransform(Transform component)
    {
        Transform = component;

        return AddComponent(component);
    }

    public ChildUpdateStatus RemoveComponentShallow(Component component)
    {
        if (component.ParentObject != this)
            return ChildUpdateStatus.Failed;

        component.DetatchInternal(this);

        if (QueueRemovals)
        {
            QueuedComponentRemovals.Add(component);

            return ChildUpdateStatus.Deferred;
        }

        Components.Remove(component);

        component.ClearChildren(false);

        return ChildUpdateStatus.Succeeded;
    }

    public ChildUpdateStatus RemoveComponentDeep(Component component)
    {
        if (component.ParentObject != this)
            return ChildUpdateStatus.Failed;

        component.DetatchInternal(this);

        if (QueueRemovals)
        {
            QueuedComponentRemovals.Add(component);

            return ChildUpdateStatus.Deferred;
        }

        Components.Remove(component);

        component.ClearChildren(true);

        return ChildUpdateStatus.Succeeded;
    }

    public bool IsAncestorOf(Entity entity)
    {
        return entity.IsDescendantOf(this);
    }

    public bool IsAncestorOf(Component component)
    {
        return component.IsDescendantOf(this);
    }

    public bool IsDescendantOf(Entity entity)
    {
        Entity? ancestor = Parent;

        while (ancestor != null)
        {
            if (ancestor == entity)
                return true;

            ancestor = ancestor.Parent;
        }

        return false;
    }

    // returns (OldParentStatus, NewParentStatus)
    public (ChildUpdateStatus, ChildUpdateStatus) SetParent(Entity? newParent)
    {
        if (newParent == null)
        {
            return (Remove(), ChildUpdateStatus.Succeeded);
        }

        if (newParent.IsDescendantOf(this))
            return (ChildUpdateStatus.Failed, ChildUpdateStatus.Failed);

        ChildUpdateStatus oldParentStatus = ChildUpdateStatus.Succeeded;

        if (Parent != null)
        {
            oldParentStatus = Remove();
        }

        ParentInternal = newParent;

        if (newParent.QueueRemovals)
        {
            newParent.QueuedAdds.Add(this);

            return (oldParentStatus, ChildUpdateStatus.Deferred);
        }

        newParent.Children.Add(this);

        return (oldParentStatus, ChildUpdateStatus.Succeeded);
    }

    public ChildUpdateStatus Remove(bool deepRemove = false)
    {
        if (Parent == null)
            return ChildUpdateStatus.Failed;

        if (Parent.QueueRemovals)
        {
            Parent.QueuedRemovals.Add(this);
            ParentInternal = null;

            if (QueueRemovals && deepRemove)
            {
                ClearAll = true;
                ClearAllDeep = true;
            }

            return ChildUpdateStatus.Deferred;
        }

        Parent.Children.Remove(this);
        ParentInternal = null;

        if (!deepRemove)
            return ChildUpdateStatus.Succeeded;

        DeepRemove();

        return ChildUpdateStatus.Succeeded;
    }

    private void DeepRemove()
    {
        ParentInternal = null;

        foreach (Entity child in Children)
        {
            child.DeepRemove();
        }

        Children.Clear();
    }

    public Component[] GetComponents()
    {
        if (!QueueRemovals)
        {
            return Components.ToArray();
        }

        int removedComponents = 0;

        foreach (Component component in Components)
        {
            if (component.ParentObject != this || component.Parent != null)
            {
                ++removedComponents;
            }
        }

        if (removedComponents == 0)
        {
            return Components.ToArray();
        }

        Component[] components = new Component[Components.Count - removedComponents];

        int index = 0;

        foreach (Component component in Components)
        {
            if (component.ParentObject == this && component.Parent == null)
            {
                components[index] = component;
                ++index;
            }
        }

        return components;
    }

    public Entity[] GetChildren()
    {
        if (!QueueRemovals)
        {
            return Children.ToArray();
        }

        int removedChildren = 0;

        foreach (Entity entity in Children)
        {
            if (entity.Parent != this)
            {
                ++removedChildren;
            }
        }

        if (removedChildren == 0)
        {
            return Children.ToArray();
        }

        Entity[] children = new Entity[Children.Count - removedChildren];

        int index = 0;

        foreach (Entity entity in Children)
        {
            if (entity.Parent == this)
            {
                children[index] = entity;
                ++index;
            }
        }

        return children;
    }

    public VisitType ForEachComponent(bool recursive, Func<Component, VisitType> callback)
    {
        foreach (Component component in Components)
        {
            VisitType type = callback(component);

            if (type == VisitType.Stop) return type;
            if (type == VisitType.Skip) continue;

            if (recursive)
            {
                type = component.ForEachChild(true, callback);

                if (type == VisitType.Stop) return type;
            }
        }

        return VisitType.Continue;
    }

    /* This method is slow and mostly used for unit testing. Prefer to use IsAncestorOf and IsDescendantOf instead */
    public bool Contains(Component component)
    {
        foreach (Component child in GetComponents())
        {
            if (child == component || child.Contains(component))
            {
                return true;
            }
        }

        return false;
    }

    /* This method is slow and mostly used for unit testing. Prefer to use IsAncestorOf and IsDescendantOf instead */
    public bool Contains(Entity entity)
    {
        foreach (Entity child in GetChildren())
        {
            if (child == entity || child.Contains(entity))
            {
                return true;
            }
        }

        return false;
    }

    public ChildUpdateStatus ClearComponents(bool clearDeep)
    {
        foreach (Component component in Components)
        {
            component.DetatchInternal(this);
        }

        if (QueueRemovals)
        {
            ClearAllComponents = true;
            ClearAllComponentsDeep |= clearDeep;

            return ChildUpdateStatus.Deferred;
        }

        foreach (Component component in Components)
        {
            if (clearDeep)
            {
                component.ClearChildren(clearDeep);
            }
        }

        Components.Clear();

        return ChildUpdateStatus.Succeeded;
    }

    public ChildUpdateStatus ClearChildren(bool clearDeep)
    {
        if (QueueRemovals)
        {
            ClearAll = true;
            ClearAllDeep |= clearDeep;

            return ChildUpdateStatus.Deferred;
        }

        foreach (Entity entity in Children)
        {
            entity.ParentInternal = null;

            if (clearDeep)
            {
                entity.ClearChildren(clearDeep);
            }
        }

        Children.Clear();

        return ChildUpdateStatus.Succeeded;
    }

    private bool QueueRemovals = false;
    private bool ClearAllComponents = false;
    private bool ClearAllComponentsDeep = false;
    private List<Component> QueuedComponentRemovals = new();
    private List<Component> QueuedComponentAdds = new();

    private bool ClearAll = false;
    private bool ClearAllDeep = false;
    private List<Entity> QueuedRemovals = new();
    private List<Entity> QueuedAdds = new();

    private void UpdateComponents()
    {
        if (ClearAllComponents)
        {
            ClearComponents(ClearAllComponentsDeep);

            ClearAllComponents = false;
            ClearAllComponentsDeep = false;
            QueuedComponentRemovals.Clear();
            QueuedComponentAdds.Clear();

            return;
        }

        ClearAllComponentsDeep = false;

        foreach (Component component in QueuedComponentRemovals)
        {
            if (!(component.ParentObject == this && component.Parent == null))
            {
                Components.Remove(component);
            }
        }

        QueuedComponentRemovals.Clear();

        foreach (Component component in QueuedComponentAdds)
        {
            if (component.ParentObject == this && component.Parent == null && !Components.Contains(component))
            {
                Components.Add(component);
            }
        }

        QueuedComponentAdds.Clear();
    }

    private void UpdateChildren()
    {
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

        foreach (Entity child in QueuedRemovals)
        {
            if (child.Parent != this)
            {
                Children.Remove(this);
            }
        }

        QueuedRemovals.Clear();

        foreach (Entity child in QueuedAdds)
        {
            if (child.Parent == this && !Children.Contains(child))
            {
                Children.Add(this);
            }
        }

        QueuedAdds.Clear();
    }

    public void Update(float delta)
    {
        QueueRemovals = true;

        foreach (Component child in Components)
        {
            child.Update(delta);
        }

        foreach (Entity child in Children)
        {
            child.Update(delta);
        }

        QueueRemovals = false;

        UpdateChildren();
        UpdateComponents();
    }

    public override string ToString() => Name;
}