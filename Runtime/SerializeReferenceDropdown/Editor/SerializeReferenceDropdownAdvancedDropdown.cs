using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace SerializeReferenceDropdown.Editor
{
    public class SerializeReferenceDropdownAdvancedDropdown : AdvancedDropdown
    {
        private readonly IEnumerable<string> _typeNames;
        private readonly Dictionary<AdvancedDropdownItem, int> _itemAndIndexes = new();
        private readonly Dictionary <string, TreeElement> _dropdownTree = new();
        
        private readonly Action<int> _onSelectedTypeIndex;

        private class TreeElement
        {
            public AdvancedDropdownItem item;
            public Dictionary <string, TreeElement> subItems;

            public TreeElement(string typeName)
            {
                item = new AdvancedDropdownItem(typeName);
                subItems = new Dictionary <string, TreeElement>();
            }
        }

        public SerializeReferenceDropdownAdvancedDropdown(AdvancedDropdownState state, IEnumerable<string> typeNames,
            Action<int> onSelectedNewType) :
            base(state)
        {
            _typeNames = typeNames;
            _onSelectedTypeIndex = onSelectedNewType;
        }

        protected override AdvancedDropdownItem BuildRoot()
        {
            var root = new AdvancedDropdownItem("Requirements");
            _itemAndIndexes.Clear();
            _dropdownTree.Clear();

            var index = 0;
            
            foreach (var typeName in _typeNames)
            {
                if (typeName.Contains("/"))
                {
                    var parts = typeName.Split("/");

                    TreeElement parent = null;

                    for (var i = 0; i < parts.Length; i++)
                    {
                        var part = parts[i];
                        var element = new TreeElement(part);

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
                    var element = new TreeElement(typeName);
                    _dropdownTree.TryAdd(typeName, element);
                    
                    _itemAndIndexes.Add(element.item, index);
                    index++;
                }
            }
            
            
            AddItems(root, _dropdownTree.Values.ToList());

            return root;
        }

        private void AddItems(AdvancedDropdownItem parent, List <TreeElement> elements)
        {
            foreach (TreeElement element in elements)
            {
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