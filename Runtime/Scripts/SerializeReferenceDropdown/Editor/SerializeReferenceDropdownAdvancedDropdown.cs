#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using MegaPint.SerializeReferenceDropdown.Runtime;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace MegaPint.SerializeReferenceDropdown.Editor
{

public class SerializeReferenceDropdownAdvancedDropdown : AdvancedDropdown
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

    private readonly IEnumerable <SerializeReferenceDropdownNameAttribute> _attributes;
    private readonly Dictionary <string, TreeElement> _dropdownTree = new();
    private readonly Dictionary <AdvancedDropdownItem, int> _itemAndIndexes = new();

    private readonly Action <int> _onSelectedTypeIndex;

    public SerializeReferenceDropdownAdvancedDropdown(
        AdvancedDropdownState state,
        IEnumerable <SerializeReferenceDropdownNameAttribute> attributes,
        Action <int> onSelectedNewType) :
        base(state)
    {
        _attributes = attributes.Where(attr => attr != null);
        _onSelectedTypeIndex = onSelectedNewType;

        minimumSize = new Vector2(minimumSize.x, 300);
    }

    #region Protected Methods

    protected override AdvancedDropdownItem BuildRoot()
    {
        var root = new AdvancedDropdownItem("Requirements");
        _itemAndIndexes.Clear();
        _dropdownTree.Clear();

        var index = 0;

        foreach (SerializeReferenceDropdownNameAttribute attribute in _attributes)
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
                        _itemAndIndexes.Add(element.item, index);
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

                _itemAndIndexes.Add(element.item, index);
                index++;
            }
        }

        AddItems(root, _dropdownTree.Values.ToList());

        return root;
    }

    protected override void ItemSelected(AdvancedDropdownItem item)
    {
        base.ItemSelected(item);

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

    #endregion
}

}
#endif
