using DungeonArchitect.Graphs;
using DungeonArchitect.MarkerGenerator.Pins;
using UnityEngine;

namespace DungeonArchitect.MarkerGenerator.Nodes.Condition
{
    public class MarkerGenRuleNodeResult : MarkerGenRuleGraphNodeConditionBase
    {
        public override Color BodyColor => new(0.3f, 0.1f, 0.1f);
        public override Color TitleColor => new(0.4f, 0.1f, 0.1f);
        
        public override string Title => "Should Select?";

        public override void Initialize(string id, Graph graph)
        {
            base.Initialize(id, graph);

            canBeDeleted = false;
        }

        protected override void CreateDefaultPins()
        {
            CreateInputPin("Result");
        }
    }
}