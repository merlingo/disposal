//veri modellerinin yonetimi icin kullanilmaktadir
var mongoose = require('mongoose')
    , Schema = mongoose.Schema;

//gerekiyorsa parse edilmesi icin kod parcasi
var functionSchema = new Schema({
    model: String,
    name: String,
    inputType: [String], 
    outputType: String, 
    type: String,
    time: Number,
    func_payload: String 
});

var functionCallSchema = new Schema({
    model: String,
    name: String,
    input: [String],
    output: String, 
    callback: String

});
var functionModel = mongoose.model('functions', functionSchema);
var functionCallModel = mongoose.model('function-calls', functionCallSchema);

module.exports.function = functionModel;
module.exports.functionCall = functionCallModel;

module.exports.getModel = function (modeln) {
    if (modeln == "function") {
        //istenen veri, türüne göre event_attr'si bulunur ve parse edilir.
        return functionModel;
    }
    else if (modeln == "functionCall") {
        //istenen veri, türüne göre event_attr'si bulunur ve parse edilir.
        return functionCallModel;
    }
}