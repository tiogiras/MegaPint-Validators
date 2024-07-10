using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.IMGUI.Controls;

namespace MegaPint.ValidationRequirement.Requirements.GameObjectValidation.RequireComponentDropdown
{

public class Dropdown : AdvancedDropdown
{
    private readonly Dictionary <AdvancedDropdownItem, string> _itemFullPaths;

    private readonly Action <string, string> _onItemSelected;
    private readonly string[] _types;

    public Dropdown(AdvancedDropdownState state, string[] types, Action <string, string> onItemSelected) : base(state)
    {
        _types = types;
        _onItemSelected = onItemSelected;

        _itemFullPaths = new Dictionary <AdvancedDropdownItem, string>();
    }

    #region Protected Methods

    protected override AdvancedDropdownItem BuildRoot()
    {
        _itemFullPaths.Clear();

        var root = new AdvancedDropdownItem("Components");

        foreach (var type in _types)
            CreateDropdownRecursively(root, type, type);

        return root;
    }

    protected override void ItemSelected(AdvancedDropdownItem item)
    {
        _onItemSelected?.Invoke(item.name, _itemFullPaths[item]);
    }

    #endregion

    #region Private Methods

    private void CreateDropdownRecursively(AdvancedDropdownItem parent, string path, string fullPath)
    {
        if (path.Contains("."))
        {
            var parts = path.Split(".");
            var part = parts[0];

            AdvancedDropdownItem existingChild = parent.children.FirstOrDefault(child => child.name.Equals(part));

            if (existingChild == null)
            {
                existingChild = new AdvancedDropdownItem(part);
                parent.AddChild(existingChild);
            }

            var newPath = string.Join(".", parts[1..]);
            CreateDropdownRecursively(existingChild, newPath, fullPath);
        }
        else
        {
            var item = new AdvancedDropdownItem(path);
            _itemFullPaths.Add(item, fullPath);
            parent.AddChild(item);
        }
    }

    #endregion
}

}
