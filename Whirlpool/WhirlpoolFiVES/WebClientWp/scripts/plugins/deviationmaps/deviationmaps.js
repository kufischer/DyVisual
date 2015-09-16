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

(function() {
    "use strict";

	function updateStep(newValue) {
        $(".currentStep").text(newValue);
    }

    var deviationmaps = function () {
        FIVES.Events.AddEntityGeometryCreatedHandler(this._handleEntityAdded.bind(this));
    };

    var d = deviationmaps.prototype;

	d._handleEntityAdded = function(e) {
		if(e["deviationmap"] && e["deviationmap"]["selectedvector"])
		{
			updateStep(e["deviationmap"]["selectedvector"]);
		}
	};

    FIVES.Plugins.DeviationMaps = new deviationmaps();
})();
