var absRep = require('../DataService/AbsRepos');
 var monitoringService = new absRep(); 
 var Model = monitoringService.getModel("main", "match");
 module.exports.vulnerabilityFromProduct= function(inputs,callback){
console.log(inputs);
 var provul = [];
     for (p in inputs) { provul.push({ 'product': inputs[p] }); }
     console.log(provul);
 Model.find({ $or:provul},function(err,provuln){
    if(!err){ var proModel = monitoringService.getModel("main", "vulnerability");
        var vuls = [];
        console.log(provuln);
        for (p in provuln) { vuls.push({ 'cve': provuln[p].cveid }); }
        console.log(vuls);
 proModel.find({ $or:vuls},function(e,vulnerabilities){
 if(!e) callback(1,vulnerabilities); else callback(e);});
 } else callback(0,err);
});
};
