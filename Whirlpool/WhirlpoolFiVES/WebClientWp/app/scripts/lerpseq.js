
Xflow.registerOperator("xflow.lerp1Seq", {
    outputs: [  {type: 'float', name: 'result'}],
    params:  [  {type: 'float', source: 'sequence'},
        {type: 'float', source: 'key'}],
    mapping: [  { name: 'value1', source: 'sequence', sequence: 1, keySource: 'key'},
        { name: 'value2', source: 'sequence', sequence: 2, keySource: 'key'},
        { name: 'weight', source: 'sequence', sequence: 3, keySource: 'key'}],
    evaluate_core: function(result, value1, value2, weight){
        var invWeight = 1 - weight[0];
        result[0] = invWeight*value1[0] + weight[0]*value2[0];
    }
});


/*Xflow.registerOperator("xflow.lerpSeqAsync", {
    outputs: [  {type: 'float3', name: 'result'}],
    params:  [  {type: 'float3', source: 'sequence'},
        {type: 'float', source: 'key'}],
    mapping: [  { name: 'value1', source: 'sequence', sequence: Xflow.SEQUENCE.PREV_BUFFER, keySource: 'key'},
        { name: 'value2', source: 'sequence', sequence: Xflow.SEQUENCE.NEXT_BUFFER, keySource: 'key'},
        { name: 'weight', source: 'sequence', sequence: Xflow.SEQUENCE.LINEAR_WEIGHT, keySource: 'key'}],
    evaluate_async: function(result, value1, value2, weight, info, callback){
        var i = info.iterateCount, off0, off1, off2;
        while(i--){
            off0 = (info.iterFlag[0] ? i : 0)*3;
            off1 = (info.iterFlag[1] ? i : 0)*3;
            off2 = info.iterFlag[2] ? i : 0;
            var invWeight = 1 - weight[off2];
            result[i*3] = invWeight*value1[off0] + weight[off2]*value2[off1];
            result[i*3+1] = invWeight*value1[off0+1] + weight[off2]*value2[off1+1];
            result[i*3+2] = invWeight*value1[off0+2] + weight[off2]*value2[off1+2];
        }
        window.setTimeout(callback, 200);
    }
});


Xflow.registerOperator("xflow.lerpKeys", {
    outputs: [  {type: 'float3', name: 'result'}],
    params:  [  {type: 'float', source: 'keys', array: true},
        {type: 'float3', source: 'values', array: true},
        {type: 'float', source: 'key'}],
    alloc: function(sizes, keys, values, key)
    {
        sizes['result'] = 3;
    },
    evaluate: function(result, keys, values, key) {
        var maxIdx = Math.min(keys.length, Math.floor(values.length / 3));
        var idx = Xflow.utils.binarySearch(keys, key[0], maxIdx);

        if(idx < 0 || idx == maxIdx - 1){
            idx = Math.max(0,idx);
            result[0] = values[3*idx];
            result[1] = values[3*idx+1];
            result[2] = values[3*idx+2];
        }
        else{
            var weight = (key[0] - keys[idx]) / (keys[idx+1] - keys[idx]);
            var invWeight = 1 - weight;
            result[0] = invWeight*values[3*idx] + weight*values[3*idx + 3];
            result[1] = invWeight*values[3*idx+1] + weight*values[3*idx + 4];
            result[2] = invWeight*values[3*idx+2] + weight*values[3*idx + 5];
        }
    }
});
*/




