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
        
        private readonly Action<int> _onSelectedTypeIndex;

        private class Item
        {
            public Dictionary <string, Item> subItems;
            public string typeName;

            public Item(string typeName)
            {
                subItems = new Dictionary <string, Item>();
                this.typeName = typeName;
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

            Dictionary <string, Item> itemStructure = new();

            foreach (var typeName in _typeNames)
            {
                if (!typeName.Contains("/"))
                    itemStructure.TryAdd(typeName, null);
                else
                {
                    var items = typeName.Split("/");
                    
                    itemStructure.TryAdd(items[0], new Item(items[0]));

                    itemStructure[items[0]].subItems = RecordSubItems(itemStructure[items[0]].subItems);
                }
            }
            
            var index = 0;

            foreach (KeyValuePair<string,Item> keyValuePair in itemStructure)
            {
                var item = new AdvancedDropdownItem(keyValuePair.Key);
                root.AddChild(item);

                if (keyValuePair.Value == null)
                {
                    _itemAndIndexes.Add(item, index);
                    index++;
                    continue;
                }
                
                AddSubItems(keyValuePair.Value.subItems.Values.ToList(), root, ref index);
            }
            
            return root;
        }

        private Dictionary <string, Item> RecordSubItems(Dictionary <string, Item> items)
        {
            // TODO sub items are not found 
            // TODO need to be found recursively

            return null;
        }
        
        private void AddSubItems(List <Item> items, AdvancedDropdownItem parent, ref int index)
        {
            foreach (Item item in items)
            {
                /*if (item == null)
                    continue;*/
                
                var subItem = new AdvancedDropdownItem(item.typeName);
                parent.AddChild(subItem);

                if (item.subItems.Count > 0)
                    AddSubItems(item.subItems.Values.ToList(), subItem, ref index);
                else
                {
                    _itemAndIndexes.Add(subItem, index);
                    index++;
                }
            }
        }

        private void AddItem(string typeName, ref int index)
        {
            
        }

        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            base.ItemSelected(item);
            if (_itemAndIndexes.TryGetValue(item, out var index)) // TODO <- rework required Only add subItems and not categories to dict with index
            {
                _onSelectedTypeIndex.Invoke(index);
            }
        }
    }
}