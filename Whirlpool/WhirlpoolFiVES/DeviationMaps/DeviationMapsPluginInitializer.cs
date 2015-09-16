using FIVES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviationMapsPlugin
{
    public class DeviationMapsPluginInitializer : IPluginInitializer
    {
        #region PlugiInitializer
        public string Name
        {
            get { return "DeviationMaps"; }
        }

        public List<string> PluginDependencies
        {
            get { return new List<string>{ "SINFONI" }; }
        }

        public List<string> ComponentDependencies
        {
            get { return new List<string>{"mesh", "location"}; }
        }

        public void Initialize()
        {
            RegisterComponent();
            AddEntity();
        }

        public void Shutdown()
        {

        }
        #endregion

        /// <summary>
        /// Register component that carries information about which model is used and what deviation
        /// map should be displayed in the synchronized views
        /// </summary>
        private void RegisterComponent()
        {
            ComponentDefinition deviationMapComponent = new ComponentDefinition("deviationmap");
            deviationMapComponent.AddAttribute<int>("selectedvector", 1);
            deviationMapComponent.AddAttribute<float>("threshold", 1.5f);
            ComponentRegistry.Instance.Register(deviationMapComponent);
        }

        private void AddEntity()
        {
            Entity e = new Entity();
            e["mesh"]["uri"].Suggest(modelUri);
            e["location"]["position"].Suggest(new Vector(0f, 0f, 0f));
            e["location"]["orientation"].Suggest(new Quat(0f, 0f, 1f, 0f));
            e["deviationmap"]["selectedvector"].Suggest(10);
            World.Instance.Add(e);
        }

        private string modelUri = "resources/models/deviationmap/model.xml";
    }
}
