// This file is part of FiVES.
//
// FiVES is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License version 3
// (LGPL v3) as published by the Free Software Foundation.
//
// FiVES is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU LGPL License
// along with FiVES.  If not, see <http://www.gnu.org/licenses/>.

var FIVES = FIVES || {};
FIVES.Plugins = FIVES.Plugins || {};

(function () {
    "use strict";

    var renderable = function () {
        FIVES.Events.AddEntityAddedHandler(this.addMeshForEntity.bind(this));
        FIVES.Events.AddOnComponentUpdatedHandler(this.updateMesh.bind(this));
    };

    var r = renderable.prototype;

    r.addMeshForEntity = function(entity) {
        if(entity.mesh.uri)
            FIVES.Resources.ResourceManager.loadExternalResource(entity, this._addMeshToScene.bind(this));
    };

    r._addMeshToScene = function(meshDocument, entityGuid) {
        var entity = FIVES.Models.EntityRegistry.getEntity(entityGuid);
        var assetId = this._addMeshDefinitionsToScene(entity, meshDocument);
        this._addXml3dGroupsForMesh(entity, meshDocument, assetId);
        FIVES.Events.EntityGeometryCreated(entity);
    };

    r._addMeshDefinitionsToScene = function(entity, meshDocument) {
        var _xml3dElement = FIVES.Resources.SceneManager.xml3dElement;
        var meshDefinitions = $(meshDocument).children("defs");
        $(_xml3dElement).append(meshDefinitions);
        entity.xml3dView.defElement = meshDefinitions[0];
        var asset = $(entity.xml3dView.defElement).find("asset")[0];
		var assetId = "none";
		if ($(asset).attr("id") !== undefined){
			assetId = $(asset).attr("id");
			//console.log(assetId);
		}
        return assetId;
    };
	
	r._findParentElement = function(entity, xml3dElement){
		//console.log(entity.mesh.parent);
		
		if (entity.mesh.parent === "None"){
			return xml3dElement;
		}else{
			var parentId = "Entity-"+entity.mesh.parent
			var parent = $(xml3dElement).find("#"+parentId)[0];
			//console.log(entity);
			//console.log(parent);
			if (parent !== undefined){
				return parent;

			}else{
				console.log("Warning: did not find parent "+parentId);
				return xml3dElement;
		   }
		}
	}
    r._addXml3dGroupsForMesh = function(entity, meshDocument, assetId) {
        var _xml3dElement = FIVES.Resources.SceneManager.xml3dElement;
		var parentElement = this._findParentElement(entity, _xml3dElement)
		var meshGroup = $(meshDocument).children("model");
        var entityGroup = this._createParentGroupForEntity(entity);
		
		
		entity.xml3dView.groupElement = entityGroup;
        parentElement.appendChild(entity.xml3dView.groupElement);
		
		if (meshGroup !== undefined){
			$(entity.xml3dView.groupElement).append(meshGroup);
		}
		if (assetId != "none"){
			var entityModelElement = this._createModelForEntity(entity, assetId)
			$(entityGroup).append(entityModelElement);
			//console.log(entityModelElement); 
		}
        this.updateMesh(entity);

    };

    r._createParentGroupForEntity = function(entity) {
        var entityGroup = XML3D.createElement("group");
        entityGroup.setAttribute("id", "Entity-" + entity.guid);
        entityGroup.setAttribute("transform", "#transform-" + entity.guid );
        return entityGroup;
    };

	
	r._createModelForEntity = function(entity, assetId) {
        var entityModelElement = XML3D.createElement("model");
        entityModelElement.setAttribute("src", "#" + assetId);
        return entityModelElement;
    };

	
    r.updateMesh = function(entity) {
        // FIXME: We do not support changes to mesh resource attribute. The correct approach would be to remove existing
        // entity (group and transform elements) from the scene graph and re-create them with the new URI. However,
        // removing the group element, which has a reference to the data element within this group, causes crashes in
        // current implementation of xml3d.js (https://github.com/xml3d/xml3d.js/issues/50).
        // Once this is fixed, we should use the following code instead:
        //scm.removeEntity(entity);
        //scm.addMeshForEntity(entity).
        if (entity.xml3dView.groupElement !== undefined)
        	entity.xml3dView.groupElement.setAttribute("visible", entity["mesh"]["visible"]);
    };

    FIVES.Plugins.Renderable = new renderable();

}());

