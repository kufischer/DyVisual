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

var CARAMEL = CARAMEL || {};
CARAMEL.Utility = CARAMEL.Utility || {};


(function() {
    /** Loader for XML3D elements from a remote server using Ajax. 
     *  Call loadXML3D() with a url and a callback to be invoked, when 
     *  loading finished. 
     *  
     *  The Loader modifies all mesh's and img's src attributes. It 
     *  makes them absolute and point to the configured server. It 
     *  will also remove the leading "file:///" prefix if it's given.
     */
    CARAMEL.Utility.RemoteXML3DLoader = new XMOT.Singleton({
		_entityQueue : [],
		_handledEntities : [],
        _cachedDocuments : {},
        _pendingRequests : {},
        /**
         * @param {string} url the url to load
         * @param {function(url: string, xml3d: <Object>)} loadedCB the callback to invoke when loading finished.
         */
        loadXML3D: function(fivesObject, loadedCB)
        {
            var uri = fivesObject.mesh.uri;

            // The requested document may have been loaded before and thus is already available. Return the
            // cached document in this case
            if(this._cachedDocuments[uri])
                this._handleLoadedXML3D(fivesObject, this._cachedDocuments[uri], loadedCB);
            // A request to the same document may have been sent, but has not returned yet. Don't send another
            // request but wait for the pending response and operate on that
            else if(this._pendingRequests[uri])
            {
                this._pendingRequests[uri].push({entity: fivesObject, callback: loadedCB});
            }
            // Send a request to retrieve an external document only when requesting a document for the first time
            else
            {
                this._pendingRequests[uri] = [];
                this._pendingRequests[uri].push({entity: fivesObject, callback: loadedCB});
                var self = this;
				console.log(uri);
				$.ajax({
					type: "GET",
					url: uri,
					headers: { "Accept":"model/vnd.xml3d.model+xml, */*; q=0.5"    },
					success: function(response) {
						if(!self._cachedDocuments[uri])
							self._cachedDocuments[uri] = response;

						self._handlePendingRequests(uri);
					},
					error: function(status) {console.error(status)}
				});
            }
        },

        /**
         * A document may be waiting for the response of an earlier request of the same URI. When this response
         * returns, all pending requests to this response are handled
         * @param uri URI to which the pending request was sent
         * @private
         */
        _handlePendingRequests: function(uri) {
            if(this._pendingRequests[uri])
            {
                for(var r in this._pendingRequests[uri])
                {
                    var request = this._pendingRequests[uri][r];
                    this._handleLoadedXML3D(request.entity, this._cachedDocuments[uri], request.callback);
                }
                delete this._pendingRequests[uri];
            }
        },


		_hasParent: function(fivesObject)
		{
		    var foundParent = true;
        	var mesh = fivesObject.mesh;
        	if (mesh !== undefined){
        		var parent = mesh.parent;
        		if (parent !== undefined && parent !== "None" && parent !== "c47d7ed9-bd64-47b8-80ba-a12bcafe8aef" && parent !== "750057aa-b13b-4e21-95e8-59b95fe1c842"){
        			if (( $.inArray( parent, this._handledEntities ) == -1 )){
						console.log("did not find parent "+parent+" for "+fivesObject.guid);
        				//console.log(this._handledEntities);
        				//console.log(this._entityQueue);
						foundParent = false;
						return foundParent;
        			}
        		}
        		console.log("found parent "+parent);
        	
        	}
        	return foundParent;
		},

        _processEntityQueue: function()
        {
        	var newEntityQueue = [];
        	var numElements = this._entityQueue.length;
        	console.log("start processing queue with length "+numElements);
        	for (i = 0; i < numElements; i++) { 
        		var parameters = this._entityQueue.pop();
        		//console.log(parameters["fivesObject"]);
				if (parameters !== undefined){
					wasAdded = this._processEntity(parameters["fivesObject"], parameters["loadedDocument"], parameters["loadedCB"] );
					if (!wasAdded){
						newEntityQueue.push(parameters);
						//console.log("not recognized as child");
					}
				}
			}
			this._entityQueue = newEntityQueue;
			var numElements = this._entityQueue.length;
			console.log("finished processing queue "+numElements);	
        },
        
        _extractXML3DNode: function(loadedDocument)
        {
        	var xml3dnode = $(loadedDocument).children("xml3d")[0];

			if (xml3dnode === undefined){
				var xml3dnode = $(loadedDocument).find("xml3d")[0];
			}
			return xml3dnode
  		},
		_processEntity(fivesObject, xmlDocument, loadedCB){
			var foundParent = this._hasParent(fivesObject);
			//console.log(foundParent);
        	if (foundParent){
				var xml3dnode = this._extractXML3DNode(xmlDocument);
				if (xml3dnode === undefined){
					console.log("Error coud not parse loadedDocument");
					console.log(loadedDocument);

				}
				var loadedXML3DEl = xml3dnode.cloneNode(true);

				// construct full path to the files by analysing urlOnServer
				var url = fivesObject.mesh.uri;
				var urlLastSlash = url.lastIndexOf("/"); 
				var urlPath = url.slice(0,  urlLastSlash + 1);
				this._adjustReferences(loadedXML3DEl, urlPath);
				this._handledEntities.push(fivesObject.guid);
				// notify load requester
				loadedCB(loadedXML3DEl, fivesObject.guid);
				return true;
        	}else{
        		return false;

        	}
		},
        /** Convert all references to point to the correct server and 
         *  notify the load requester using loadedCB. 
         */
        _handleLoadedXML3D: function(fivesObject, loadedDocument, loadedCB)
        {
        	console.log("process "+fivesObject.guid);
			wasAdded = this._processEntity(fivesObject, loadedDocument, loadedCB);
			if (wasAdded){
				this._processEntityQueue();
            }else{
            	console.log("add to queue");
            	parameters = {}
            	parameters["fivesObject"] = fivesObject;
            	parameters["loadedDocument"] = loadedDocument;
            	parameters["loadedCB"] = loadedCB;
            	this._entityQueue.push(parameters);
            }
          
        },
        
        /** Up to now convert all img's and mesh's elements' src attributes 
         *  to point to this.serverURL. It will remove the "file:///" prefix, 
         *  if given.  
         */
        _adjustReferences: function(node, baseURL)
        {
            // adjust all meshes and images 
            if(node.tagName === "mesh" || node.tagName === "model" || node.tagName === "img"  || node.tagName === "light" || node.tagName === "group" || node.tagName === "data")
            {
				this._adjustReferenceForAttribute(node, baseURL, "src");
				this._adjustReferenceForAttribute(node, baseURL, "shader");
				this._adjustReferenceForAttribute(node, baseURL, "transform");
            }

            // handle children
            _.each(node.childNodes, function(child) {
                this._adjustReferences(child, baseURL); 
            }, this);
        },

		_adjustReferenceForAttribute: function(node, baseURL, attribute)
		{
			var uriStr = $(node).attr(attribute);

			if(uriStr && uriStr.length > 0 && uriStr.indexOf("http://") < 0) // only replace relative paths
			{
				if(uriStr.indexOf("file:///") === 0)
				{
					uriStr = uriStr.slice("file:///".length);
				}

				if(uriStr.indexOf("#") !== 0)
				{
					$(node).attr(attribute, baseURL + uriStr);
				}
			}
		}
    });    
}());

