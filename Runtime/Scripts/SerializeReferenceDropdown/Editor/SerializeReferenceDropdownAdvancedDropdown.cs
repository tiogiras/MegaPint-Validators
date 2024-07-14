#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MegaPint.SerializeReferenceDropdown.Runtime;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace MegaPint.SerializeReferenceDropdown.Editor
{

internal class SerializeReferenceDropdownAdvancedDropdown : AdvancedDropdown
{
    private class TreeElement : IComparable <TreeElement>
    {
        public readonly AdvancedDropdownItem item;

        public readonly int order;
        public readonly Dictionary <string, TreeElement> subItems;

        public TreeElement(string typeName, int order)
        {
            item = new AdvancedDropdownItem(typeName);
            subItems = new Dictionary <string, TreeElement>();
            this.order = order;
        }

        #region Public Methods

        public int CompareTo(TreeElement other)
        {
            if (order > other.order)
                return 1;

            if (order < other.order)
                return -1;

            return subItems.Count switch
                   {
                       > 0 when other.subItems.Count == 0 => -1,
                       0 when other.subItems.Count > 0 => 1,
                       var _ => string.CompareOrdinal(item.name, other.item.name)
                   };
        }

        #endregion
    }

    private readonly Type[] _addedRequirements;

    private readonly IEnumerable <ValidationRequirementNameAttribute> _attributes;

    private readonly Type _currentValue;
    private readonly Dictionary <string, TreeElement> _dropdownTree = new();

    private readonly Dictionary <AdvancedDropdownItem, bool> _itemAndAllowMultiple = new();
    private readonly Dictionary <AdvancedDropdownItem, Type[]> _itemAndIncompatibles = new();
    private readonly Dictionary <AdvancedDropdownItem, int> _itemAndIndexes = new();
    private readonly Dictionary <AdvancedDropdownItem, Type> _itemTypes = new();

    private readonly Action <int> _onSelectedTypeIndex;

    public SerializeReferenceDropdownAdvancedDropdown(
        AdvancedDropdownState state,
        IEnumerable <ValidationRequirementNameAttribute> attributes,
        Type[] addedRequirements,
        Type currentValue,
        Action <int> onSelectedNewType) :
        base(state)
    {
        _attributes = attributes.Where(attr => attr != null);
        _addedRequirements = addedRequirements;
        _onSelectedTypeIndex = onSelectedNewType;
        _currentValue = currentValue;

        minimumSize = new Vector2(minimumSize.x, 300);
    }

    #region Protected Methods

    protected override AdvancedDropdownItem BuildRoot()
    {
        var root = new AdvancedDropdownItem("Requirements");
        _itemAndIndexes.Clear();
        _itemAndAllowMultiple.Clear();
        _itemAndIncompatibles.Clear();

        _itemTypes.Clear();
        _dropdownTree.Clear();

        var index = 0;

        foreach (ValidationRequirementNameAttribute attribute in _attributes)
        {
            if (attribute.name.Contains("/"))
            {
                var parts = attribute.name.Split("/");

                TreeElement parent = null;

                for (var i = 0; i < parts.Length; i++)
                {
                    var part = parts[i];

                    int order = default;

                    if (attribute.menuOrder != null)
                    {
                        if (attribute.menuOrder.Length >= i + 1)
                            order = attribute.menuOrder[i];
                    }

                    var element = new TreeElement(part, order);
                    var key = $"{part}{element.order}";

                    if (i == parts.Length - 1)
                    {
                        _itemTypes.Add(element.item, attribute.requirementType);
                        _itemAndIndexes.Add(element.item, index);
                        _itemAndAllowMultiple.Add(element.item, attribute.allowMultiple);
                        _itemAndIncompatibles.Add(element.item, attribute.incompatibleRequirements);

                        index++;
                    }

                    if (parent == null)
                    {
                        _dropdownTree.TryAdd(key, element);
                        parent = _dropdownTree[key];
                    }
                    else
                    {
                        parent.subItems.TryAdd(key, element);
                        parent = parent.subItems[key];
                    }
                }
            }
            else
            {
                var element = new TreeElement(
                    attribute.name,
                    attribute.menuOrder is {Length: > 0}
                        ? attribute.menuOrder[0]
                        : default);

                _dropdownTree.TryAdd($"{attribute.name}{element.order}", element);

                _itemTypes.Add(element.item, attribute.requirementType);
                _itemAndIndexes.Add(element.item, index);
                _itemAndAllowMultiple.Add(element.item, attribute.allowMultiple);
                _itemAndIncompatibles.Add(element.item, attribute.incompatibleRequirements);

                index++;
            }
        }

        AddItems(root, _dropdownTree.Values.ToList());

        return root;
    }

    protected override void ItemSelected(AdvancedDropdownItem item)
    {
        base.ItemSelected(item);

        if (HasIncompatibleRequirements(item, out Type[] incompatibleRequirements))
        {
            var message = new StringBuilder();

            message.AppendLine(
                "The requirement is not compatible with the following requirements on this gameObject and therefor cannot be added.");

            foreach (Type requirement in incompatibleRequirements)
                message.AppendLine($"- {requirement.Name}");

            Debug.LogWarning(message.ToString()[..^2]);

            return;
        }

        if (!_itemAndAllowMultiple[item] && ItemIsAdded(item))
        {
            Debug.LogWarning("The requirement already exists on this gameObject and cannot be added multiple times.");

            return;
        }

        if (_itemAndIndexes.TryGetValue(item, out var index))
            _onSelectedTypeIndex.Invoke(index);
    }

    #endregion

    #region Private Methods

    private void AddItem(AdvancedDropdownItem parent, TreeElement element)
    {
        parent.AddChild(element.item);
    }

    private void AddItems(AdvancedDropdownItem parent, List <TreeElement> elements)
    {
        elements.Sort();

        var lastOrder = elements[0].order;

        foreach (TreeElement element in elements)
        {
            if (Mathf.Abs(Mathf.Abs(lastOrder) - Mathf.Abs(element.order)) >= 10)
                parent.AddSeparator();

            lastOrder = element.order;

            AddItem(parent, element);

            if (element.subItems.Count > 0)
                AddItems(element.item, element.subItems.Values.ToList());
        }
    }

    private bool HasIncompatibleRequirements(AdvancedDropdownItem item, out Type[] types)
    {
        types = null;

        Type[] incompatibles = _itemAndIncompatibles[item];

        if (incompatibles is not {Length: > 0})
            return false;

        types =
            _addedRequirements.Where(requirement => incompatibles.Contains(requirement)).ToArray();

        if (types.Length == 1 && types[0] == _currentValue)
            return false;

        return types.Length > 0;
    }

    private bool ItemIsAdded(AdvancedDropdownItem item)
    {
        Type type = _itemTypes[item];

        if (type == _currentValue)
            return false;

        return type != null && _addedRequirements.Contains(type);
    }

    #endregion
}

}
#endif
