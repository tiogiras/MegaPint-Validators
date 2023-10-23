using System;
using System.Collections.Generic;
using System.Linq;
using SerializeReferenceDropdown.Runtime;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.UI;

namespace SerializeReferenceDropdown.Editor
{
    public class SerializeReferenceDropdownAdvancedDropdown : AdvancedDropdown
    {
        private readonly IEnumerable<SerializeReferenceDropdownNameAttribute> _attributes;
        private readonly Dictionary<AdvancedDropdownItem, int> _itemAndIndexes = new();
        private readonly Dictionary <string, TreeElement> _dropdownTree = new();
        
        private readonly Action<int> _onSelectedTypeIndex;

        private class TreeElement : IComparable <TreeElement>
        {
            public AdvancedDropdownItem item;
            public Dictionary <string, TreeElement> subItems;
            
            public int order;

            public TreeElement(string typeName, int order)
            {
                item = new AdvancedDropdownItem(typeName);
                subItems = new Dictionary <string, TreeElement>();
                this.order = order;
            }

            public int CompareTo(TreeElement other)
            {
                if (order > other.order)
                    return 1;

                if (order < other.order)
                    return -1;

                if (subItems.Count > 0 && other.subItems.Count == 0)
                    return -1;

                if (subItems.Count == 0 && other.subItems.Count > 0)
                    return 1;

                return 0;
            }
        }

        public SerializeReferenceDropdownAdvancedDropdown(AdvancedDropdownState state, IEnumerable<SerializeReferenceDropdownNameAttribute> attributes,
            Action<int> onSelectedNewType) :
            base(state)
        {
            _attributes = attributes;
            _onSelectedTypeIndex = onSelectedNewType;

            minimumSize = new Vector2(minimumSize.x, 300);
        }

        protected override AdvancedDropdownItem BuildRoot()
        {
            var root = new AdvancedDropdownItem("Requirements");
            _itemAndIndexes.Clear();
            _dropdownTree.Clear();

            var index = 0;
            
            foreach (var attribute in _attributes)
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

                        if (i == parts.Length - 1)
                        {
                            _itemAndIndexes.Add(element.item, index);
                            index++;
                        }

                        if (parent == null)
                        {
                            _dropdownTree.TryAdd(part, element);
                            parent = _dropdownTree[part];
                        }
                        else
                        {
                            parent.subItems.TryAdd(part, element);
                            parent = parent.subItems[part];
                        }
                    }
                }
                else
                {
                    var element = new TreeElement(attribute.name, attribute.menuOrder is {Length: > 0}
                                                      ? attribute.menuOrder[0]
                                                      : default);
                    
                    _dropdownTree.TryAdd(attribute.name, element);
                    
                    _itemAndIndexes.Add(element.item, index);
                    index++;
                }
            }
            
            AddItems(root, _dropdownTree.Values.ToList());

            return root;
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
        
        private void AddItem(AdvancedDropdownItem parent, TreeElement element)
        {
            parent.AddChild(element.item);
        }
        

        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            base.ItemSelected(item);
            if (_itemAndIndexes.TryGetValue(item, out var index))
            {
                _onSelectedTypeIndex.Invoke(index);
            }
        }
    }
}