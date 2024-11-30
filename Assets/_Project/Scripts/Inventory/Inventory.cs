using Sirenix.OdinInspector;
using System.Collections.Generic;
using Systems.Persistence;
using UnityEngine;

namespace Systems.Inventory
{
    public class Inventory : MonoBehaviour, IBind<InventoryData>
    {
        [HorizontalGroup("ItemSplit", 0.25f), VerticalGroup("ItemSplit/Right"), HideLabel, PreviewField(100)]
        public Sprite customSprite;

        [SerializeField] InventoryView view;
        [SerializeField] int capacity = 20;
        [SerializeField] List<ItemDetails> startingItems = new List<ItemDetails>();
        [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();

        public InventoryController controller;


        void Awake()
        {
            controller = new InventoryController.Builder(view)
                .WithStartingItems(startingItems)
                .WithCapacity(capacity)
                .Build();
            controller.customSprite = customSprite;
        }

        public void Bind(InventoryData data)
        {
            controller.Bind(data);
            data.Id = Id;
        }

        //public void AddItem(ItemDetails itemDetails)
        //{
        //controller.model.Add(itemDetails);
        //}
    }
}