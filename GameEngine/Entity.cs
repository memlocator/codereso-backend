namespace GameEngine;

public class Entity
{
    public string Name = "Entity";
    public Transform? Transform { get; private set; }

    private List<Component> Components = new();

    public bool AddComponent(Component component)
    {
        if (component.ParentObject == this && component.Parent == null)
            return true;

        if (component.ParentObject != null)
            return false;

        Components.Add(component);
        component.AttachToInternal(this);

        return true;
    }

    public bool RemoveComponent(Component component)
    {
        if (component.ParentObject != this || component.Parent != null)
            return false;

        Components.Remove(component);
        component.DetatchInternal();

        if (component == Transform)
        {
            Transform = null;
        }

        return true;
    }

    public bool AddTransform(Transform component)
    {
        if (component.ParentObject == this)
        {
            if (component.Parent != null)
                return false;

            Transform = component;

            return true;
        }

        if (component.ParentObject != null)
            return false;

        AddComponent(component);

        Transform = component;

        return true;
    }

    public void RemoveComponentShallow(Component component)
    {
        if (component.ParentObject != this)
            return;

        Components.Remove(component);
        component.DetatchInternal();
        component.ClearChildren(false);
    }

    public void RemoveComponentDeep(Component component)
    {
        if (component.ParentObject != this)
            return;

        Components.Remove(component);
        component.DetatchInternal();
        component.ClearChildren(true);
    }

    public IReadOnlyCollection<Component> GetComponents()
    {
        return Components;
    }
}