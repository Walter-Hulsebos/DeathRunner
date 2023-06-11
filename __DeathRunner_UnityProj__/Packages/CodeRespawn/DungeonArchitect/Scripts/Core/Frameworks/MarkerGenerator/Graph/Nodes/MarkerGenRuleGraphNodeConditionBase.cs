using DungeonArchitect.Graphs;
using DungeonArchitect.MarkerGenerator.Pins;
using UnityEngine;

namespace DungeonArchitect.MarkerGenerator.Nodes
{
    public abstract class MarkerGenRuleGraphNodeConditionBase : MarkerGenRuleGraphNode
    {
        public override Color BodyColor => new(0.15f, 0.1f, 0.1f);
        public override Color TitleColor => new(0.3f, 0.1f, 0.1f);

        protected void CreateInputPin(string pinName)
        {
            var pin = CreatePinOfType<MarkerGenRuleGraphPinBool>(GraphPinType.Input);
            pin.text = pinName;
        }
        
        protected void CreateOutputPin(string pinName)
        {
            var pin = CreatePinOfType<MarkerGenRuleGraphPinBool>(GraphPinType.Output);
            pin.text = pinName;
        }
    }
}