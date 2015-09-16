var FIVES = FIVES || {};
FIVES.Plugins = FIVES.Plugins || {};

(function (){
    "use strict";

    var BVHAction = function()
    {
        this.actionData = [];
    };

    var a = BVHAction.prototype;

    a.checkInArray = function(array, data)
    {
        if (array != null)
        {
            for(var i = 0; i<array.length; i++)
            {
                if(array[i][0] == data[0])
                {
                    //can not happen in same frame twice
                    return true;
                }
            }
        }

        return false;
    }

    a.initActionData = function(recvString)
    {
        var nameString = "";
        var counter = 0;
        var data = [];
        for (var i=0; i<recvString.length; i++)
        {
            if (recvString[i] == ":")
            {
                if (counter > 0)
                    if (!this.checkInArray(this.actionData, data))
                            this.actionData.push(data);
                data = [];
                counter = counter + 1;
                continue;
            }

            if (counter == 0) //loop number
            {
                continue;
            }

            if (recvString[i] != "," && counter > 0)
            {
                nameString += recvString[i];
            }
            else
            {
                data.push(nameString);
                nameString = "";
            }
        }

        //console.log(this.actionData);
    }



    FIVES.Plugins.BVHAction = new BVHAction();
}());/**
 * Created by Oscar on 2015/4/29.
 */