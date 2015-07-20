﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SyncTrayzor.SyncThing.ApiClient
{
    public class ItemStartedEventData
    {
        [JsonProperty("item")]
        public string Item { get; set; }

        [JsonProperty("folder")]
        public string Folder { get; set; }

        [JsonProperty("type")]
        public ItemChangedItemType Type { get; set; }

        [JsonProperty("action")]
        public ItemChangedActionType Action { get; set; }
    }

    public class ItemStartedEvent : Event
    {
        [JsonProperty("data")]
        public ItemStartedEventData Data { get; set; }

        public override void Visit(IEventVisitor visitor)
        {
            visitor.Accept(this);
        }

        public override string ToString()
        {
            return $"<ItemStarted ID={this.Id} Time={this.Time} Item={this.Data.Item,} Folder={this.Data.Folder} Type={this.Data.Type} Action={this.Data.Action}>";
        }
    }
}
